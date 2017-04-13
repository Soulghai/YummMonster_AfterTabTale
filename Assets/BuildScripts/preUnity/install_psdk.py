#!/usr/bin/python

import os, sys, getopt
import json
import glob

from uninstall_psdk import delete_psdk
from pprint import pprint
from distutils.dir_util import copy_tree
#from appsDbReader import get_appsdb_json
from xml.etree import ElementTree as ET
import zipfile

installReferenceDict = {
	"core"			: "PSDKCore",
	"installer"		: "PSDKInstaller",
	"appsFlyer"		: "PSDKAppsFlyer",
	"locationMgr"	: "PSDKMonetization",
	"banners"		: "PSDKBanners",
	"gameLevelData"	: "PSDKGameLevelData",
	"rewardedAds"	: "PSDKRewardedAds",
	"splash"		: "PSDKSplash",
	"referrals"		: "PSDKGoogleAnalytics",
	"gameCenter"	: "PSDKGooglePlayServices",
	"socialGame"	: "PSDKGooglePlayServices",
	"crashMonitoringTool"	: "PSDKCrashTool",
	"analytics"		: "PSDKAnalytics"
}



def main(argv):
	currentScriptDirectory=os.path.dirname(os.path.abspath(__file__))
	unityProjectDirectory=os.path.join(currentScriptDirectory,'..','..')
	psdkBranch=None
	unityExePath=None
	for n in range(len(argv)):
		if (argv[n] == '-psdk.branch' and n+1 < len(argv)):
			psdkBranch=argv[n+1]
		if (argv[n] == '-unityExePath' and n+1 < len(argv)):
			unityExePath=argv[n+1]


	if (psdkBranch == None):
		print __file__, ": No PSDK branch specified, not installing PSDK !"
		return

	if (psdkBranch.lower() == 'nosdk'):
		print __file__, ": NoSDK specified, not installing PSDK !"
		return

	if (psdkBranch.lower() == 'master'):
		psdkBranch='0.0.0'

	if (unityExePath != None and unityExePath.startswith('/Applications/Unity_4')):
		print __file__, ": Unity 4 not supported for PSDK automatic installtion !"
		return

	if (len(psdkBranch.split('.')) < 3):
		print __file__, ": PSDK branch too short " + psdkBranch + " , not installing PSDK !"
		return

	print "psdkBranch=" + psdkBranch

	streamingAssetsDirectory = os.path.join(unityProjectDirectory,'Assets','StreamingAssets')

	psdkIosJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_ios.json'))
	if (psdkIosJson == None):
		psdkIosJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_ios_.json'))

	if (psdkIosJson == None):
		print "Error reading ",os.path.join(streamingAssetsDirectory,'psdk_ios.json')," !!"
		return -1

	psdkGoogleJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_google.json'))
	if (psdkGoogleJson == None):
		psdkGoogleJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_google_.json'))

	if (psdkGoogleJson == None):
		print "Error reading ",os.path.join(streamingAssetsDirectory,'psdk_google_.json')," !!"
		return -1

	psdkAmazonJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_amazon.json'))

	psdkPkgsToInstall={ "core":True }
	if (psdkIosJson != None):
		set_psdk_pkgs_to_install(psdkIosJson, psdkPkgsToInstall)
	if (psdkGoogleJson != None):
		set_psdk_pkgs_to_install(psdkGoogleJson, psdkPkgsToInstall)
	if (psdkAmazonJson != None):
		set_psdk_pkgs_to_install(psdkAmazonJson, psdkPkgsToInstall)

	install_psdk_pkgs(psdkPkgsToInstall,psdkBranch,unityProjectDirectory)


def mount_tab_share_repo():
	os.system("mkdir -p /Volumes/REPO; mount_smbfs smb://guest@192.168.200.8/REPO /Volumes/REPO");

def get_installed_psdk_version(unityProjectDirectory,serviceName='core'):
	streamingAssetsDirectory = os.path.join(unityProjectDirectory,'Assets','StreamingAssets')
	psdkVersionFilePath = os.path.join(streamingAssetsDirectory,"psdk","versions",installReferenceDict[serviceName]+".unitypackage.version.txt");
        installedPsdkVersion=None
        try:
                fileContent=None
                with open(psdkVersionFilePath,'r') as f:
                        fileContent=f.read().replace('\n', '')
                if fileContent:
                        #print("---------------------------- \n{}\n --------------------------\n".format(fileContent))
			installedPsdkVersion=fileContent.replace('\n', '').replace(' ', '')
                f.close()
        except:
                pass

        return installedPsdkVersion


def install_psdk_pkgs(psdkPkgsToInstall, psdkBranch, unityProjectDirectory):
	print "Mounting REPO"
	mount_tab_share_repo()
	remotePsdkPath = get_remote_psdk_path(psdkBranch)
	remotePsdkVersion = os.path.basename(remotePsdkPath.rstrip('/'))
        needInstallation=False
        for pkgToInstall in psdkPkgsToInstall:
	    installedPsdkVersion = get_installed_psdk_version(unityProjectDirectory,pkgToInstall);
	    if installedPsdkVersion:
	        print "installedPsdkVersion "+pkgToInstall+":\t" + installedPsdkVersion + ", remotePsdkVersion:" + remotePsdkVersion
	    if (installedPsdkVersion != remotePsdkVersion):
		needInstallation=True
        if not needInstallation:
            print "no need to install psdk, remote and installed are the same " + installedPsdkVersion + " !"
            return
        print "Injecting PSDK " + remotePsdkVersion
	print "Deleting PSDK"
	delete_psdk(unityProjectDirectory)
	print "installing:",psdkPkgsToInstall.keys()," from branch:",  psdkBranch
	for service in psdkPkgsToInstall:
		if psdkPkgsToInstall[service]:
			if service in installReferenceDict:
                                zipFilePath = os.path.join(remotePsdkPath,installReferenceDict[service]+".unity.zip")
                                print 'extracting ' + zipFilePath + ' -> ' + unityProjectDirectory
                                unzipCommand = "unzip -o " + zipFilePath + " -d " + unityProjectDirectory;
                                print unzipCommand
                                os.system(unzipCommand);
                                #with zipfile.ZipFile(zipFilePath, 'r') as zipToExtract:
                                #       zipToExtract.extractall(unityProjectDirectory)


def get_remote_psdk_path(psdkBranch):
    # checking if its a minor or major branch
    minorBranch=None
    mount_tab_share_repo();
    # looking for the exact psdkBranch as major branch
    majorBranchPath = os.path.join('/Volumes','REPO','unity','PublishingSDK',psdkBranch)
    if not os.path.exists(majorBranchPath):
        majorBranch='.'.join(psdkBranch.split('.')[0:len(psdkBranch.split('.'))-1])
        # trying to treat psdk branch as minor branch, e.g. looking for major branch of minor branch
        majorBranchPath = os.path.join('/Volumes','REPO','unity','PublishingSDK',majorBranch)
        if not os.path.exists(majorBranchPath):
            # psdkBrnach is not a major or minor branch, it means we need to find a major branch
            majorVersionsNumbers = []
            refs = {}
            majorBranchPath=None
            for b in glob.glob(os.path.join(os.path.join('/Volumes','REPO','unity','PublishingSDK'), psdkBranch + '*/')):
               lastNumber = int(b[b.rfind('.')+1:b.rfind('/')])
               majorVersionsNumbers.append(lastNumber)
               refs[str(lastNumber)]=b
            majorBranchPath=refs[str(sorted(majorVersionsNumbers,key=int)[-1])]

        if not majorBranchPath:
            #fail build
            print "Didn't find major branch path for psdkBranch ",psdkBranch,"!!! failure !"
            return None

    minorBranchPath=os.path.join('/Volumes','REPO','unity','PublishingSDK',majorBranchPath,psdkBranch)
    if not os.path.exists(minorBranchPath):
        minorBranchPath=None
        #find latest minor verison
        minorVersionsNumbers = []
        refs={}
        for b in glob.glob(os.path.join(majorBranchPath, '*/')):
            lastNumber = int(b[b.rfind('.')+1:b.rfind('/')])
            minorVersionsNumbers.append(lastNumber)
            refs[str(lastNumber)]=b
        minorBranchPath = refs[str(sorted(minorVersionsNumbers,key=int)[-1])]
    return minorBranchPath



def set_psdk_pkgs_to_install(psdkJson, psdkPkgsToInstall):
	for service in installReferenceDict:
		if service in psdkJson:
			if "included" in psdkJson[service]:
				if psdkJson[service]["included"]:
					psdkPkgsToInstall[service] = True;


def read_json_file(json_file_path):
	jsonData=None
	try:
		jsonTxt=None
		with open(json_file_path,'r') as f:
			jsonTxt=f.read().replace('\n', '')
		if jsonTxt:
			#print("---------------------------- \n{}\n --------------------------\n".format(jsonTxt))
			jsonData=json.loads(jsonTxt)
		f.close()
	except:
		pass

	return jsonData



if __name__ == "__main__":
   exit(main(sys.argv[1:]))

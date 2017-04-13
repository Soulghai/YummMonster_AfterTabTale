#!/usr/bin/python

import os, sys, getopt
import glob

def main(argv):
	currentScriptDirectory=os.path.dirname(os.path.abspath(__file__))
	unityProjectDirectory=os.path.join(currentScriptDirectory,'..','..')
	delete_psdk(unityProjectDirectory)
	
def delete_psdk(unityProjectDirectory):
	for files_to_delete_file in glob.glob(unityProjectDirectory + "/Assets/TabTale/UnityPackages/PSDK/services/*/FileSet/*.files.txt") + glob.glob(os.path.abspath(__file__) + "/../../Assets/PSDK/FileSet/*.files.txt"):
		try:
			with open(files_to_delete_file,'r') as ftd:
				for fileToDelete in ftd:
					try:
						print "rm ", os.path.join(unityProjectDirectory , fileToDelete.replace('\n', ''))
						os.remove(os.path.join(unityProjectDirectory , fileToDelete.replace('\n', '')))
					except:
						pass
				ftd.close()
				try:
					print "rm ",files_to_delete_file
					os.remove(files_to_delete_file)
				except:
					pass
		except:
			pass
	for fileToDelete in glob.glob(os.path.join(unityProjectDirectory , "Assets/TabTale/UnityPackages/PSDK/services/*/Editor/iOS/*framework")):
		try:
			print "rmdir ",fileToDelete
			os.rmdir(fileToDelete)
		except:
			pass

if __name__ == "__main__":
   exit(main(sys.argv[1:]))

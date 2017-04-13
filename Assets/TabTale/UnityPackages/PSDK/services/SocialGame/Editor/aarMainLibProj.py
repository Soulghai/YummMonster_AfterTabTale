#!/usr/bin/env python
import os
import zipfile

def zipdir(path, ziph):
    # ziph is zipfile handle
    for root, dirs, files in os.walk(path):
        for file in files:
	    if file.endswith('.meta'):
	        continue
            if file.endswith('.py'):
                continue;
            if file.endswith('.jar_'):
		oldFile=file
                file = file.replace('.jar_','.jar')
                os.rename(os.path.join(root,oldFile),os.path.join(root,file))
            if os.path.isdir(file):
                zipdir(os.path.join(path,file),ziph)
            else:
                ziph.write(os.path.join(root, file))
            if file.endswith('.jar'):
		oldFile=file
                file = file.replace('.jar','.jar_')
		os.rename(os.path.join(root,oldFile),os.path.join(root,file))



if __name__ == '__main__':
    zipf = zipfile.ZipFile('../MainLibProj.aar', 'w')
    zipdir('./', zipf)
    zipf.close()

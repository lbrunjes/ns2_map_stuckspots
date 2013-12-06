# a python script to genrate all maps up to current version
# 
#
from subprocess import call

startBuild =181;
currentBuild = 262
maps =["ns2_biodome","ns2_tram"]
useMono = 1

for mapname in maps:
	for build in range(startBuild, currentBuild):
		if useMono :
			call(["mono","StuckSpotMapper.exe", mapname, str(build)]);
		else:
			call(["StuckSpotMapper.exe", mapname, str(build)]);



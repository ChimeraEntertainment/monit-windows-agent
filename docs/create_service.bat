@echo off

sc create MonitWindowsAgent start= auto binPath= %~dp0MonitWindowsAgent.exe DisplayName= "Monit Windows Agent"

sc start MonitWindowsAgent
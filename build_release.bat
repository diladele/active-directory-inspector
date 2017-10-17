@echo off

set MSBUILD="c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

:: first clean
call clean.bat

:: set variables
call env.bat

:: decend to tools folder
pushd tools\builder

:: build the project one by one
%MSBUILD% build.xml    /p:Configuration=Release
%MSBUILD% sign.xml     /p:Configuration=Release
:: %MSBUILD% pack.xml     /p:Configuration=Release
:: %MSBUILD% sign_msi.xml /p:Configuration=Release

:: get back
popd

:: and pause
pause

@echo off

set msbuild="c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

:: first clean
call clean.bat

:: decend to tools folder
pushd tools\builder

:: build the project one by one
%msbuild% build.xml    /p:Configuration=Debug
:: %msbuild% sign.xml     /p:Configuration=Debug
:: %msbuild% pack.xml     /p:Configuration=Debug
:: %msbuild% sign_msi.xml /p:Configuration=Debug

:: get back
popd

:: and pause
pause

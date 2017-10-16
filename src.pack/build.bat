@echo off
rem
rem build_msi.bat debug|release <version as x.x.x.x> <git revision> <project> <path like m:\aachen>
rem

rem
rem configuration section
rem
set PLATFORM=x64
set CONFIGURATION=%1
set VERSION=%2
set REVISION=%4
set PROJECT=%5
set BASEDIR=m:\active-directory-inspector
set CANDLE="c:\Program Files (x86)\WiX Toolset v3.9\bin\candle.exe"
set LIGHT="c:\Program Files (x86)\WiX Toolset v3.9\bin\light.exe"
set WIX_COMPRESSION_LEVEL=none
set BUILDDIR=%BASEDIR%\ztmp\%PLATFORM%\pack\%CONFIGURATION%
set BINDIR=%BASEDIR%\src\Diladele.ActiveDirectory.Service\bin\%CONFIGURATION%
set BINDIRANYCPU=%BASEDIR%\bin\AnyCPU\%CONFIGURATION%
IF "%PLATFORM%" == "Win32" ( 
    set DIFZLIB=%BASEDIR%\contrib\wix36\difxapp_x86.wixlib 
) ELSE (
    set DIFZLIB=%BASEDIR%\contrib\wix36\difxapp_x64.wixlib
)
set MSI=%PROJECT%.msi

rem
rem code section
rem
echo %BUILDDIR%
del /F /S /Q %BUILDDIR%\ > nul


::copy %BINDIR%\custom_actions.* %BUILDDIR%\ > nul
echo %BINDIR%
copy %BINDIR%\Diladele.ActiveDirectory.Inspection.dll %BUILDDIR%\ > nul
copy %BINDIR%\Diladele.ActiveDirectory.Inspection.pdb %BUILDDIR%\ > nul
copy %BINDIR%\Diladele.ActiveDirectory.Service.exe %BUILDDIR%\ > nul
copy %BINDIR%\Diladele.ActiveDirectory.Service.pdb %BUILDDIR%\ > nul
copy %BINDIR%\log4net.dll %BUILDDIR%\ > nul

::copy %BINDIR%\ddwsupd.* %BUILDDIR%\ > nul
::copy %BINDIR%\ddwsc.* %BUILDDIR%\ > nul
::copy %BINDIR%\ddwscadm.* %BUILDDIR%\ > nul
::copy %BINDIR%\ddws_test.* %BUILDDIR%\ > nul
::copy %BINDIR%\events.* %BUILDDIR%\ > nul
::copy %BINDIR%\interface.* %BUILDDIR%\ > nul
::copy %BINDIR%\Diladele.WebSafety.Interop.* %BUILDDIR%\ > nul
::copy %BINDIRANYCPU%\*.* %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BINDIR%\drivers %BUILDDIR%\drivers > nul
::copy %BINDIR%\SQLite.Interop.* %BUILDDIR%\ > nul
::copy %BASEDIR%\src\libs\kernel_daemon\perf_counters.man %BUILDDIR%\ > nul
::copy %BASEDIR%\src\libs\history_events\tracing.man %BUILDDIR%\ > nul
::copy %BASEDIR%\src\pack\transparent.gif %BUILDDIR%\ > nul
::copy %BASEDIR%\src\pack\block*.html %BUILDDIR%\ > nul
::copy %BASEDIR%\src\pack\wirunsql.vbs %BUILDDIR%\ > nul
::
::mkdir %BUILDDIR%\res > nul
::xcopy /Q /Y /E %BASEDIR%\src\res %BUILDDIR%\res > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.wxi %BUILDDIR%\ > nul
xcopy /Q /Y /E %BASEDIR%\src.pack1\*.wxs %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.wxl %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.bmp %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.ico %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.rtf %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.exe %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.vbs %BUILDDIR%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.cmd %BUILDDIR%\ > nul

pushd .
cd %BUILDDIR%

echo
echo Building English MSI in %BUILDDIR%...
echo
%CANDLE% /nologo /pedantic /ext WiXUtilExtension /ext WixUIExtension ^
    /dOUTDIR=%BUILDDIR% /dBUILDTYPE=%CONFIGURATION% /dPLATFORM=x64 /dVERSION=%VERSION% /dREVISION=%REVISION% /out %BUILDDIR%\ /dcodepage=1252 *.wxs
%LIGHT% /pedantic /nologo /ext WiXUtilExtension /ext WixUIExtension /cultures:en-us /out %MSI%  %BUILDDIR%\*.wixobj

::xcopy /Q /Y /E %BUILDDIR%\%MSI% %BINDIR%\%VERSION%_%REVISION%\

popd

pause
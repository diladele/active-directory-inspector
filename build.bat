@echo off
rem
rem build_msi.bat debug|release <version as x.x.x.x> <git revision> <path like m:\aachen>
rem

rem
rem configuration section
rem
set PLATFORM=x64
set CONFIGURATION=%1
set VERSION=0.0.0.1
set REVISION=%4

:: folders
set DIR_TOP=m:\active-directory-inspector
set DIR_BIN=%DIR_TOP%\src\Diladele.ActiveDirectory.Service\bin\%CONFIGURATION%
set DIR_TMP=%DIR_TOP%\ztmp\x64\pack\%CONFIGURATION%

:: remove build dir
del /F /S /Q %DIR_TMP%\ > nul

:: recreate it
mkdir %DIR_TMP% > nul

:: copy all binaries
copy %DIR_BIN%\Diladele.ActiveDirectory.Inspection.dll %DIR_TMP%\ > nul
copy %DIR_BIN%\Diladele.ActiveDirectory.Inspection.pdb %DIR_TMP%\ > nul
copy %DIR_BIN%\Diladele.ActiveDirectory.Server.dll     %DIR_TMP%\ > nul
copy %DIR_BIN%\Diladele.ActiveDirectory.Server.pdb     %DIR_TMP%\ > nul
copy %DIR_BIN%\Diladele.ActiveDirectory.Service.exe    %DIR_TMP%\ > nul
copy %DIR_BIN%\Diladele.ActiveDirectory.Service.pdb    %DIR_TMP%\ > nul
copy %DIR_BIN%\log4net.dll                             %DIR_TMP%\ > nul

::copy %BINDIR%\ddwsupd.* %DIR_TMP%\ > nul
::copy %BINDIR%\ddwsc.* %DIR_TMP%\ > nul
::copy %BINDIR%\ddwscadm.* %DIR_TMP%\ > nul
::copy %BINDIR%\ddws_test.* %DIR_TMP%\ > nul
::copy %BINDIR%\events.* %DIR_TMP%\ > nul
::copy %BINDIR%\interface.* %DIR_TMP%\ > nul
::copy %BINDIR%\Diladele.WebSafety.Interop.* %DIR_TMP%\ > nul
::xcopy /Q /Y /E %BINDIR%\drivers %DIR_TMP%\drivers > nul
::copy %BINDIR%\SQLite.Interop.* %DIR_TMP%\ > nul
::copy %BASEDIR%\src\libs\kernel_daemon\perf_counters.man %DIR_TMP%\ > nul
::copy %BASEDIR%\src\libs\history_events\tracing.man %DIR_TMP%\ > nul
::copy %BASEDIR%\src\pack\transparent.gif %DIR_TMP%\ > nul
::copy %BASEDIR%\src\pack\block*.html %DIR_TMP%\ > nul
::copy %BASEDIR%\src\pack\wirunsql.vbs %DIR_TMP%\ > nul
::
::mkdir %DIR_TMP%\res > nul
::xcopy /Q /Y /E %BASEDIR%\src\res %DIR_TMP%\res > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.wxi %DIR_TMP%\ > nul

:: copy installer files
xcopy /Q /Y /E %DIR_TOP%\src.pack\*.wxs %DIR_TMP%\ > nul
xcopy /Q /Y /E %DIR_TOP%\src.pack\*.bmp %DIR_TMP%\ > nul
xcopy /Q /Y /E %DIR_TOP%\src.pack\*.ico %DIR_TMP%\ > nul
xcopy /Q /Y /E %DIR_TOP%\src.pack\*.rtf %DIR_TMP%\ > nul

::xcopy /Q /Y /E %BASEDIR%\src\pack\*.wxl %DIR_TMP%\ > nul

::xcopy /Q /Y /E %BASEDIR%\src\pack\*.exe %DIR_TMP%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.vbs %DIR_TMP%\ > nul
::xcopy /Q /Y /E %BASEDIR%\src\pack\*.cmd %DIR_TMP%\ > nul

:: change into the temp folder
pushd . && cd %DIR_TMP%

:: wix executables
set CANDLE="c:\Program Files (x86)\WiX Toolset v3.9\bin\candle.exe"
set LIGHT="c:\Program Files (x86)\WiX Toolset v3.9\bin\light.exe"

:: and build the installer
%CANDLE% /nologo /pedantic ^
	/ext WiXUtilExtension /ext WixUIExtension ^
    /dOUTDIR=%DIR_TMP% ^
    /dBUILDTYPE=%CONFIGURATION% ^
    /dPLATFORM=x64 ^
    /dVERSION=%VERSION% ^
    /dREVISION=%REVISION% ^
    /out %DIR_TMP%\ ^
    /dcodepage=1252 *.wxs

%LIGHT% /pedantic /nologo ^
	/ext WiXUtilExtension ^
	/ext WixUIExtension ^
	/cultures:en-us ^
	/out active-directory-inspector.msi  ^
	%DIR_TMP%\*.wixobj

::xcopy /Q /Y /E %DIR_TMP%\%MSI% %BINDIR%\%VERSION%_%REVISION%\

popd

pause
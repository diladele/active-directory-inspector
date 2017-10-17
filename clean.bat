:: remove bin and ztmp folders
rmdir /S /Q .\bin
rmdir /S /Q .\ztmp

:: remove all output of .net projects
rmdir /S /Q .\src\Diladele.ActiveDirectory.Inspection\bin
rmdir /S /Q .\src\Diladele.ActiveDirectory.Inspection\obj
rmdir /S /Q .\src\Diladele.ActiveDirectory.Server\bin
rmdir /S /Q .\src\Diladele.ActiveDirectory.Server\obj
rmdir /S /Q .\src\Diladele.ActiveDirectory.Service\bin
rmdir /S /Q .\src\Diladele.ActiveDirectory.Service\obj

:: remove all output of .net test projects
rmdir /S /Q .\src.test\Diladele.ActiveDirectory.Inspection.Test\bin
rmdir /S /Q .\src.test\Diladele.ActiveDirectory.Inspection.Test\obj
rmdir /S /Q .\src.test\Diladele.ActiveDirectory.Inspection.UI\bin
rmdir /S /Q .\src.test\Diladele.ActiveDirectory.Inspection.UI\obj
REM :: This would be much easier with a solution copy of mstest !
SET VISUAL_STUDIO_PATH=c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\
call "%VISUAL_STUDIO_PATH%\vsvars32.bat"

SET PATH=%PATH%;.\packages\OpenCover.4.5.3427\;.\packages\ReportGenerator.2.0.2.0\

del .\reports /Q

OpenCover.Console.Exe -register:user -target:mstest.exe -targetargs:"/testcontainer:.\JenkinsTransport.UnitTests\bin\Debug\JenkinsTransport.UnitTests.dll" -output:tests.xml -filter:"+[Jenkins*]*" -coverbytest:"*Test*"

ReportGenerator.exe "-reports:tests.xml" "-targetdir:.\reports"
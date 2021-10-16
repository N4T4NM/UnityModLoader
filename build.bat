@echo off
set msdir="D:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin"

echo Compiling UnityModLoader.Library...
%msdir%\msbuild.exe /p:Configuration=Release UnityModLoader.Library\UnityModLoader.Library.csproj >> build.log

echo Compiling UnityModLoader.Manager...
%msdir%\msbuild.exe /p:Configuration=Release UnityModLoader.Manager\UnityModLoader.Manager.csproj >> build.log

echo Copying files...
mkdir UnityModLoader
copy UnityModLoader.Library\bin\Release\UnityModLoader.Library.dll .\UnityModLoader
copy UnityModLoader.Manager\bin\Release\UnityModLoader.Manager.exe .\UnityModLoader
copy Release\UnityModLoader.Injection.dll .\UnityModLoader

echo Packing...
powershell "Compress-Archive UnityModLoader UnityModLoader.zip"

echo Cleaning...
rmdir /S /Q UnityModLoader
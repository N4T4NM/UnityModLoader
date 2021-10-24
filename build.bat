@echo off
set msdir="D:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin"

echo == x86 == > build.log

echo ==== Platform x86 ====
echo Compiling UnityModLoader.Library x86...
%msdir%\msbuild.exe /p:Configuration=Release /p:Platform=x86 /p:OutputPath="./x86" UnityModLoader.Library\UnityModLoader.Library.csproj >> build.log

echo Compiling UnityModLoader.Manager x86...
%msdir%\msbuild.exe /p:Configuration=Release /p:Platform=x86 /p:OutputPath="./x86" UnityModLoader.Manager\UnityModLoader.Manager.csproj >> build.log

echo Compiling UnityModLoader.Injection x86...
%msdir%\msbuild.exe /p:Configuration=Release /p:Platform=x86 UnityModLoader.Injection\UnityModLoader.Injection.vcxproj >> build.log

echo Copying files...
mkdir UnityModLoader_x86
copy UnityModLoader.Library\x86\UnityModLoader.Library.dll .\UnityModLoader_x86
copy UnityModLoader.Manager\x86\UnityModLoader.Manager.exe .\UnityModLoader_x86
copy UnityModLoader.Injection\Release\UnityModLoader.Injection.dll .\UnityModLoader_x86

echo Packing...
powershell "Compress-Archive UnityModLoader_x86 UnityModLoader_x86.zip"

echo Cleaning...
rmdir /S /Q UnityModLoader.Library\x86
rmdir /S /Q UnityModLoader.Manager\x86
del /Q /F UnityModLoader.Injection\Release\UnityModLoader.Injection.dll
rmdir /S /Q UnityModLoader_x86

echo == x64 == >> build.log
echo  

echo ==== Platform x64 ====
echo Compiling UnityModLoader.Library x64...
%msdir%\msbuild.exe /p:Configuration=Release;Prefer32bit=false;Platform=x64;OutputPath="./x64" UnityModLoader.Library\UnityModLoader.Library.csproj >> build.log

echo Compiling UnityModLoader.Manager x64...
%msdir%\msbuild.exe /p:Configuration=Release;Prefer32bit=false;Platform=x64;OutputPath="./x64" UnityModLoader.Manager\UnityModLoader.Manager.csproj >> build.log

echo Compiling UnityModLoader.Injection x64...
%msdir%\msbuild.exe /p:Configuration=Release;Prefer32bit=false;Platform=x64 UnityModLoader.Injection\UnityModLoader.Injection.vcxproj >> build.log

echo Copying files...
mkdir UnityModLoader_x64
copy UnityModLoader.Library\x64\UnityModLoader.Library.dll .\UnityModLoader_x64
copy UnityModLoader.Manager\x64\UnityModLoader.Manager.exe .\UnityModLoader_x64
copy UnityModLoader.Injection\x64\Release\UnityModLoader.Injection.dll .\UnityModLoader_x64

echo Packing...
powershell "Compress-Archive UnityModLoader_x64 UnityModLoader_x64.zip"

echo Cleaning...
rmdir /S /Q UnityModLoader.Library\x64
rmdir /S /Q UnityModLoader.Manager\x64
rmdir /S /Q UnityModLoader_x64
@echo off
cd /d %~dp0
for %%a in ("%~dp0\.") do set "parent=%%~nxa"
set UnityProjectName=Unity%parent%

title Setup Script
echo Setting up pipeline!!
echo This script will:
echo 1. Change unity project name to %UnityProjectName%
echo 2. Re add submodule
pause
setlocal
@REM Remove old submodule
git rm UnityProjectTemplate\Packages\com.gdk.core\
git rm UnityProjectTemplate\Packages\com.3rd.core\
git rm UnityProjectTemplate\Assets\FeatureTemplate\
@REM Rename Unity Project
ren UnityProjectTemplate %UnityProjectName%
git remote add template git@github.com:NotthingStudioo/UnityProjectTemplate.git
@REM RE-Add submodule
git submodule add -b haihoang/native git@github.com:GameDevelopmentKit/GameFoundation.git %UnityProjectName%/Packages/com.gdk.core
git submodule add git@github.com:NotthingStudioo/FeatureTemplate.git %UnityProjectName%/Assets/FeatureTemplate
git submodule add -b haihoang/native git@github.com:GameDevelopmentKit/ThirdPartyServices.git %UnityProjectName%/Packages/com.3rd.core
pause
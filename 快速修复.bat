@echo off
chcp 65001 >nul
cd /d "H:\JX1_Alone Oline\JX1_Thai_Github\autotrainvlbs_source自动挂机源码\source"

echo ========================================
echo 快速修复 GitHub 仓库
echo ========================================
echo.

echo [1] 初始化 Git 仓库...
if not exist ".git" (
    git init
    echo ✓ Git 仓库已初始化
) else (
    echo ✓ Git 仓库已存在
)
echo.

echo [2] 添加所有文件...
git add .
echo ✓ 文件已添加
echo.

echo [3] 创建提交...
git commit -m "Initial commit: JX1Tool-Hanhua" 2>nul
if %errorlevel% neq 0 (
    git commit -m "更新: 添加所有文件" 2>nul
)
echo ✓ 提交已创建
echo.

echo [4] 设置分支为 main...
git branch -M main 2>nul
echo ✓ 分支已设置
echo.

echo [5] 配置远程仓库...
git remote remove origin 2>nul
git remote add origin https://maiyadiu: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@github.com/maiyadiu/JX1Tool-Hanhua.git
echo ✓ 远程仓库已配置
echo.

echo [6] 推送到 GitHub...
git push -u origin main --force
echo.

echo ========================================
echo 完成！
echo ========================================
echo.
echo 请访问: https://github.com/maiyadiu/JX1Tool-Hanhua
echo.
pause

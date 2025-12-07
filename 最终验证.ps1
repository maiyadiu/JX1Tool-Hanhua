# 最终验证脚本
$sourceDir = "H:\JX1_Alone Oline\JX1_Thai_Github\autotrainvlbs_source自动挂机源码\source"
Set-Location $sourceDir

$token = "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"
$username = "maiyadiu"
$repoName = "JX1Tool-Hanhua"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub 仓库最终验证" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 验证仓库是否存在
Write-Host "[1] 检查 GitHub 仓库..." -ForegroundColor Yellow
try {
    $repo = Invoke-RestMethod -Uri "https://api.github.com/repos/$username/$repoName" -Headers @{Authorization = "token $token"}
    Write-Host "  ✓ 仓库存在" -ForegroundColor Green
    Write-Host "  名称: $($repo.name)" -ForegroundColor Cyan
    Write-Host "  地址: $($repo.html_url)" -ForegroundColor Cyan
    Write-Host "  描述: $($repo.description)" -ForegroundColor Cyan
    Write-Host "  创建时间: $($repo.created_at)" -ForegroundColor Cyan
    Write-Host "  可见性: $(if($repo.private){'私有'}else{'公开'})" -ForegroundColor Cyan
} catch {
    Write-Host "  ✗ 仓库不存在或无法访问" -ForegroundColor Red
    Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "  正在创建仓库..." -ForegroundColor Yellow
    $headers = @{
        Authorization = "token $token"
        "Content-Type" = "application/json"
    }
    $body = @{
        name = $repoName
        description = "JX1 Tool 汉化工具"
        private = $false
    } | ConvertTo-Json
    
    try {
        $newRepo = Invoke-RestMethod -Uri "https://api.github.com/user/repos" -Method Post -Headers $headers -Body $body
        Write-Host "  ✓ 仓库创建成功: $($newRepo.html_url)" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ 创建失败: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# 2. 检查本地 Git 配置
Write-Host "[2] 检查本地 Git 配置..." -ForegroundColor Yellow
$remoteUrl = git remote get-url origin 2>$null
if ($remoteUrl) {
    Write-Host "  ✓ 远程仓库已配置" -ForegroundColor Green
    Write-Host "  URL: $remoteUrl" -ForegroundColor Cyan
} else {
    Write-Host "  ⚠ 远程仓库未配置" -ForegroundColor Yellow
    Write-Host "  正在配置..." -ForegroundColor Cyan
    $newRemoteUrl = "https://$username`:$token@github.com/$username/$repoName.git"
    git remote add origin $newRemoteUrl 2>&1 | Out-Null
    Write-Host "  ✓ 远程仓库已配置" -ForegroundColor Green
}
Write-Host ""

# 3. 检查本地提交
Write-Host "[3] 检查本地提交..." -ForegroundColor Yellow
$commits = git log --oneline 2>$null
if ($commits) {
    $commitCount = ($commits | Measure-Object -Line).Lines
    Write-Host "  ✓ 找到 $commitCount 个提交" -ForegroundColor Green
    Write-Host "  最近3个提交:" -ForegroundColor Cyan
    git log --oneline -3 | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
} else {
    Write-Host "  ⚠ 没有提交，正在创建..." -ForegroundColor Yellow
    git add . 2>&1 | Out-Null
    git commit -m "Initial commit: JX1Tool-Hanhua 项目" 2>&1 | Out-Null
    Write-Host "  ✓ 初始提交已创建" -ForegroundColor Green
}
Write-Host ""

# 4. 检查分支
Write-Host "[4] 检查分支..." -ForegroundColor Yellow
$branch = git branch --show-current
if ($branch) {
    Write-Host "  ✓ 当前分支: $branch" -ForegroundColor Green
} else {
    Write-Host "  ⚠ 没有分支，创建 main 分支..." -ForegroundColor Yellow
    git branch -M main 2>&1 | Out-Null
    $branch = "main"
    Write-Host "  ✓ 已创建 main 分支" -ForegroundColor Green
}
Write-Host ""

# 5. 推送到远程
Write-Host "[5] 推送到远程仓库..." -ForegroundColor Yellow
Write-Host "  正在推送..." -ForegroundColor Cyan
$pushResult = git push -u origin $branch 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ 推送成功！" -ForegroundColor Green
} else {
    Write-Host "  ⚠ 普通推送失败，尝试强制推送..." -ForegroundColor Yellow
    $forcePush = git push -u origin $branch --force 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ 强制推送成功！" -ForegroundColor Green
    } else {
        Write-Host "  ✗ 推送失败:" -ForegroundColor Red
        $forcePush | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
    }
}
Write-Host ""

# 6. 最终状态
Write-Host "[6] 最终状态检查..." -ForegroundColor Yellow
$status = git status -sb
Write-Host "  $status" -ForegroundColor Cyan
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "验证完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "仓库地址: https://github.com/$username/$repoName" -ForegroundColor Cyan
Write-Host ""
Write-Host "请访问上面的链接确认仓库是否已创建并包含代码。" -ForegroundColor Yellow

# 诊断并修复 GitHub 仓库问题
$ErrorActionPreference = "Continue"
$sourceDir = "H:\JX1_Alone Oline\JX1_Thai_Github\autotrainvlbs_source自动挂机源码\source"
Set-Location $sourceDir

$token = "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"
$username = "maiyadiu"
$repoName = "JX1Tool-Hanhua"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "诊断并修复 GitHub 仓库" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 步骤1: 检查 Git 仓库
Write-Host "[步骤 1] 检查本地 Git 仓库..." -ForegroundColor Yellow
if (Test-Path ".git") {
    Write-Host "  ✓ Git 仓库存在" -ForegroundColor Green
} else {
    Write-Host "  ✗ Git 仓库不存在，正在初始化..." -ForegroundColor Red
    git init
    Write-Host "  ✓ Git 仓库已初始化" -ForegroundColor Green
}
Write-Host ""

# 步骤2: 检查并创建 GitHub 仓库
Write-Host "[步骤 2] 检查 GitHub 仓库..." -ForegroundColor Yellow
$headers = @{Authorization = "token $token"}
try {
    $repo = Invoke-RestMethod -Uri "https://api.github.com/repos/$username/$repoName" -Headers $headers
    Write-Host "  ✓ 仓库已存在" -ForegroundColor Green
    Write-Host "    地址: $($repo.html_url)" -ForegroundColor Cyan
    Write-Host "    默认分支: $($repo.default_branch)" -ForegroundColor Cyan
    Write-Host "    是否为空: $(if($repo.size -eq 0){'是'}else{'否'} - $($repo.size) KB)" -ForegroundColor Cyan
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 404) {
        Write-Host "  ✗ 仓库不存在，正在创建..." -ForegroundColor Red
        $createHeaders = @{
            Authorization = "token $token"
            "Content-Type" = "application/json"
        }
        $body = @{
            name = $repoName
            description = "JX1 Tool 汉化工具"
            private = $false
            auto_init = $false
        } | ConvertTo-Json
        
        try {
            $newRepo = Invoke-RestMethod -Uri "https://api.github.com/user/repos" -Method Post -Headers $createHeaders -Body $body
            Write-Host "  ✓ 仓库创建成功: $($newRepo.html_url)" -ForegroundColor Green
        } catch {
            Write-Host "  ✗ 创建失败: $($_.Exception.Message)" -ForegroundColor Red
            if ($_.Exception.Response) {
                $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                $responseBody = $reader.ReadToEnd()
                Write-Host "  响应: $responseBody" -ForegroundColor Red
            }
            exit 1
        }
    } else {
        Write-Host "  ✗ 检查失败: $($_.Exception.Message) (状态码: $statusCode)" -ForegroundColor Red
        exit 1
    }
}
Write-Host ""

# 步骤3: 配置远程仓库
Write-Host "[步骤 3] 配置远程仓库..." -ForegroundColor Yellow
$remoteUrl = git remote get-url origin 2>$null
if ($remoteUrl) {
    Write-Host "  ✓ 远程仓库已配置: $remoteUrl" -ForegroundColor Green
    # 更新为包含 token 的 URL
    $newRemoteUrl = "https://$username`:$token@github.com/$username/$repoName.git"
    git remote set-url origin $newRemoteUrl
    Write-Host "  ✓ 已更新远程 URL（包含认证信息）" -ForegroundColor Green
} else {
    Write-Host "  ✗ 远程仓库未配置，正在添加..." -ForegroundColor Red
    $newRemoteUrl = "https://$username`:$token@github.com/$username/$repoName.git"
    git remote add origin $newRemoteUrl
    Write-Host "  ✓ 远程仓库已添加" -ForegroundColor Green
}
Write-Host ""

# 步骤4: 添加所有文件
Write-Host "[步骤 4] 添加文件到 Git..." -ForegroundColor Yellow
git add . 2>&1 | Out-Null
$staged = git diff --cached --name-only
if ($staged) {
    $fileCount = ($staged | Measure-Object -Line).Lines
    Write-Host "  ✓ 已添加 $fileCount 个文件到暂存区" -ForegroundColor Green
} else {
    Write-Host "  ⚠ 没有新文件需要添加" -ForegroundColor Yellow
}
Write-Host ""

# 步骤5: 创建提交
Write-Host "[步骤 5] 创建提交..." -ForegroundColor Yellow
$hasCommits = git log --oneline 2>$null
if (-not $hasCommits) {
    Write-Host "  创建初始提交..." -ForegroundColor Cyan
    git commit -m "Initial commit: JX1Tool-Hanhua 项目" 2>&1 | Out-Null
    Write-Host "  ✓ 初始提交已创建" -ForegroundColor Green
} else {
    $status = git status --porcelain
    if ($status) {
        Write-Host "  提交更改..." -ForegroundColor Cyan
        git commit -m "更新: 添加所有项目文件" 2>&1 | Out-Null
        Write-Host "  ✓ 更改已提交" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ 没有更改需要提交" -ForegroundColor Yellow
    }
}

$commitCount = (git log --oneline 2>$null | Measure-Object -Line).Lines
Write-Host "  总提交数: $commitCount" -ForegroundColor Cyan
Write-Host ""

# 步骤6: 确保分支名称正确
Write-Host "[步骤 6] 检查分支..." -ForegroundColor Yellow
$branch = git branch --show-current
if (-not $branch) {
    Write-Host "  创建 main 分支..." -ForegroundColor Cyan
    git checkout -b main 2>&1 | Out-Null
    $branch = "main"
} else {
    if ($branch -ne "main") {
        Write-Host "  重命名分支为 main..." -ForegroundColor Cyan
        git branch -M main 2>&1 | Out-Null
        $branch = "main"
    }
}
Write-Host "  ✓ 当前分支: $branch" -ForegroundColor Green
Write-Host ""

# 步骤7: 推送到远程
Write-Host "[步骤 7] 推送到远程仓库..." -ForegroundColor Yellow
Write-Host "  正在推送 $branch 分支到 origin..." -ForegroundColor Cyan

# 先尝试普通推送
$pushOutput = git push -u origin $branch 2>&1
$pushSuccess = $LASTEXITCODE -eq 0

if (-not $pushSuccess) {
    Write-Host "  ⚠ 普通推送失败，尝试强制推送..." -ForegroundColor Yellow
    $pushOutput = git push -u origin $branch --force 2>&1
    $pushSuccess = $LASTEXITCODE -eq 0
}

if ($pushSuccess) {
    Write-Host "  ✓ 推送成功！" -ForegroundColor Green
} else {
    Write-Host "  ✗ 推送失败，错误信息:" -ForegroundColor Red
    $pushOutput | ForEach-Object { 
        if ($_ -match "error|fatal|denied|permission") {
            Write-Host "    $_" -ForegroundColor Red
        } else {
            Write-Host "    $_" -ForegroundColor Yellow
        }
    }
    Write-Host ""
    Write-Host "  可能的原因:" -ForegroundColor Yellow
    Write-Host "  1. Token 权限不足（需要 repo 权限）" -ForegroundColor Yellow
    Write-Host "  2. 网络连接问题" -ForegroundColor Yellow
    Write-Host "  3. 仓库访问限制" -ForegroundColor Yellow
}
Write-Host ""

# 步骤8: 验证最终状态
Write-Host "[步骤 8] 验证最终状态..." -ForegroundColor Yellow
$finalStatus = git status -sb
Write-Host "  $finalStatus" -ForegroundColor Cyan

$remoteCheck = git remote get-url origin 2>$null
if ($remoteCheck) {
    # 隐藏 token 显示
    $displayUrl = $remoteCheck -replace "://.*@", "://***@"
    Write-Host "  远程 URL: $displayUrl" -ForegroundColor Cyan
}

Write-Host ""

# 最终总结
Write-Host "========================================" -ForegroundColor Cyan
if ($pushSuccess) {
    Write-Host "✓ 完成！代码已推送到 GitHub" -ForegroundColor Green
} else {
    Write-Host "⚠ 部分完成，但推送失败" -ForegroundColor Yellow
}
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "仓库地址: https://github.com/$username/$repoName" -ForegroundColor Cyan
Write-Host ""
Write-Host "请访问上面的链接查看仓库。" -ForegroundColor Yellow
if (-not $pushSuccess) {
    Write-Host ""
    Write-Host "如果推送失败，请检查:" -ForegroundColor Yellow
    Write-Host "1. Token 是否有 'repo' 权限" -ForegroundColor Yellow
    Write-Host "2. 网络连接是否正常" -ForegroundColor Yellow
    Write-Host "3. 手动执行: git push -u origin main" -ForegroundColor Yellow
}

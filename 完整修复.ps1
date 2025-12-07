# 完整修复脚本 - 输出到文件
$sourceDir = "H:\JX1_Alone Oline\JX1_Thai_Github\autotrainvlbs_source自动挂机源码\source"
$logFile = "$sourceDir\修复日志.txt"
Set-Location $sourceDir

$token = "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"
$username = "maiyadiu"
$repoName = "JX1Tool-Hanhua"

function Write-Log {
    param($Message, $Color = "White")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] $Message"
    Write-Host $logMessage -ForegroundColor $Color
    Add-Content -Path $logFile -Value $logMessage
}

# 清空日志文件
"" | Out-File $logFile

Write-Log "========================================" "Cyan"
Write-Log "开始修复 GitHub 仓库" "Cyan"
Write-Log "========================================" "Cyan"
Write-Log ""

# 1. 初始化 Git
Write-Log "[1] 初始化 Git 仓库..." "Yellow"
if (-not (Test-Path ".git")) {
    git init | Out-Null
    Write-Log "  ✓ Git 仓库已初始化" "Green"
} else {
    Write-Log "  ✓ Git 仓库已存在" "Green"
}

# 2. 添加文件
Write-Log "[2] 添加文件..." "Yellow"
git add . 2>&1 | Out-Null
Write-Log "  ✓ 文件已添加到暂存区" "Green"

# 3. 创建提交
Write-Log "[3] 创建提交..." "Yellow"
$hasCommits = git log --oneline 2>$null
if (-not $hasCommits) {
    $commitResult = git commit -m "Initial commit: JX1Tool-Hanhua" 2>&1
    Write-Log "  ✓ 初始提交已创建" "Green"
} else {
    $status = git status --porcelain
    if ($status) {
        $commitResult = git commit -m "更新: 添加所有文件" 2>&1
        Write-Log "  ✓ 更新提交已创建" "Green"
    } else {
        Write-Log "  ⚠ 没有更改需要提交" "Yellow"
    }
}

# 4. 设置分支
Write-Log "[4] 设置分支为 main..." "Yellow"
git branch -M main 2>&1 | Out-Null
Write-Log "  ✓ 分支已设置为 main" "Green"

# 5. 配置远程
Write-Log "[5] 配置远程仓库..." "Yellow"
git remote remove origin 2>$null
$remoteUrl = "https://$username`:$token@github.com/$username/$repoName.git"
git remote add origin $remoteUrl
Write-Log "  ✓ 远程仓库已配置" "Green"
Write-Log "  URL: https://github.com/$username/$repoName.git" "Cyan"

# 6. 创建 GitHub 仓库（如果不存在）
Write-Log "[6] 检查 GitHub 仓库..." "Yellow"
$headers = @{Authorization = "token $token"}
try {
    $repo = Invoke-RestMethod -Uri "https://api.github.com/repos/$username/$repoName" -Headers $headers
    Write-Log "  ✓ 仓库已存在" "Green"
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 404) {
        Write-Log "  仓库不存在，正在创建..." "Yellow"
        $createHeaders = @{
            Authorization = "token $token"
            "Content-Type" = "application/json"
        }
        $body = @{
            name = $repoName
            description = "JX1 Tool 汉化工具"
            private = $false
        } | ConvertTo-Json
        
        try {
            $newRepo = Invoke-RestMethod -Uri "https://api.github.com/user/repos" -Method Post -Headers $createHeaders -Body $body
            Write-Log "  ✓ 仓库创建成功: $($newRepo.html_url)" "Green"
        } catch {
            Write-Log "  ✗ 创建失败: $($_.Exception.Message)" "Red"
        }
    }
}

# 7. 推送
Write-Log "[7] 推送到 GitHub..." "Yellow"
$pushOutput = git push -u origin main --force 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Log "  ✓ 推送成功！" "Green"
} else {
    Write-Log "  ✗ 推送失败" "Red"
    $pushOutput | ForEach-Object { Write-Log "    $_" "Red" }
}

Write-Log ""
Write-Log "========================================" "Cyan"
Write-Log "完成！" "Green"
Write-Log "========================================" "Cyan"
Write-Log ""
Write-Log "仓库地址: https://github.com/$username/$repoName" "Cyan"
Write-Log "详细日志已保存到: $logFile" "Yellow"

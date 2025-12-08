#!/bin/bash

# Git 认证配置脚本
# 此脚本用于配置 GitHub 推送认证

echo "=== Git 认证配置 ==="
echo ""
echo "请选择认证方式："
echo "1. 使用 Personal Access Token (HTTPS)"
echo "2. 使用 SSH 密钥"
echo ""
read -p "请选择 (1 或 2): " choice

if [ "$choice" = "1" ]; then
    echo ""
    echo "=== 配置 HTTPS + Personal Access Token ==="
    echo "1. 访问 https://github.com/settings/tokens"
    echo "2. 点击 'Generate new token (classic)'"
    echo "3. 选择权限: 至少需要 'repo' 权限"
    echo "4. 生成并复制 token"
    echo ""
    read -p "请输入您的 GitHub 用户名: " username
    read -s -p "请输入您的 Personal Access Token: " token
    echo ""
    
    # 使用 token 配置远程 URL
    git remote set-url origin https://${username}:${token}@github.com/maiyadiu/JX1Tool-Hanhua.git
    
    echo ""
    echo "✅ HTTPS 认证已配置"
    echo "正在测试推送..."
    git push origin main
    
elif [ "$choice" = "2" ]; then
    echo ""
    echo "=== 配置 SSH 密钥 ==="
    echo "您的 SSH 公钥："
    cat ~/.ssh/id_ed25519.pub
    echo ""
    echo "请执行以下步骤："
    echo "1. 复制上面的公钥内容"
    echo "2. 访问 https://github.com/settings/keys"
    echo "3. 点击 'New SSH key'"
    echo "4. 粘贴公钥并保存"
    echo ""
    read -p "完成上述步骤后，按 Enter 继续..."
    
    # 切换为 SSH URL
    git remote set-url origin git@github.com:maiyadiu/JX1Tool-Hanhua.git
    
    echo ""
    echo "✅ SSH 配置完成"
    echo "正在测试推送..."
    git push origin main
else
    echo "无效的选择"
    exit 1
fi


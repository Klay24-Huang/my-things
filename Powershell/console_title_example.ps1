# 更改 PowerShell 字元集為 UTF-8
chcp 65001

# 初始標題
$Host.UI.RawUI.WindowTitle = '開始執行腳本'

# 模擬腳本運行過程
Write-Host '執行步驟 1...'
Start-Sleep -Seconds 2
$Host.UI.RawUI.WindowTitle = '正在執行步驟 1'

Write-Host '執行步驟 2...'
Start-Sleep -Seconds 2
$Host.UI.RawUI.WindowTitle = '正在執行步驟 2'

Write-Host '腳本執行完成！'
$Host.UI.RawUI.WindowTitle = '腳本完成'

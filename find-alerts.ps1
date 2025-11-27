# PowerShell script to find all alert() and confirm() usages
# Run this script from the project root directory

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CareSync Alert Usage Finder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = ".\CareSync"
$alertPattern = "alert\("
$confirmPattern = "confirm\("

# Find all .cshtml files
$files = Get-ChildItem -Path $projectPath -Filter "*.cshtml" -Recurse | Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" }

$totalAlerts = 0
$totalConfirms = 0
$fileResults = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    $alertMatches = ([regex]::Matches($content, $alertPattern)).Count
    $confirmMatches = ([regex]::Matches($content, $confirmPattern)).Count
    
    $totalIssues = $alertMatches + $confirmMatches
    
    if ($totalIssues -gt 0) {
        $totalAlerts += $alertMatches
        $totalConfirms += $confirmMatches
        
        $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart("\")
        
        $fileResults += [PSCustomObject]@{
            File = $relativePath
            Alerts = $alertMatches
            Confirms = $confirmMatches
            Total = $totalIssues
        }
    }
}

# Sort by total count descending
$fileResults = $fileResults | Sort-Object -Property Total -Descending

# Display results
Write-Host "SUMMARY" -ForegroundColor Yellow
Write-Host "-------" -ForegroundColor Yellow
Write-Host "Total Files with Issues: $($fileResults.Count)" -ForegroundColor White
Write-Host "Total alert() calls: $totalAlerts" -ForegroundColor Red
Write-Host "Total confirm() calls: $totalConfirms" -ForegroundColor Red
Write-Host "Total issues: $($totalAlerts + $totalConfirms)" -ForegroundColor Red
Write-Host ""

Write-Host "FILES BY PRIORITY (Most issues first)" -ForegroundColor Yellow
Write-Host "-------------------------------------" -ForegroundColor Yellow
Write-Host ""

$fileResults | Format-Table -Property @{
    Label = "File"
    Expression = { $_.File }
    Width = 70
}, @{
    Label = "Alerts"
    Expression = { $_.Alerts }
    Width = 8
}, @{
    Label = "Confirms"
    Expression = { $_.Confirms }
    Width = 10
}, @{
    Label = "Total"
    Expression = { $_.Total }
    Width = 7
} -AutoSize

Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Green
Write-Host "1. Update files starting from the top (highest count)" -ForegroundColor White
Write-Host "2. Replace alert() with toast.success/error/warning/info()" -ForegroundColor White
Write-Host "3. Replace confirm() with toast.confirm()" -ForegroundColor White
Write-Host "4. See TOAST_IMPLEMENTATION_GUIDE.md for examples" -ForegroundColor White
Write-Host ""

# Save detailed report to file
$reportPath = ".\ALERT_REPLACEMENT_REPORT.txt"
$fileResults | Out-File -FilePath $reportPath -Encoding UTF8

Write-Host "Detailed report saved to: $reportPath" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = 'Stop'

function Update-FileText {
    param(
        [string]$Path,
        [scriptblock]$Updater
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "File not found: $Path"
    }

    $old = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    $new = & $Updater $old

    if ($null -eq $new -or $new -eq $old) {
        Write-Host "No change (already applied or anchor not matched): $Path"
        return
    }

    Set-Content -LiteralPath $Path -Value $new -Encoding UTF8
    Write-Host "Updated: $Path"
}

function Add-AfterFirstRegex {
    param(
        [string]$Text,
        [string]$Pattern,
        [string]$InsertText,
        [string]$ErrorMessage
    )

    $m = [regex]::Match($Text, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)
    if (-not $m.Success) { throw $ErrorMessage }
    $pos = $m.Index + $m.Length
    return $Text.Insert($pos, $InsertText)
}

function Find-ProjectRoot {
    param([string]$StartDir)

    $dir = $StartDir
    while ($dir) {
        $candidate = Join-Path $dir 'LCD\Data\Project.cs'
        if (Test-Path -LiteralPath $candidate) { return $dir }

        $parent = Split-Path -Parent $dir
        if ($parent -eq $dir) { break }
        $dir = $parent
    }

    throw "Project root not found from: $StartDir"
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Find-ProjectRoot -StartDir $scriptDir

$projectCs  = Join-Path $root 'LCD\Data\Project.cs'
$mpcViewXaml = Join-Path $root 'LCD\View\MPCView.xaml'
$movCtrlCs  = Join-Path $root 'LCD\Ctrl\MovCtrl.cs'

# -----------------------------------------------------------------------
# 1. Project.cs  -- add IsFlipped computed property to Config class
# -----------------------------------------------------------------------
Update-FileText -Path $projectCs -Updater {
    param($text)
    if ($text -match 'public bool IsFlipped') { return $text }
    Add-AfterFirstRegex $text `
        '(?m)^\s*public int VReverse;\s*\r?\n' `
        "        public bool IsFlipped => EQType == 5; // 仪器固定旋转式`r`n" `
        'VReverse anchor not found in Project.cs'
}

# -----------------------------------------------------------------------
# 2. MPCView.xaml  -- add "仪器固定旋转式" ComboBoxItem (index 5)
# -----------------------------------------------------------------------
Update-FileText -Path $mpcViewXaml -Updater {
    param($text)
    if ($text -match '仪器固定旋转式') { return $text }
    Add-AfterFirstRegex $text `
        '(?m)^\s*<ComboBoxItem Content="仪器旋转式"></ComboBoxItem>\s*\r?\n' `
        "                        <ComboBoxItem Content=`"仪器固定旋转式`"></ComboBoxItem>`r`n" `
        '仪器旋转式 anchor not found in MPCView.xaml'
}

# -----------------------------------------------------------------------
# 3. MovCtrl.cs  -- MoveXAxisDown: inject dir variable
# -----------------------------------------------------------------------
Update-FileText -Path $movCtrlCs -Updater {
    param($text)
    if ($text -match '(?s)MoveXAxisDown[\s\S]{0,200}IsFlipped') { return $text }

    $m = [regex]::Match($text, '(?s)(public void MoveXAxisDown\(EnumMoveSpeed speed\)\s*\{[\s\S]*?if \(!axe\.IsEnable\) \{ return; \}[ \t]*)(\r?\n)', [System.Text.RegularExpressions.RegexOptions]::None)
    if (-not $m.Success) { throw 'MoveXAxisDown IsEnable anchor not found in MovCtrl.cs' }
    $insert = $m.Groups[2].Value + '            int dir = Project.cfg.IsFlipped ? -axe.direction : axe.direction;' + $m.Groups[2].Value
    $text = $text.Substring(0, $m.Index + $m.Length) + $insert + $text.Substring($m.Index + $m.Length)

    # Replace axe.direction with dir inside MoveXAxisDown block
    $bm = [regex]::Match($text, '(?s)(public void MoveXAxisDown\(EnumMoveSpeed speed\)\s*\{)([\s\S]*?)(\r?\n        \})')
    if ($bm.Success) {
        $blockOld = $bm.Value
        $blockNew = $blockOld -replace 'MoveAxisNoWaitByVector\(axe, speed, axe\.direction\)', 'MoveAxisNoWaitByVector(axe, speed, dir)' `
                              -replace 'MoveAxisNoWait\(axe, speed, axe\.direction\)', 'MoveAxisNoWait(axe, speed, dir)'
        if ($blockNew -ne $blockOld) {
            $text = $text.Substring(0, $bm.Index) + $blockNew + $text.Substring($bm.Index + $bm.Length)
        }
    }
    return $text
}

# -----------------------------------------------------------------------
# 4. MovCtrl.cs  -- MoveYAxiesDown: inject dir variable
# -----------------------------------------------------------------------
Update-FileText -Path $movCtrlCs -Updater {
    param($text)
    if ($text -match '(?s)MoveYAxiesDown[\s\S]{0,200}IsFlipped') { return $text }

    $m = [regex]::Match($text, '(?s)(public void MoveYAxiesDown\(EnumMoveSpeed speed\)\s*\{[\s\S]*?if \(!axe\.IsEnable\) \{ return; \}[ \t]*)(\r?\n)', [System.Text.RegularExpressions.RegexOptions]::None)
    if (-not $m.Success) { throw 'MoveYAxiesDown IsEnable anchor not found in MovCtrl.cs' }
    $insert = $m.Groups[2].Value + '            int dir = Project.cfg.IsFlipped ? -axe.direction : axe.direction;' + $m.Groups[2].Value
    $text = $text.Substring(0, $m.Index + $m.Length) + $insert + $text.Substring($m.Index + $m.Length)

    $bm = [regex]::Match($text, '(?s)(public void MoveYAxiesDown\(EnumMoveSpeed speed\)\s*\{)([\s\S]*?)(\r?\n        \})')
    if ($bm.Success) {
        $blockOld = $bm.Value
        $blockNew = $blockOld -replace 'MoveAxisNoWaitByVector\(axe, speed, axe\.direction\)', 'MoveAxisNoWaitByVector(axe, speed, dir)' `
                              -replace 'MoveAxisNoWait\(axe, speed, axe\.direction\)', 'MoveAxisNoWait(axe, speed, dir)'
        if ($blockNew -ne $blockOld) {
            $text = $text.Substring(0, $bm.Index) + $blockNew + $text.Substring($bm.Index + $bm.Length)
        }
    }
    return $text
}

Write-Host ""
Write-Host "Flip update (仪器固定旋转式) applied."
Write-Host "Please rebuild the project on the target machine."

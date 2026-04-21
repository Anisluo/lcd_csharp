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
        Write-Host "No change: $Path"
        return
    }

    Set-Content -LiteralPath $Path -Value $new -Encoding UTF8
    Write-Host "Updated: $Path"
}

function Insert-AfterFirstRegex {
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

function Replace-FirstRegex {
    param(
        [string]$Text,
        [string]$Pattern,
        [string]$Replacement,
        [string]$ErrorMessage
    )

    $m = [regex]::Match($Text, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)
    if (-not $m.Success) { throw $ErrorMessage }
    return $Text.Substring(0, $m.Index) + $Replacement + $Text.Substring($m.Index + $m.Length)
}

function Find-ProjectRoot {
    param(
        [string]$StartDir
    )

    $dir = $StartDir
    while ($dir) {
        $candidate1 = Join-Path $dir 'LCD\Data\Project.cs'
        if (Test-Path -LiteralPath $candidate1) {
            return $dir
        }

        $candidate2 = Join-Path $dir 'Data\Project.cs'
        $candidate3 = Join-Path $dir 'View\CustomView.xaml.cs'
        if ((Test-Path -LiteralPath $candidate2) -and (Test-Path -LiteralPath $candidate3)) {
            return (Split-Path -Parent $dir)
        }

        $parent = Split-Path -Parent $dir
        if ($parent -eq $dir) {
            break
        }
        $dir = $parent
    }

    throw "Project root not found from: $StartDir"
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Find-ProjectRoot -StartDir $scriptDir

$projectCs = Join-Path $root 'LCD\Data\Project.cs'
$customViewXaml = Join-Path $root 'LCD\View\CustomView.xaml'
$customViewCs = Join-Path $root 'LCD\View\CustomView.xaml.cs'
$dataTemplateViewCs = Join-Path $root 'LCD\View\DataTemplateView.xaml.cs'
$processCtrlCs = Join-Path $root 'LCD\Ctrl\ProcessCtrl.cs'
$customTemplateCs = Join-Path $root 'LCD\View\CustomTemplate.xaml.cs'
$mainWindowCs = Join-Path $root 'LCD\MainWindow.xaml.cs'

Update-FileText -Path $projectCs -Updater {
    param($text)
    if ($text -match '(?m)^\s*TCO,\s*$') { return $text }
    Insert-AfterFirstRegex $text '(?m)^\s*_07_warmup,\s*\r?\n' "        TCO,`r`n" 'ENUMMESSTYLE anchor not found in Project.cs'
}

Update-FileText -Path $customViewXaml -Updater {
    param($text)
    if ($text.Contains('<ComboBoxItem>08-TCO</ComboBoxItem>')) { return $text }
    Insert-AfterFirstRegex $text '(?m)^\s*<ComboBoxItem>07-warmup</ComboBoxItem>\s*\r?\n' "                                <ComboBoxItem>08-TCO</ComboBoxItem>`r`n" 'ComboBox anchor not found in CustomView.xaml'
}

Update-FileText -Path $customViewCs -Updater {
    param($text)

    if ($text -notmatch 'inf\.Type = "TCO";') {
        $text = Insert-AfterFirstRegex $text '(?ms)else if\s*\(\s*inf\.MESTYPE\s*==\s*ENUMMESSTYLE\._07_warmup\s*\)\s*\{\s*inf\.Type\s*=\s*"Warmup";\s*\}\s*' "else if (inf.MESTYPE == ENUMMESSTYLE.TCO)`r`n            {`r`n                inf.Type = `"TCO`";`r`n            }`r`n            " 'TCO update_type anchor not found in CustomView.xaml.cs'
    }

    if ($text -notmatch 'IndexOf\("TCO"\)') {
        $text = Insert-AfterFirstRegex $text '(?m)^\s*else if\s*\(this\.comboBoxType\.Text\.IndexOf\("7"\)!=-1\)\s*\{\s*MESTYPE\s*=\s*ENUMMESSTYLE\._07_warmup;\s*\}\s*\r?\n' '            else if ((this.comboBoxType.Text.IndexOf("08") != -1) || (this.comboBoxType.Text.IndexOf("TCO") != -1)) { MESTYPE = ENUMMESSTYLE.TCO; }' + "`r`n" 'AddTemplate mapping anchor not found in CustomView.xaml.cs'
    }

    if ($text -notmatch 'case ENUMMESSTYLE\.TCO:\s*InitDataGrid_POINT\(info\);') {
        $text = Insert-AfterFirstRegex $text '(?m)^\s*case ENUMMESSTYLE\._07_warmup:\s*InitDataGrid_warmup\(info\);\s*break;\s*\r?\n' '                case ENUMMESSTYLE.TCO: InitDataGrid_POINT(info); break;' + "`r`n" 'InitDataGrid switch anchor not found in CustomView.xaml.cs'
    }

    if ($text -notmatch 'infoData\.MESTYPE = info\.MESTYPE;') {
        if ($text -match '(?m)^\s*infoData\.id = info\.id;\s*$') {
            $text = Replace-FirstRegex $text '(?m)^(\s*infoData\.id = info\.id;\s*)$' '$1' + "`r`n" + '                infoData.MESTYPE = info.MESTYPE;' 'infoData.id line not found in CustomView.xaml.cs'
        }

        if ($text -notmatch 'else\s*\{\s*infoData\.MESTYPE = info\.MESTYPE;\s*\}') {
            if ($text -match '(?m)^\s*if\s*\(\s*infoData\.MESTYPE\s*==\s*ENUMMESSTYLE\._05_CROSSTALK\s*\)\s*$') {
                $text = Insert-AfterFirstRegex $text '(?m)^\s*\}\s*\r?\n(?=\s*if\s*\(\s*infoData\.MESTYPE\s*==\s*ENUMMESSTYLE\._05_CROSSTALK\s*\))' '            else' + "`r`n" + '            {' + "`r`n" + '                infoData.MESTYPE = info.MESTYPE;' + "`r`n" + '            }' + "`r`n" 'Could not insert else block in CustomView.xaml.cs'
            } elseif ($text -match '(?m)^\s*CustomTemplate cstm = new CustomTemplate\(info, infoData\);\s*$') {
                $text = Insert-AfterFirstRegex $text '(?m)^\s*\}\s*\r?\n(?=\s*CustomTemplate cstm = new CustomTemplate\(info, infoData\);)' '            else' + "`r`n" + '            {' + "`r`n" + '                infoData.MESTYPE = info.MESTYPE;' + "`r`n" + '            }' + "`r`n" 'Could not insert else block before CustomTemplate in CustomView.xaml.cs'
            } else {
                throw 'AutoCreate else anchor not found in CustomView.xaml.cs'
            }
        }
    }

    if ($text -notmatch 'case 5:\s*MESTYPE = ENUMMESSTYLE\.TCO; break;') {
        $text = Insert-AfterFirstRegex $text '(?m)^\s*case 4:\s*MESTYPE = ENUMMESSTYLE\._07_warmup;\s*break;\s*\r?\n' '                case 5: MESTYPE = ENUMMESSTYLE.TCO; break;' + "`r`n" 'comboBoxType switch anchor not found in CustomView.xaml.cs'
    }

    if ($text -notmatch 'MESTYPE == ENUMMESSTYLE\.TCO') {
        if ($text -match '(?ms)if\s*\(\s*MESTYPE == ENUMMESSTYLE\._01_POINT.*?MESTYPE == ENUMMESSTYLE\._05_CROSSTALK\s*\)') {
            $text = Replace-FirstRegex $text '(?ms)if\s*\(\s*MESTYPE == ENUMMESSTYLE\._01_POINT\s*\|\|\s*MESTYPE == ENUMMESSTYLE\._07_warmup\|\|\s*MESTYPE == ENUMMESSTYLE\._03_SPECTRUM \|\| MESTYPE == ENUMMESSTYLE\._05_CROSSTALK\s*\)' 'if (MESTYPE == ENUMMESSTYLE._01_POINT || MESTYPE == ENUMMESSTYLE._07_warmup||' + "`r`n" + '                    MESTYPE == ENUMMESSTYLE._03_SPECTRUM || MESTYPE == ENUMMESSTYLE._05_CROSSTALK ||' + "`r`n" + '                    MESTYPE == ENUMMESSTYLE.TCO)' 'AutoCreate visibility block anchor not found in CustomView.xaml.cs'
        }
    }

    return $text
}

Update-FileText -Path $dataTemplateViewCs -Updater {
    param($text)
    if ($text -match 'case ENUMMESSTYLE\.TCO:\s*Init_POINT') { return $text }
    Insert-AfterFirstRegex $text '(?m)^\s*case ENUMMESSTYLE\._07_warmup:\s*Init_POINT\(lstdata, InitDataTemplate\);\s*break;\s*\r?\n' '                    case ENUMMESSTYLE.TCO: Init_POINT(lstdata, InitDataTemplate); break;' + "`r`n" 'Init switch anchor not found in DataTemplateView.xaml.cs'
}

Update-FileText -Path $processCtrlCs -Updater {
    param($text)
    if ($text -match 'case ENUMMESSTYLE\.TCO:\s*ProcessPointTemplateXYZ') { return $text }
    Insert-AfterFirstRegex $text '(?m)^\s*case ENUMMESSTYLE\._07_warmup:\s*ProcessPointTemplateXYZ\(dt, ENUMMESSTYLE\._07_warmup, TestName, Project\.lstInfos\[i\]\.id, height\);\s*;\s*break;\s*\r?\n' '                        case ENUMMESSTYLE.TCO: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._01_POINT, TestName, Project.lstInfos[i].id, height); break;' + "`r`n" 'Run switch anchor not found in ProcessCtrl.cs'
}

Update-FileText -Path $customTemplateCs -Updater {
    param($text)
    if ($text -match 'infodata\.MESTYPE = info\.MESTYPE;') { return $text }
    Replace-FirstRegex $text '(?m)^\s*infodata\.MESTYPE = ENUMMESSTYLE\._01_POINT;\s*$' '                infodata.MESTYPE = info.MESTYPE;' 'SaveFileDialog MESTYPE anchor not found in CustomTemplate.xaml.cs'
}

Update-FileText -Path $mainWindowCs -Updater {
    param($text)
    if ($text -match 'eNUMMESSTYLE == ENUMMESSTYLE\._01_POINT \|\| eNUMMESSTYLE == ENUMMESSTYLE\.TCO') { return $text }
    Replace-FirstRegex $text '(?m)^\s*if \(eNUMMESSTYLE == ENUMMESSTYLE\._01_POINT\)\s*$' '            if (eNUMMESSTYLE == ENUMMESSTYLE._01_POINT || eNUMMESSTYLE == ENUMMESSTYLE.TCO)' 'Pctrl_UpDataUi anchor not found in MainWindow.xaml.cs'
}

Write-Host ""
Write-Host "TCO minimal update applied with v3 script."
Write-Host "Please rebuild the project on the target machine."

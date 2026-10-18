param(
    [string] $ProjectPath = (Join-Path $PSScriptRoot '../VisualPlayer')
)

$ErrorActionPreference = 'Stop'
$project = (Resolve-Path -LiteralPath $ProjectPath).Path
$englishPath = [IO.Path]::GetFullPath((Join-Path $project 'Resources/EnglishTranslations.cs'))
$polishPath = [IO.Path]::GetFullPath((Join-Path $project 'Lang/pl-PL.json'))
$english = @{}
$polish = @{}
$problems = [System.Collections.Generic.List[string]]::new()

# Read C# string literals without treating escaped quotation marks as delimiters.
$entries = [regex]::Matches([IO.File]::ReadAllText($englishPath),
    '\["(?<key>[^"\r\n]+)"\]\s*=\s*"(?<value>(?:\\.|[^"\\])*)"')
foreach ($entry in $entries) {
    $key = $entry.Groups['key'].Value
    if ($english.ContainsKey($key)) { $problems.Add("Duplicate English key: $key") }
    $english[$key] = $entry.Groups['value'].Value
}
if ($english.Count -eq 0) { throw 'No English translations found.' }

$polishJson = [IO.File]::ReadAllText($polishPath)
$language = $polishJson | ConvertFrom-Json
if ($language.header.code -ne 'pl-PL') { $problems.Add('Invalid Polish language header.') }
foreach ($entry in $language.translations.PSObject.Properties) {
    $polish[$entry.Name] = $entry.Value
}
# ConvertFrom-Json can silently collapse repeated JSON properties.
$jsonKeys = @{}
foreach ($entry in [regex]::Matches($polishJson, '"([A-Z][A-Z0-9_]+)"\s*:')) {
    $key = $entry.Groups[1].Value
    if ($jsonKeys.ContainsKey($key)) { $problems.Add("Duplicate Polish key: $key") }
    $jsonKeys[$key] = $true
}

foreach ($key in @($english.Keys + $polish.Keys | Sort-Object -Unique)) {
    if (!$english.ContainsKey($key)) { $problems.Add("Missing English translation: $key"); continue }
    if (!$polish.ContainsKey($key)) { $problems.Add("Missing Polish translation: $key"); continue }
    foreach ($dictionary in @($english, $polish)) {
        if ([string]::IsNullOrWhiteSpace($dictionary[$key]) -or $dictionary[$key] -eq $key) {
            $problems.Add("Empty or untranslated value: $key")
        }
    }
    $englishArguments = @([regex]::Matches($english[$key], '(?<!\{)\{(\d+)(?:[,}:])') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique) -join ','
    $polishArguments = @([regex]::Matches($polish[$key], '(?<!\{)\{(\d+)(?:[,}:])') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique) -join ','
    if ($englishArguments -ne $polishArguments) { $problems.Add("Different format arguments: $key") }
}

# Covers XAML indexer bindings and C# literals (including keys passed via T,
# Format, converters and helper methods). Exclude generated/build files.
$references = @{}
Get-ChildItem -LiteralPath $project -Recurse -File | Where-Object {
    $_.Extension -in '.cs', '.axaml' -and
    $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.FullName -ne $englishPath
} | ForEach-Object {
    $file = $_.FullName
    foreach ($match in [regex]::Matches([IO.File]::ReadAllText($file),
        'Translations\[([A-Z][A-Z0-9_]+)\]|"([A-Z][A-Z0-9]*(?:_[A-Z0-9]+)+)"')) {
        $key = if ($match.Groups[1].Success) { $match.Groups[1].Value } else { $match.Groups[2].Value }
        $references[$key] = $true
        if (!$english.ContainsKey($key)) { $problems.Add("Undefined translation: $key ($file)") }
    }
}

if ($problems.Count -gt 0) {
    $problems | Sort-Object -Unique | ForEach-Object { Write-Output $_ }
    throw "Translation audit failed: $($problems.Count) problem(s)."
}
Write-Output "Translations OK: $($english.Count) English, $($polish.Count) Polish, $($references.Count) referenced keys."

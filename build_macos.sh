#!/usr/bin/env bash
set -euo pipefail

usage() {
    echo "Usage: bash build_macos.sh [osx-arm64|osx-x64]"
    echo "Default: the architecture of this computer."
    echo "Builds a self-contained VisualPlayer.app on macOS."
}

if [[ ${1:-} == --help || ${1:-} == -h ]]; then
    usage
    exit 0
fi
if (( $# > 1 )); then
    usage >&2
    exit 2
fi

runtime=${1:-}
if [[ -z $runtime ]]; then
    case "$(uname -m)" in
        arm64|aarch64) runtime=osx-arm64 ;;
        x86_64|amd64) runtime=osx-x64 ;;
        *) echo "ERROR: Unsupported architecture. Specify osx-arm64 or osx-x64." >&2; exit 2 ;;
    esac
fi
case "$runtime" in
    osx-arm64|osx-x64) ;;
    *) usage >&2; exit 2 ;;
esac

if [[ $(uname -s) != Darwin ]]; then
    echo "ERROR: Building the .app bundle requires macOS (sips, iconutil, plutil, codesign)." >&2
    exit 1
fi

if ! command -v dotnet >/dev/null 2>&1; then
    echo "ERROR: Install the .NET 10 SDK and restart your terminal." >&2
    exit 1
fi
for tool in sips iconutil plutil codesign; do
    if ! command -v "$tool" >/dev/null 2>&1; then
        echo "ERROR: Required macOS tool is missing: $tool" >&2
        exit 1
    fi
done

script_dir=$(CDPATH='' cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
project="$script_dir/VisualPlayer/VisualPlayer.csproj"
output_dir="$script_dir/artifacts/publish/$runtime"
mkdir -p "$output_dir"
work_dir=$(mktemp -d "$output_dir/.bundle.XXXXXX")
app_dir=''

cleanup() {
    local result=$?
    # Restore the previous package if installing the newly built one failed.
    if [[ -d "$work_dir/previous.app" && -n $app_dir && ! -e $app_dir ]]; then
        mv -- "$work_dir/previous.app" "$app_dir"
    fi
    if [[ $work_dir == "$output_dir"/.bundle.* && -d $work_dir ]]; then
        rm -rf -- "$work_dir"
    fi
    return "$result"
}
trap cleanup EXIT

# Read evaluated MSBuild properties, including overrides from imported files.
metadata="$work_dir/metadata.json"
dotnet msbuild "$project" -nologo -p:Configuration=Release \
    -getProperty:AssemblyName,AssemblyTitle,Product,Version,InformationalVersion,Authors,Company,Copyright,MacOSBundleIdentifier,MacOSMinimumSystemVersion > "$metadata"
property() { plutil -extract "Properties.$1" raw -o - "$metadata"; }

assembly_name=$(property AssemblyName)
title=$(property AssemblyTitle)
product=$(property Product)
version=$(property Version)
information_version=$(property InformationalVersion)
authors=$(property Authors)
company=$(property Company)
copyright=$(property Copyright)
bundle_id=$(property MacOSBundleIdentifier)
minimum_version=$(property MacOSMinimumSystemVersion)

# Keep the executable/package name a single safe path component.
if [[ ! $assembly_name =~ ^[A-Za-z0-9_-][A-Za-z0-9._-]*$ ||
      ! $bundle_id =~ ^[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)+$ ||
      ! $version =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)(\.[0-9]+)?([+-].*)?$ ]]; then
    echo "ERROR: Invalid assembly name, bundle identifier or project version." >&2
    exit 1
fi
short_version="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.${BASH_REMATCH[3]}"
bundle_version=${MACOS_BUNDLE_VERSION:-$short_version}
if [[ ! $bundle_version =~ ^[0-9]+(\.[0-9]+){0,2}$ ]]; then
    echo "ERROR: MACOS_BUNDLE_VERSION must contain one to three numeric components." >&2
    exit 1
fi

app_dir="$output_dir/$assembly_name.app"
staged_app="$work_dir/$assembly_name.app"
contents="$staged_app/Contents"
mkdir -p "$contents/MacOS" "$contents/Resources"

echo "Publishing VisualPlayer Release for $runtime..."
publish_dir="$work_dir/publish"
dotnet publish "$project" \
    --configuration Release --runtime "$runtime" --self-contained true \
    --output "$publish_dir" \
    -p:PublishTrimmed=false -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:DebugSymbols=false -p:DebugType=None -p:UseAppHost=true
mv -- "$publish_dir/$assembly_name" "$contents/MacOS/$assembly_name"
if [[ -d "$publish_dir/Lang" ]]; then
    mv -- "$publish_dir/Lang" "$contents/Resources/Lang"
fi
chmod +x "$contents/MacOS/$assembly_name"

# Supply all standard and Retina icon sizes using the existing application logo.
iconset="$work_dir/VisualPlayer.iconset"
mkdir -p "$iconset"
for size in 16 32 128 256 512; do
    sips -z "$size" "$size" "$script_dir/VisualPlayer/Assets/logo.png" \
        --out "$iconset/icon_${size}x${size}.png" >/dev/null
    retina_size=$((size * 2))
    sips -z "$retina_size" "$retina_size" "$script_dir/VisualPlayer/Assets/logo.png" \
        --out "$iconset/icon_${size}x${size}@2x.png" >/dev/null
done
iconutil -c icns "$iconset" -o "$contents/Resources/VisualPlayer.icns"

plist="$contents/Info.plist"
printf '%s\n' '<?xml version="1.0" encoding="UTF-8"?>' \
    '<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">' \
    '<plist version="1.0"><dict/></plist>' > "$plist"
plist_string() { plutil -insert "$1" -string "$2" "$plist"; }
plist_string CFBundleExecutable "$assembly_name"
plist_string CFBundleIdentifier "$bundle_id"
plist_string CFBundleName "$product"
plist_string CFBundleDisplayName "$title"
plist_string CFBundlePackageType APPL
plist_string CFBundleInfoDictionaryVersion 6.0
plist_string CFBundleShortVersionString "$short_version"
plist_string CFBundleVersion "$bundle_version"
plist_string CFBundleLongVersionString "$information_version"
plist_string CFBundleGetInfoString "$title $information_version, $authors, $company. $copyright"
plist_string CFBundleIconFile VisualPlayer.icns
plist_string CFBundleDevelopmentRegion en
plist_string NSHumanReadableCopyright "$copyright"
plist_string NSPrincipalClass NSApplication
plist_string LSApplicationCategoryType public.app-category.music
plist_string LSMinimumSystemVersion "$minimum_version"
plutil -insert NSHighResolutionCapable -bool YES "$plist"
plutil -insert CFBundleSupportedPlatforms -json '["MacOSX"]' "$plist"
plutil -insert CFBundleLocalizations -json '["en", "pl"]' "$plist"
plutil -lint "$plist"
printf 'APPL????' > "$contents/PkgInfo"

# Local ad-hoc signatures allow the assembled bundle to run on Apple Silicon.
codesign --force --sign - "$contents/MacOS/$assembly_name"
codesign --force --sign - "$staged_app"
codesign --verify --deep --strict "$staged_app"

# Replace an older output only after the complete new package passes validation.
if [[ -e $app_dir ]]; then
    mv -- "$app_dir" "$work_dir/previous.app"
fi
mv -- "$staged_app" "$app_dir"

echo "Ready: $app_dir"
echo "Open this .app in Finder, or run: open \"$app_dir\""

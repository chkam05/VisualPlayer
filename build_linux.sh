#!/usr/bin/env bash
set -euo pipefail

usage() {
    echo "Usage: bash build_linux.sh [linux-x64|linux-arm64|linux-arm]"
    echo "Default: the architecture of this computer."
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
        x86_64|amd64) runtime=linux-x64 ;;
        aarch64|arm64) runtime=linux-arm64 ;;
        armv7l|armv8l) runtime=linux-arm ;;
        *) echo "ERROR: Unsupported architecture. Specify a supported Linux runtime." >&2; exit 2 ;;
    esac
fi
case "$runtime" in
    linux-x64|linux-arm64|linux-arm) ;;
    *) usage >&2; exit 2 ;;
esac

if ! command -v dotnet >/dev/null 2>&1; then
    echo "ERROR: Install the .NET 10 SDK and restart your terminal." >&2
    exit 1
fi

script_dir=$(CDPATH='' cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
output_dir="$script_dir/artifacts/publish/$runtime"

echo "Publishing VisualPlayer Release for $runtime..."
dotnet publish "$script_dir/VisualPlayer/VisualPlayer.csproj" \
    --configuration Release --runtime "$runtime" --self-contained true \
    --output "$output_dir" \
    -p:PublishTrimmed=false -p:PublishSingleFile=false -p:UseAppHost=true
chmod +x "$output_dir/VisualPlayer"

echo "Ready: $output_dir/VisualPlayer"
echo "Copy the entire output folder when distributing the application."

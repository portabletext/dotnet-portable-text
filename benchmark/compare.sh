#!/usr/bin/env bash
# Benchmarks the working tree ("Diff") against a base commit ("Base") in a single
# BenchmarkDotNet run, and stores the results under benchmark/results/<name>/:
#
#   report.md    Markdown table with Ratio, Mann-Whitney U test and allocations
#   report.json  Structured BenchmarkDotNet results, including the environment
#
# Usage: benchmark/compare.sh [base-ref] [name] [-- extra BenchmarkDotNet arguments]
#
#   base-ref  Commit to compare against. Default: HEAD
#   name      Name of the results folder. Default: a timestamp
#
# Environment:
#   FRAMEWORK  Runtime to benchmark on. Default: net10.0
#
# Examples:
#   benchmark/compare.sh
#   benchmark/compare.sh main avoid-reparsing
#   benchmark/compare.sh HEAD article-only -- --filter '*Article*'
set -euo pipefail

base_ref="HEAD"
name="$(date +%Y-%m-%d-%H%M%S)"
extra_args=()
positional=0
while [[ $# -gt 0 ]]; do
  case "$1" in
    --) shift; extra_args=("$@"); break ;;
    *)
      positional=$((positional + 1))
      if [[ $positional -eq 1 ]]; then base_ref="$1"; elif [[ $positional -eq 2 ]]; then name="$1"; fi
      shift ;;
  esac
done

framework="${FRAMEWORK:-net10.0}"
repo_root="$(git rev-parse --show-toplevel)"
base_sha="$(git -C "$repo_root" rev-parse --verify --short=12 "$base_ref^{commit}")"
head_sha="$(git -C "$repo_root" rev-parse --short=12 HEAD)"
dirty=""
if [[ -n "$(git -C "$repo_root" status --porcelain -- src)" ]]; then dirty=" + uncommitted changes in src/"; fi
results_dir="$repo_root/benchmark/results/$name"

if [[ -e "$results_dir" ]]; then
  echo "error: $results_dir already exists" >&2
  exit 1
fi

work="$(mktemp -d)"
cleanup() {
  git -C "$repo_root" worktree remove --force "$work/base" 2>/dev/null || true
  rm -rf "$work"
}
trap cleanup EXIT

echo "==> Packing PortableText from $base_sha"
git -C "$repo_root" worktree add --detach --quiet "$work/base" "$base_sha"
# Unique per commit, so NuGet's package cache never serves a different build under the same version
baseline_version="0.0.0-baseline.g$base_sha"
dotnet pack "$work/base/src/PortableText" --configuration Release --output "$work/feed" --nologo --verbosity quiet \
  -p:Version="$baseline_version" -p:EnablePackageValidation=false -p:IncludeSymbols=false

# Run every benchmark unless the extra arguments contain a filter. BenchmarkDotNet rejects
# --filter when it is given twice.
has_filter=false
for arg in ${extra_args[@]+"${extra_args[@]}"}; do
  case "$arg" in --filter|--filter=*|-f) has_filter=true ;; esac
done
filter_args=()
if [[ "$has_filter" == false ]]; then filter_args=(--filter '*'); fi

echo "==> Benchmarking $base_sha (Base) against the working tree (Diff) on $framework"
PORTABLETEXT_BASELINE_VERSION="$baseline_version" \
PORTABLETEXT_BASELINE_SOURCE="$work/feed" \
  dotnet run --configuration Release --framework "$framework" --project "$repo_root/benchmark" -- \
    ${filter_args[@]+"${filter_args[@]}"} --statisticalTest 3% --exporters json --artifacts "$work/artifacts" \
    ${extra_args[@]+"${extra_args[@]}"}

mkdir -p "$results_dir"
{
  echo "Base: \`$base_sha\` $(git -C "$repo_root" log -1 --format=%s "$base_sha")"
  echo "Diff: working tree on \`$head_sha\`$dirty"
  echo
  cat "$work"/artifacts/results/*-report-github.md
} > "$results_dir/report.md"
cp "$work"/artifacts/results/*-report-full-compressed.json "$results_dir/report.json"

echo
echo "==> Results saved to ${results_dir#"$repo_root"/}"
cat "$results_dir/report.md"

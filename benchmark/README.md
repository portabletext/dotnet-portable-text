# Benchmarks

## Running

```sh
dotnet run -c Release -f net10.0 --project benchmark -- --filter '*'
```

Runs every benchmark on .NET 8 and .NET 10, with allocations (`MemoryDiagnoser`) and thrown
exceptions (`ExceptionDiagnoser`). Use `--filter '*Article*'` to run a subset.

| Benchmark | Input |
|---|---|
| `HugeJson` | `data/bigcontent.json`, 1 MB and 1309 elements |
| `Article` | The first 25 elements of `bigcontent.json`, about 10 KB with lists, marks and links |
| `ArticleWithCustomSerializers` | `Article` with a custom block style and decorator |

## Proving a performance change

Every performance change should come with evidence that it is an improvement. Make the change,
then compare it with the commit it is based on:

```sh
benchmark/compare.sh HEAD <short-name>   # uncommitted changes in the working tree
benchmark/compare.sh main <short-name>   # commits on a branch, compared with main
```

There is no stored baseline. The script builds `Base` from the given commit every time,
because times measured on another machine or at another time are not comparable.

The script packs the library from the base commit and benchmarks it (`Base`) against the
working tree (`Diff`) in the same BenchmarkDotNet run, on the same machine. It stores the results
in `benchmark/results/<short-name>/`:

- `report.md`: the summary table. `Ratio` and `Alloc Ratio` are relative to `Base`, and
  `MannWhitney(3%)` says whether the difference in time is statistically significant
  (`Faster`, `Slower` or `Same`).
- `report.json`: all measurements and the environment (CPU, OS, runtime), for later analysis.

Commit the results folder together with the change, and include the relevant rows of the
table in the commit message.

Allocated bytes and exceptions are deterministic for a given runtime, so they are hard evidence.
Time varies between runs and machines, so only compare times from the same run.

## Results history

Each folder in `benchmark/results/` is the evidence for one change:

- Name it `<NN>-<what-changed>`, with the next free number and lowercase words separated by
  dashes, for example `05-cache-merged-serializers`. Don't use slashes, they create nested folders.
- Commit it in the same commit as the change. That commit is the "after" state and its parent is
  the "before" state. The SHAs in `report.md` are the ones at measurement time and change when a
  pull request is merged, so find the commit with git instead:

      git log --diff-filter=A --format='%h %s' -- benchmark/results/<name>
      git log --reverse --format='%h %ad %s' --date=short -- benchmark/results/

- Don't rename or change existing benchmarks or `data/bigcontent.json`. Add a new scenario
  instead, so older results stay comparable.
- Allocated bytes and exceptions can be compared between reports measured on the same .NET
  runtime version. Times can only be compared within one report.

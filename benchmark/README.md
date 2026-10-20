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

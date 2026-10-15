Base: `f42d1008f1ec` perf: deserialize from JsonElement instead of re-parsing raw JSON
Diff: working tree on `f42d1008f1ec` + uncommitted changes in src/

```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.402
  [Host] : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Base   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Diff   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a


```
| Method                       | Job  | Mean        | Error    | StdDev   | Ratio | MannWhitney(3%) | Gen0      | Exceptions | Gen1     | Gen2     | Allocated   | Alloc Ratio |
|----------------------------- |----- |------------:|---------:|---------:|------:|---------------- |----------:|-----------:|---------:|---------:|------------:|------------:|
| HugeJson                     | Base | 12,349.4 μs | 35.37 μs | 29.54 μs |  1.00 | Baseline        | 1406.2500 |          - | 718.7500 | 234.3750 | 10965.02 KB |        1.00 |
| HugeJson                     | Diff | 12,103.6 μs | 35.72 μs | 29.83 μs |  0.98 | Same            | 1406.2500 |          - | 781.2500 | 234.3750 | 10834.36 KB |        0.99 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| Article                      | Base |    145.4 μs |  0.82 μs |  0.68 μs |  1.00 | Baseline        |   18.3105 |          - |   0.7324 |        - |   151.34 KB |        1.00 |
| Article                      | Diff |    139.9 μs |  0.30 μs |  0.26 μs |  0.96 | Faster          |   17.3340 |          - |   0.7324 |        - |   143.21 KB |        0.95 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| ArticleWithCustomSerializers | Base |    144.8 μs |  0.45 μs |  0.42 μs |  1.00 | Baseline        |   18.5547 |          - |   0.7324 |        - |   152.66 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |    139.8 μs |  0.30 μs |  0.27 μs |  0.97 | Faster          |   17.5781 |          - |   0.7324 |        - |   144.52 KB |        0.95 |

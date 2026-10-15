Base: `23e34dc06b26` chore(benchmark): add compare script for before/after evidence
Diff: working tree on `23e34dc06b26` + uncommitted changes in src/

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
| HugeJson                     | Base | 16,592.5 μs | 92.69 μs | 86.70 μs |  1.00 | Baseline        | 1625.0000 |  1246.0000 | 593.7500 | 187.5000 | 13206.43 KB |        1.00 |
| HugeJson                     | Diff | 12,826.4 μs | 44.80 μs | 37.41 μs |  0.77 | Faster          | 1625.0000 |          - | 625.0000 | 234.3750 | 12943.63 KB |        0.98 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| Article                      | Base |    187.8 μs |  2.01 μs |  1.88 μs |  1.00 | Baseline        |   22.2168 |    16.0000 |   0.7324 |        - |    181.8 KB |        1.00 |
| Article                      | Diff |    148.8 μs |  0.31 μs |  0.28 μs |  0.79 | Faster          |   21.7285 |          - |   0.7324 |        - |   178.43 KB |        0.98 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| ArticleWithCustomSerializers | Base |    191.7 μs |  1.73 μs |  1.61 μs |  1.00 | Baseline        |   22.2168 |    16.0000 |   0.7324 |        - |   183.12 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |    151.0 μs |  0.39 μs |  0.35 μs |  0.79 | Faster          |   21.9727 |          - |   0.7324 |        - |   179.74 KB |        0.98 |

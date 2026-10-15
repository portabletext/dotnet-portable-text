Base: `197ce4fee6cc` perf: check for list items without throwing exceptions
Diff: working tree on `197ce4fee6cc` + uncommitted changes in src/

```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.402
  [Host] : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Base   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Diff   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a


```
| Method                       | Job  | Mean        | Error     | StdDev    | Ratio | MannWhitney(3%) | RatioSD | Gen0      | Exceptions | Gen1     | Gen2     | Allocated   | Alloc Ratio |
|----------------------------- |----- |------------:|----------:|----------:|------:|---------------- |--------:|----------:|-----------:|---------:|---------:|------------:|------------:|
| HugeJson                     | Base | 12,651.3 μs | 239.61 μs | 266.33 μs |  1.00 | Baseline        |    0.03 | 1640.6250 |          - | 703.1250 | 234.3750 | 12943.59 KB |        1.00 |
| HugeJson                     | Diff | 12,255.6 μs |  24.83 μs |  22.01 μs |  0.97 | Same            |    0.02 | 1406.2500 |          - | 718.7500 | 234.3750 | 10965.08 KB |        0.85 |
|                              |      |             |           |           |       |                 |         |           |            |          |          |             |             |
| Article                      | Base |    147.8 μs |   0.36 μs |   0.34 μs |  1.00 | Baseline        |    0.00 |   21.7285 |          - |   0.7324 |        - |   178.43 KB |        1.00 |
| Article                      | Diff |    145.0 μs |   0.51 μs |   0.45 μs |  0.98 | Same            |    0.00 |   18.3105 |          - |   0.7324 |        - |   151.34 KB |        0.85 |
|                              |      |             |           |           |       |                 |         |           |            |          |          |             |             |
| ArticleWithCustomSerializers | Base |    150.1 μs |   0.62 μs |   0.52 μs |  1.00 | Baseline        |    0.00 |   21.9727 |          - |   0.7324 |        - |   179.74 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |    145.0 μs |   0.31 μs |   0.28 μs |  0.97 | Same            |    0.00 |   18.5547 |          - |   0.7324 |        - |   152.66 KB |        0.85 |

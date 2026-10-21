Base: `4ad4b2dc831e` perf: read spans directly from JsonElement
Diff: working tree on `4ad4b2dc831e` + uncommitted changes in src/

```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.402
  [Host] : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Base   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Diff   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a


```
| Method                       | Job  | Mean        | Error     | StdDev    | Ratio | MannWhitney(3%) | Gen0     | Exceptions | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|----------------------------- |----- |------------:|----------:|----------:|------:|---------------- |---------:|-----------:|---------:|---------:|-----------:|------------:|
| HugeJson                     | Base | 7,978.23 μs | 51.061 μs | 39.865 μs |  1.00 | Baseline        | 953.1250 |          - | 468.7500 | 218.7500 | 7093.56 KB |        1.00 |
| HugeJson                     | Diff | 6,708.48 μs | 29.766 μs | 26.387 μs |  0.84 | Faster          | 859.3750 |          - | 507.8125 | 234.3750 | 6213.92 KB |        0.88 |
|                              |      |             |           |           |       |                 |          |            |          |          |            |             |
| Article                      | Base |    84.18 μs |  0.323 μs |  0.287 μs |  1.00 | Baseline        |   9.8877 |          - |   0.3662 |        - |   81.75 KB |        1.00 |
| Article                      | Diff |    58.86 μs |  0.082 μs |  0.073 μs |  0.70 | Faster          |   7.7515 |          - |   0.2441 |        - |   63.38 KB |        0.78 |
|                              |      |             |           |           |       |                 |          |            |          |          |            |             |
| ArticleWithCustomSerializers | Base |    85.74 μs |  0.393 μs |  0.368 μs |  1.00 | Baseline        |  10.1318 |          - |   0.3662 |        - |   83.06 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |    59.23 μs |  0.108 μs |  0.096 μs |  0.69 | Faster          |   7.8735 |          - |   0.3052 |        - |   64.69 KB |        0.78 |

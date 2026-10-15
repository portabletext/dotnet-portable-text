Base: `06042151fa58` fix: call mark serializers once per mark
Diff: working tree on `06042151fa58` + uncommitted changes in src/

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
| HugeJson                     | Base | 12,112.4 μs | 51.53 μs | 40.23 μs |  1.00 | Baseline        | 1406.2500 |          - | 781.2500 | 234.3750 | 10834.73 KB |        1.00 |
| HugeJson                     | Diff |  9,956.4 μs | 31.43 μs | 29.40 μs |  0.82 | Faster          | 1156.2500 |          - | 562.5000 | 218.7500 |  8774.13 KB |        0.81 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| Article                      | Base |    140.2 μs |  0.32 μs |  0.27 μs |  1.00 | Baseline        |   17.3340 |          - |   0.7324 |        - |   143.21 KB |        1.00 |
| Article                      | Diff |    104.1 μs |  0.35 μs |  0.29 μs |  0.74 | Faster          |   13.9160 |          - |   0.4883 |        - |   114.56 KB |        0.80 |
|                              |      |             |          |          |       |                 |           |            |          |          |             |             |
| ArticleWithCustomSerializers | Base |    139.8 μs |  0.31 μs |  0.29 μs |  1.00 | Baseline        |   17.5781 |          - |   0.7324 |        - |   144.52 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |    104.6 μs |  0.19 μs |  0.16 μs |  0.75 | Faster          |   14.1602 |          - |   0.4883 |        - |   115.88 KB |        0.80 |

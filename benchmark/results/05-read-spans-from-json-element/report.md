Base: `ecb9c0b2adda` docs(benchmark): compare with origin/main instead of main
Diff: working tree on `ecb9c0b2adda` + uncommitted changes in src/

```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.402
  [Host] : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Base   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Diff   : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a


```
| Method                       | Job  | Mean         | Error      | StdDev     | Median       | Ratio | MannWhitney(3%) | RatioSD | Gen0      | Exceptions | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|----------------------------- |----- |-------------:|-----------:|-----------:|-------------:|------:|---------------- |--------:|----------:|-----------:|---------:|---------:|-----------:|------------:|
| HugeJson                     | Base | 10,425.77 μs | 205.388 μs | 433.234 μs | 10,209.52 μs |  1.00 | Baseline        |    0.06 | 1156.2500 |          - | 562.5000 | 218.7500 | 8774.28 KB |        1.00 |
| HugeJson                     | Diff |  8,721.83 μs |  20.548 μs |  19.221 μs |  8,727.16 μs |  0.84 | Faster          |    0.03 |  953.1250 |          - | 468.7500 | 218.7500 | 7093.58 KB |        0.81 |
|                              |      |              |            |            |              |       |                 |         |           |            |          |          |            |             |
| Article                      | Base |    106.28 μs |   0.472 μs |   0.442 μs |    106.30 μs |  1.00 | Baseline        |    0.01 |   13.9160 |          - |   0.4883 |        - |  114.56 KB |        1.00 |
| Article                      | Diff |     88.14 μs |   0.567 μs |   0.503 μs |     88.37 μs |  0.83 | Faster          |    0.01 |    9.8877 |          - |   0.3662 |        - |   81.75 KB |        0.71 |
|                              |      |              |            |            |              |       |                 |         |           |            |          |          |            |             |
| ArticleWithCustomSerializers | Base |    105.44 μs |   0.413 μs |   0.322 μs |    105.54 μs |  1.00 | Baseline        |    0.00 |   14.1602 |          - |   0.4883 |        - |  115.88 KB |        1.00 |
| ArticleWithCustomSerializers | Diff |     88.70 μs |   1.128 μs |   1.055 μs |     87.97 μs |  0.84 | Faster          |    0.01 |   10.1318 |          - |   0.3662 |        - |   83.06 KB |        0.72 |

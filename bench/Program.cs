using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Sate.Benchmark;

#if DEBUG
var config = new ManualConfig();
config.AddLogger(NullLogger.Instance);
config.AddExporter(new MarkdownConsoleExporter());
config.AddColumnProvider(DefaultColumnProviders.Instance);
config.WithOption(ConfigOptions.DisableOptimizationsValidator, true);
BenchmarkRunner.Run(typeof(Program).Assembly, config, args);
#else
var config = ManualConfig
    .Create(DefaultConfig.Instance)
    .WithOptions(ConfigOptions.DisableOptimizationsValidator);
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
#endif

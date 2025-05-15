using BenchmarkDotNet.Attributes;
using RazorEngine;
using RazorEngine.Templating;

namespace Sate.Benchmark;

public class GeneratorBench
{
    public GeneratorBench()
    {
        Engine.Razor.Compile(RazorSource, "RazorSource", typeof(Model));
    }

    [Benchmark(Description = "Sate(dynamic)")]
    public void Sate1Bench()
    {
        _ = SateTest.Build(new { Name = "John Does", Age = 10 });
    }

    [Benchmark(Description = "Sate(static)")]
    public void Sate2Bench()
    {
        _ = SateTest.Build(new() { Name = "John Does", Age = 10 });
    }

    [Benchmark(Description = "T4")]
    public void T4Bench()
    {
        _ = new T4Test { Name = "John Does", Age = 10 }.TransformText();
    }

    [Benchmark(Description = "Razor(Run)")]
    public void RazorRunBench()
    {
        _ = Engine.Razor.Run("RazorSource", typeof(Model), new Model { Name = "John Does", Age = 10 });
    }

    [Benchmark(Description = "Razor(RunCompile)")]
    public void RazorRunCompileBench()
    {
        _ = Engine.Razor.RunCompile(RazorSource, "templateKey", typeof(Model), new Model { Name = "John Does", Age = 10 });
    }

    public string RazorSource { get; } = @"
/*
    SQL文サンプル
*/
select *
from Dummy
where 1 = 1

@if (Model.Name != """")
{
@: and   Name = @@Name
}

@if (Model.Age >= 20)
{
@: and   Age = @@Age
}

order by
    Id,
    Name
";

    public class Model
    {
        public required string Name { get; init; }
        public required int Age { get; init; }
    }
}

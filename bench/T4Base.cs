using System.Text;

namespace Sate.Benchmark;

public abstract class T4Base
{
    public StringBuilder GenerationEnvironment { get; } = new();

    public void Write(string text)
    {
        GenerationEnvironment.Append(text);
    }

    public abstract string TransformText();
}

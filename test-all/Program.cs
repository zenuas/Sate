using System;

Console.WriteLine(Sate.TestAll.TestDir.Dummy.Build(new { Name = "John Does", Age = 20 }));

TestMethod.Test(new { Id = 1, Name = "ab" });
TestMethod.Test(new() { Id = 2, Name = "cd" });

public class TestMethod
{
    public static void Test(dynamic a)
    {
        Console.WriteLine("call Test(dynamic a)");
        Test(new TestClass { Id = a.Id, Name = a.Name });
    }

    public static void Test(TestClass a)
    {
        Console.WriteLine("Test(TestClass a)");
        Test((ITestClass)a);
    }

    public static void Test(ITestClass a)
    {
        Console.WriteLine("Test(ITestClass a)");
        Console.WriteLine(a.Id);
        Console.WriteLine(a.Name);
    }
}


public class TestClass : ITestClass
{
    public int Id { get; set; }
    public required string Name { get; set; }
}


public interface ITestClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}

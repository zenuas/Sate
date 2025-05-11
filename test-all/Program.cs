using System;

Sate.TestAll.TestDir.Dummy.Build(new { Id = 1 });

TestMethod.Test(new { Id = 1, Name = "ab" });
TestMethod.Test(new() { Id = 2, Name = "cd" });

public class TestMethod
{
    public static void Test(dynamic a)
    {
        Test(new TestClass { Id = a.Id, Name = a.Name });
    }

    public static void Test(TestClass a)
    {
        Console.WriteLine(a.Id);
        Console.WriteLine(a.Name);
    }
}


public class TestClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}

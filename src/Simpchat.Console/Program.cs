using Simpchat.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var person1 = new Person { Name = "David" };
        var person2 = new Person { Name = "David" };

        Console.WriteLine(person1 == person2);
    }
}
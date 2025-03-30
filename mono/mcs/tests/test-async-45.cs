using System;
using System.Threading.Tasks;

class MainClass
{
    public static void Main()
    {
        var task = Connect("a", "b", "c");
        task.Wait();
    }

    static async Task Connect(params string[] names)
    {
        foreach (var h in names)
        {
            try
            {
                await Task.Yield();
            }
            catch (Exception) { }
        }
    }
}

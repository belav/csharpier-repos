using System;
using System.Collections.Generic;
using System.Runtime;

public class DataContractSerializerTest
{
    public static void Main(string[] args)
    {
        var source = new List<string>();
        source.Add("a");
        source.Add("b");
        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(
                typeof(List<string>)
            );
            serializer.WriteObject(stream, source);
            stream.Flush();
        }
    }
}

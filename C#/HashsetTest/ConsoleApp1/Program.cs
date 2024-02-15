using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        HashSet<string> myHashSet = new HashSet<string>();

        // 測試加入不存在的字串
        AddIfNotExists(myHashSet, "apple");
        AddIfNotExists(myHashSet, "banana");
        AddIfNotExists(myHashSet, "apple");  // 已經存在，不會再次加入

        // 輸出集合中的字串
        Console.WriteLine("HashSet Contents:");
        foreach (var item in myHashSet)
        {
            Console.WriteLine(item);
        }
    }

    static void AddIfNotExists(HashSet<string> hashSet, string value)
    {
        if (!hashSet.Contains(value))
        {
            hashSet.Add(value);
            Console.WriteLine($"Added: {value}");
        }
        else
        {
            Console.WriteLine($"Already exists: {value}");
        }
    }
}

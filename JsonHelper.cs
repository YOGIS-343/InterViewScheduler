using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace InterViewScheduler
{
    public static class JsonHelper
{
    public static void SerializeToFile<T>(List<T> data, string filePath)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        File.WriteAllText(filePath, jsonData);
    }

    public static List<T> DeserializeFromFile<T>(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonData);
        }
        else
        {
            return new List<T>();
        }
    }
}

}

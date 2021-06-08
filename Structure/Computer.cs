using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;

namespace CafeSystem.Structure
{
    public class Computer
    {
        public Computer()
        {
            ConnectedDevices = new List<string>();
        }

        public string Name { get; set; }

        public List<string> ConnectedDevices { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string GetDeviceString()
        {
            var res = "";

            if (ConnectedDevices.Count == 0) return "";
            if (ConnectedDevices.Count == 1) return $"[{ConnectedDevices[0]}]";
            for (var i = 0; i < ConnectedDevices.Count; i++)
                if (i == 0) res += $"[{ConnectedDevices[i]}|";
                else if (i == ConnectedDevices.Count - 1) res += $"{ConnectedDevices[i]}]";
                else res += $"{ConnectedDevices[i]}|";

            return res;
        }

        public string GetFullString()
        {
            return ConnectedDevices.Count == 0 ? Name : $"{Name}:\n{GetDeviceString()}";
        }

        #region

        #endregion
    }

    public class JsonConverter<T> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return JsonConvert.SerializeObject(value);
        }
    }

    internal sealed class ComputerMap : ClassMap<Computer>
    {
        public ComputerMap()
        {
            Map(m => m.Name);
            Map(m => m.ConnectedDevices).Name("Devices").TypeConverter<JsonConverter<List<string>>>();
        }
    }
}
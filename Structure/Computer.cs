using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace CafeSystem.Structure
{
    class Computer
    {
        public string Name { get; set; }

        public List<string> ConnectedDevices { get; set; }

        public Computer()
        {
            ConnectedDevices = new List<string>();
        }

        #region 

        #endregion
        public override string ToString()
        {
            return Name;
        }

        public string GetDeviceString()
        {
            string res = "";

            if (ConnectedDevices.Count == 0) return "";
            if (ConnectedDevices.Count == 1) return $"[{ConnectedDevices[0]}]";
            for (int i = 0; i < ConnectedDevices.Count; i++)
            {
                if (i == 0) res += $"[{ConnectedDevices[i]}|";
                else if (i == ConnectedDevices.Count - 1) res += $"{ConnectedDevices[i]}]";
                else res += $"{ConnectedDevices[i]}|";
            }

            return res;
        }

        public string GetFullString()
        {
            return ConnectedDevices.Count == 0 ? Name : $"{Name}:\n{GetDeviceString()}";
        }
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

    sealed class ComputerMap : ClassMap<Computer>
    {
        public ComputerMap()
        {
            Map(m => m.Name);
            Map(m => m.ConnectedDevices).Name("Devices").TypeConverter<JsonConverter<List<string>>>();
        }
    }
}

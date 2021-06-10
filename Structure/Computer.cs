using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;

namespace CafeSystem.Structure
{
    public class Computer
    {
        /// <summary>
        /// ПК забронирован?
        /// </summary>
        internal bool Reserved;

        /// <summary>
        /// Данные о брони пк.
        /// </summary>
        internal Reservation Reservation;

        /// <summary>
        /// Текущий пользователь пк
        /// </summary>
        internal User User;

        /// <summary>
        /// Цена за час бронирования пк
        /// </summary>
        internal double PricePerHour = 0;

        public Computer()
        {
            ConnectedDevices = new List<string>();
        }

        /// <summary>
        /// Имя ПК
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Подключенные устройства к этому пк
        /// </summary>
        public List<string> ConnectedDevices { get; set; }

        #region Работа со строками/строковыми форматами
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Вывод всех подключенных устройств к ПК
        /// </summary>
        /// <returns>список подключенных устройств</returns>
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
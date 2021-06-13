using CsvHelper.Configuration;
using System;

namespace CafeSystem.Structure
{
    public class User
    {
        /// <summary>
        ///     Уровень доступа к данным в проекте
        /// </summary>
        public enum Permissions
        {
            Owner, // Владелец (если вдруг появится соучредитель, полезно).
            Admin, // Администратор.
            User // Обычный пользователь. 
        }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ID пользователя
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     Сумма времени бронирования.
        /// </summary>
        public double VisitedTime { get; set; } = 0;

        /// <summary>
        ///     Уровень доступа пользователя
        /// </summary>
        public Permissions Perms { get; set; } = Permissions.User;

        public override string ToString()
        {
            return $"{Name} -> {Perms}";
        }

        public string PermToStr()
        {
            switch (Perms)
            {
                case Permissions.Owner:
                    return "Владелец";
                case Permissions.Admin:
                    return "Администратор";
                case Permissions.User:
                    return "Пользователь";
            }
            return "Не указано";
        }
    }

    internal sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.UserId).Name("id");
            Map(m => m.Name).Name("username");
            Map(m => m.VisitedTime).Name("hours");
            Map(m => m.Perms).Name("permissions");
        }
    }
}
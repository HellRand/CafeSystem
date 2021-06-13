using CsvHelper.Configuration;

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
        public double VisitedTime { get; set; }

        /// <summary>
        ///     Уровень доступа пользователя
        /// </summary>
        public Permissions Perms { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, UserId: {UserId}, VisitedTime: {VisitedTime}, Perms: {Perms}";
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
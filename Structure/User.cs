using CsvHelper.Configuration;

namespace CafeSystem.Structure
{
    public class User
    {
        public enum Permissions
        {
            Owner,
            Admin,
            User
        }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ID пользователя
        /// </summary>
        public int UserId { get; set; }
    }

    internal sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.UserId).Name("id");
            Map(m => m.Name).Name("username");
        }
    }
}
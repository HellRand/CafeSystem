using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;

namespace CafeSystem.Structure
{
    class User
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID пользователя
        /// </summary>
        public int UserId { get; set; }
        
        public enum Permissions
        {
            Owner,
            Admin,
            User
        }
    }

    sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.UserId).Name("id");
            Map(m => m.Name).Name("username");
        }
    }
}

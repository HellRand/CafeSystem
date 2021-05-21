using System;
using System.Collections.Generic;
using System.Text;

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
}

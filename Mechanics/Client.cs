using System.Collections.Generic;
using CafeSystem.Structure;

namespace CafeSystem.Mechanics
{
    public abstract class Client
    {
        public abstract List<Computer> Computers { get; set; }

        public abstract List<User> Users { get; set; }

        public abstract List<Reservation> Reservations { get; set; }

        public delegate void NotifyHandler(Computer computer, string message);

        public event NotifyHandler On_Message;

        public Client(List<Computer> pcs, List<User> usrs)
        {
            Computers = pcs;
            Users = usrs;
        }

        public bool ReserveComputer(Computer pc, Reservation res)
        {
            if (pc == null || res == null) throw new System.Exception("Выберите пк и время резервации!");
            
            if (pc.Reserved)
            {
                On_Message(pc, "Этот компьютер занят!");
                return false;
            }

            pc.Reservation = res;
            pc.Reserved = true;

            On_Message(pc, $"Компьютер \"{pc.Name}\" забронирован успешно!");
            return true;
        }

        internal User GetUserById(int id)
        {
            return Users.Find(m => m.UserId == id);
        }

        
    }
}
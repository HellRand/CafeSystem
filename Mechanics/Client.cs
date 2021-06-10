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

        public void ReserveComputer(Computer pc, Reservation res)
        {
            if (pc == null || res == null) throw new System.Exception("В конструкторе пустой объект!");
            
            if (pc.Reserved)
            {
                On_Message(pc, "Этот компьютер занят!");
                return;
            }

           
        }
    }
}
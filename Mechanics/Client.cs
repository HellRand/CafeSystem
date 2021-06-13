using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CafeSystem.Structure;
using CsvHelper;

namespace CafeSystem.Mechanics
{
    public abstract class Client
    {
        public abstract List<Computer> Computers { get; set; }

        public abstract List<User> Users { get; set; }

        public abstract List<Reservation> Reservations { get; set; }

        #region Handlers & Events

        public delegate void NotifyHandler(Computer computer, string message);

        public delegate void UserHandler(User user);

        public delegate void ComputerHandler(Computer computer);

        public event NotifyHandler On_Message;
        public event UserHandler On_UserAdded;

        #endregion

        public Client(List<Computer> pcs, List<User> users)
        {
            Computers = pcs;
            Users = users;
        }

        public bool ReserveComputer(Computer pc, Reservation res)
        {
            if (pc == null || res == null)
            {
                On_Message?.Invoke(null, "Ошибка выбора пк или бронирования");
                return false;
            }

            if (pc.Reserved)
            {
                On_Message?.Invoke(pc, "Этот компьютер занят!");
                return false;
            }

            pc.Reservation = res; //Объявляем бронирование
            pc.Reserved = true; //отмечаем ПК как забронированый

            res.On_ReservationEnded += PC_On_ReservationEnd; //Подписываемся на событие, когда резервация закончится

            On_Message?.Invoke(pc, $"Компьютер \"{pc.Name}\" забронирован успешно!");
            return true;
        }

        private void PC_On_ReservationEnd(Reservation reservation, Computer pc)
        {
            pc.Reservation = null;
            pc.Reserved = false;
            pc.User.VisitedHours += reservation.Duration.TotalSeconds / 3600;
        }

        internal User GetUserById(int id)
        {
            return Users.Find(m => m.UserId == id);
        }

        public async void SaveUserData()
        {
            using (var writer = new StreamWriter("Users.csv"))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<UserMap>();

                await csv.WriteRecordsAsync(Users);
                LogBox.Log("Список пользователей сохранён успешно!", LogBox.LogType.Succes);
            }
        }
    }
}
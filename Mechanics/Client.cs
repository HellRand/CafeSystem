using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CafeSystem.Structure;
using CsvHelper;

namespace CafeSystem.Mechanics
{
    public abstract class Client
    {
        public Client(List<Computer> pcs, List<User> users)
        {
            Computers = pcs;
            Users = users;
        }

        public List<Computer> Computers { get; set; }

        public List<User> Users { get; set; }

        public List<Reservation> Reservations { get; set; }
        public abstract List<Computer> GetFreePCs();

        public bool ReserveComputer(Computer pc, Reservation res)
        {
            if (pc == null || res == null)
            {
                OnMessage?.Invoke(null, "Ошибка выбора пк или бронирования");
                return false;
            }

            if (pc.Reserved)
            {
                OnMessage?.Invoke(pc, "Этот компьютер занят!");
                return false;
            }

            pc.Reservation = res; //Объявляем бронирование
            pc.Reserved = true; //отмечаем ПК как забронированый

            res.On_ReservationEnded += PC_On_ReservationEnd; //Подписываемся на событие, когда резервация закончится

            OnMessage?.Invoke(pc, $"Компьютер \"{pc.Name}\" забронирован успешно!");
            return true;
        }

        private void PC_On_ReservationEnd(Reservation reservation, Computer pc)
        {
            pc.Reservation = null;
            pc.Reserved = false;
            pc.User.VisitedTime += reservation.Duration.TotalSeconds;
        }

        /// <summary>
        ///     Выполняет поиск пользователя по его Id =>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal User GetUserById(long id)
        {
            return Users.Find(m => m.UserId == id);
        }

        /// <summary>
        ///     Сохранить базу данных пользователей.
        ///     <remarks>Не затрагивает основной поток</remarks>
        /// </summary>
        public async void SaveUserData()
        {
            using (var writer = new StreamWriter("Users.csv"))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<UserMap>();

                await csv.WriteRecordsAsync(Users);
                LogBox.Log("Список пользователей сохранён успешно!", LogBox.LogType.Success);
            }
        }

        #region Handlers & Events

        public delegate void NotifyHandler(Computer computer, string message);

        public delegate void UserHandler(User user);

        public delegate void ComputerHandler(Computer computer);

        public event NotifyHandler OnMessage;
        public event UserHandler OnUserAdded;

        #endregion
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CafeSystem.Structure
{
    public class Reservation
    {
        public delegate void ReservationHandler(Reservation reservation);

        public Reservation(TimeSpan duration)
        {
            Duration = duration;
            StartTime = DateTime.Now;
            EndTime = StartTime + Duration;
            Remaining = Duration;

            CheckLoop();
        }
        
        public Reservation(DateTime from, TimeSpan duration)
        {
            Duration = duration;
            StartTime = from;
            EndTime = StartTime + Duration;
            Remaining = Duration;

            CheckLoop();
        }

        /// <summary>
        /// Срабатывает, когда бронь начинает действовать
        /// </summary>
        public event ReservationHandler On_ReservationStarted;

        /// <summary>
        /// Срабатывает, когда бронь заканчивает своё действие
        /// </summary>
        public event ReservationHandler On_ReservationEnded;

        private async Task CheckLoop()
        {
            await Task.Run(() =>
            {
                Status = Status.Waiting;
                while (StartTime > DateTime.Now) Thread.Sleep(1000);

                On_ReservationStarted?.Invoke(this);

                Status = Status.Active;
                while (DateTime.Now < EndTime) Thread.Sleep(1000);

                On_ReservationEnded?.Invoke(this);
            });
        }

        //public async void Start()
        //{
        //    await CheckLoop();
        //}
        
        #region Всё что касается непосредственно брони

        /// <summary>
        ///     Время, в которое бронь начала действовать
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Длительность брони
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Время, в которое бронь заканчивается
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     Оставшееся время брони
        /// </summary>
        public TimeSpan Remaining { get; set; }

        /// <summary>
        /// Текущий статус брони
        /// </summary>
        public Status Status { get; private set; }

        #endregion

        public override string ToString()
        {
            return $"Время бронирования: [{StartTime.ToString("HH:mm:ss")} - {EndTime.ToString("HH:mm:ss")}]\n" +
                   $"Длительность брони: {Duration.ToString()}";
        }
    }

    public enum Status
    {
        Waiting,
        Active
    }
}
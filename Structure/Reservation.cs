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
        }

        public event ReservationHandler On_ReservationStarted;
        public event ReservationHandler On_ReservationEnded;

        private async void CheckLoop()
        {
            On_ReservationStarted?.Invoke(this);
            await Task.Run(() =>
            {
                var currentTime = DateTime.Now;
                while (StartTime >= currentTime && currentTime < EndTime) Thread.Sleep(1000);
            });
        }

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

        #endregion
    }
}
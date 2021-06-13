using System;
using System.Collections.Generic;
using System.Linq;
using CafeSystem.Mechanics;
using CafeSystem.Structure;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CafeSystem.Clients
{
    internal sealed class TelegramBot : Client
    {
        #region SecretKey

        /// <summary>
        ///     Токен
        /// </summary>
        private readonly TelegramBotClient TgClient =
            new TelegramBotClient("1800173075:AAFy3ZjXgfPpTCbfJzhLJo10M7za2zANxAc");

        #endregion

        public TelegramBot(List<Computer> pcs, List<User> users) : base(pcs, users)
        {
            Computers = pcs;
            Users = users;
            TgClient.OnMessage += TgClientOnMessage;
            TgClient.OnCallbackQuery += TgClientOnCallbackQuery;
            OnMessage += OnClientUpdate;
            
            TgClient.StartReceiving();
            LogBox.Log("Telegram клиент запущен!\n" +
                       "(при отсутствии интернет соединения telegram клиент будет в режиме ожидания)");
        }

        private void OnClientUpdate(Computer computer, string message)
        {
            if (computer != null && computer.User != null) TgClient.SendTextMessageAsync(computer.User.UserId, message);
        }

        private void TgClientOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            string msg = e.CallbackQuery.Data;
            
            switch (msg)
            {
                case "PCS":
                    var freePCs = GetFreePCs();
                    var buttons = new List<InlineKeyboardButton[]>();
                    
                    for (var i = 0; i < freePCs.Count; i++)
                    {
                        buttons.Add(new[] {InlineKeyboardButton.WithCallbackData(freePCs[i].Name)});
                    }
                    var keyboard = new InlineKeyboardMarkup(buttons);

                    TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "Выберите любой ПК:", ParseMode.Default, false, false, 0, keyboard);
                    return;
                default:
                    
                    break;
            }

            var selectedPc = GetComputerByName(msg);
            User curUser = GetUserById(e.CallbackQuery.Message.Chat.Id);
            if (selectedPc != null && curUser != null)
            {
                
                Reservation reservation = new Reservation(TimeSpan.FromSeconds(5));

                selectedPc.User = curUser;
                reservation.User = curUser;

                reservation.ReservedPC = selectedPc;
                TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, $"Вы забронировали компьютер: \"{selectedPc}\".\n" + 
                                                                       $"Детали брони: {reservation}.");
                reservation.On_ReservationEnded += (res, pc) =>
                {
                    TgClient.SendTextMessageAsync(pc.User.UserId, $"Время бронирования истекло.\nСуммарное время бронирования, всего: {pc.User.VisitedTime}сек.");
                };
                ReserveComputer(selectedPc, reservation);
            } else { TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "Вероятно, что-то пошло не так. Введите /start чтобы попробовать ещё раз."); }
            
        }

        private void TgClientOnMessage(object sender, MessageEventArgs e)
        {
            var username = e.Message.From.Username;
            var userId = e.Message.Chat.Id;

            var currentUser = GetUserById(userId);
            if (currentUser != null && currentUser.Name == String.Empty) currentUser.Name = e.Message.From.Username;

            LogBox.Log($"Сообщение (тип: {e.Message.Type.ToString().ToLower()}) от {username}: \"{e.Message.Text}\"");
            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    
                    if (currentUser == null) //Если кто-то пишет впервые - записать в бд
                    {
                        var user = new User();
                        user.Name = e.Message.From.Username;
                        user.Perms = User.Permissions.User;
                        user.UserId = userId;
                        user.VisitedTime = 0;
                        Users.Add(user);
                        SaveUserData();

                        TgClient.SendTextMessageAsync(e.Message.Chat.Id, $"Привет, {username}!\nТвой ID: {userId}.");
                    }
                    else
                    {
                        TgClient.SendTextMessageAsync(e.Message.Chat.Id, $"Вы уже зарегистрированы в системе, {username}.\nВаш идентификатор: {userId}.");
                    }

                    break;
                default:
                    TgClient.SendTextMessageAsync(e.Message.Chat.Id, "Неизвестная команда!",
                        ParseMode.Markdown, false, false, e.Message.MessageId,
                        new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData($"Кол-во доступных ПК {GetFreePCs().Count}", "PCS")));
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CafeSystem.Mechanics;
using CafeSystem.Structure;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CafeSystem.Clients
{
    public class TelegramBot : Client
    {
        #region SecretKey

        /// <summary>
        ///     Токен
        /// </summary>
        public TelegramBotClient TgClient { get; private set; } = new TelegramBotClient("1800173075:AAFy3ZjXgfPpTCbfJzhLJo10M7za2zANxAc");

        #endregion

        public TelegramBot(List<Computer> pcs, List<Structure.User> users) : base(pcs, users)
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

                    TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "Выберите любой компьютер:", ParseMode.Default, false, false, 0, keyboard);
                    return;
                default:
                    break;
            }

            var selectedPc = GetComputerByName(msg);
            Structure.User curUser = GetUserById(e.CallbackQuery.Message.Chat.Id);
            if (selectedPc != null && curUser != null)
            {
                if (selectedPc.Reserved)
                {
                    TgClient.SendTextMessageAsync(curUser.UserId, $"Просим прощения, но \"{selectedPc.Name}\" уже зарезервирован.\n Попробуйте выбрать другой пк.");
                    return;
                }
                Reservation reservation = new Reservation(TimeSpan.FromSeconds(30), selectedPc);
                selectedPc.User = curUser;
                reservation.User = curUser;

                ReserveComputer(selectedPc, reservation);
                TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, $"Вы забронировали компьютер: \"{selectedPc}\".\n" + 
                                                                       $"Детали брони: {reservation}.");
                reservation.On_ReservationEnded += (res, pc) =>
                {
                    TgClient.SendTextMessageAsync(pc.User.UserId, $"Время бронирования \"{pc.Name}\" истекло.\nСуммарное время бронирования, всего: {Math.Round(pc.User.VisitedTime / 60, 1)}мин.");
                };
                
            } else { TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "Вероятно, что-то пошло не так. Введите /start чтобы попробовать ещё раз."); }
            
        }

        private void TgClientOnMessage(object sender, MessageEventArgs e)
        {
            string username;
            if (e.Message.From.Username != null) username = e.Message.From.Username;
            else username = $"{e.Message.From.FirstName} {e.Message.From.LastName}";


            var userId = e.Message.Chat.Id;

            var currentUser = GetUserById(userId);
            if (currentUser == null) //Если кто-то пишет впервые - записать в бд
            {
                var user = new Structure.User();
                user.Name = username;
                user.Perms = Structure.User.Permissions.User;
                user.UserId = userId;
                user.VisitedTime = 0;
                Users.Add(user);
                SaveUserData();
            }
            else currentUser.Name = username; //Перезаписать имя пользователя, если тот его изменил
            

                LogBox.Log($"Сообщение (тип: {e.Message.Type.ToString().ToLower()}) от {username}: \"{e.Message.Text}\"");
            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    
                    if (currentUser == null) //Если кто-то пишет впервые - записать в бд
                    {
                        var user = new Structure.User();
                        user.Name = username;
                        user.Perms = Structure.User.Permissions.User;
                        user.UserId = userId;
                        user.VisitedTime = 0;
                        Users.Add(user);
                        SaveUserData();

                        TgClient.SendTextMessageAsync(userId, $"Привет, {username}!\nТвой ID: {userId}.");
                    }
                    else
                    {
                        TgClient.SendTextMessageAsync(userId, $"Вы уже зарегистрированы в системе, {username}.\nВаш идентификатор: {userId}.");
                    }

                    break;
                case "/pcs":
                    int pc_count = GetFreePCs().Count;
                    if (pc_count == 0)
                    {
                        TgClient.SendTextMessageAsync(userId, "Увы, но все компьютеры заняты, повторите попытку чуть позднее.\nПросим прощения за причиненные неудобства.");
                        return;
                    }
                    TgClient.SendTextMessageAsync(userId, "Выберите любой ПК для бронирования:",
                        ParseMode.Markdown, false, false, e.Message.MessageId,
                        new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData($"Забронировать компьютер (свободных ПК {GetFreePCs().Count})", "PCS")));
                    break;
                case "/me":
                    TgClient.SendTextMessageAsync(userId, $"Инфо о {currentUser.Name}\n" +
                        $"Ваш ID: {currentUser.UserId}\n" +
                        $"Общее время бронирования: {Math.Round(currentUser.VisitedTime/60, 1)} мин.\n" +
                        $"Уровень доступа: {currentUser.PermToStr()}");
                    if (currentUser.Perms == Structure.User.Permissions.Owner)
                    {
                        int freePcs = GetFreePCs().Count;
                        TgClient.SendTextMessageAsync(userId, $"Свободных ПК: {freePcs}\n" +
                                                              $"Зарезервированных ПК: {Computers.Count - freePcs}");
                        var reserved = Computers.Where(m => m.Reserved == true).ToList();
                        string show = "";
                        for (int i = 0; i < reserved.Count; i++)
                        {
                            show += $"{i+1}) {reserved[i].Name} забронирован пользователем {reserved[i].User.Name}.\n" +
                                    $"Данные о брони: {reserved[i].Reservation}\n" +
                                    $"Цена услуги: {Math.Round(reserved[i].PricePerHour * reserved[i].Reservation.Duration.TotalHours, 2)}руб.\n\n";
                        }
                        TgClient.SendTextMessageAsync(userId, show);
                    }
                    break;
                default:
                    
                    break;
            }
        }
    }
}
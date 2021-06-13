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
            TgClient.StartReceiving();
            LogBox.Log("Telegram клиент запущен!\n" +
                       "(при отсутствии интернет соединения telegram клиент будет в режиме ожидания)");
        }

        private void TgClientOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            switch (e.CallbackQuery.Data)
            {
                case "PCS":
                    var freePCs = GetFreePCs();
                    var buttons = new InlineKeyboardButton[freePCs.Count];
                    for (var i = 0; i < buttons.Length; i++)
                        buttons[i] = InlineKeyboardButton.WithCallbackData(freePCs[i].Name);

                    TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "Выберите любой ПК:",
                        ParseMode.Default, false, false, 0,
                        new InlineKeyboardMarkup(buttons));
                    break;
                default:
                    TgClient.SendTextMessageAsync(e.CallbackQuery.From.Id, "???");

                    TgClient.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);
                    break;
            }
        }

        private void TgClientOnMessage(object sender, MessageEventArgs e)
        {
            var username = e.Message.From.Username;
            var userId = e.Message.Chat.Id;

            LogBox.Log(
                $"Получено {e.Message.Type.ToString().ToLower()} от {username}: \"{e.Message.Text}\"");
            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    if (GetUserById(userId) == null) //Если кто-то пишет впервые - записать в бд
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
                        TgClient.SendTextMessageAsync(e.Message.Chat.Id,
                            $"Вы уже зарегистрированы в системе, {username}.\nВаш идентификатор: {userId}.");
                    }

                    break;
                default:
                    TgClient.SendTextMessageAsync(e.Message.Chat.Id, "Незнакомая команда!",
                        ParseMode.Markdown, false, false, e.Message.MessageId,
                        new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData($"Кол-во доступных ПК {GetFreePCs().Count}", "PCS")));
                    break;
            }
        }

        public override List<Computer> GetFreePCs()
        {
            return Computers.Where(m => m.Reserved == false).ToList();
        }
    }
}
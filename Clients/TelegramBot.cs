using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using CafeSystem.Mechanics;
using System.Collections.Generic;
using CafeSystem.Structure;

namespace CafeSystem.TelegramClient
{
    internal class TelegramBot : Client
    {
        #region SecretKey

        /// <summary>
        ///     Токен
        /// </summary>
        public TelegramBotClient client = new TelegramBotClient("1800173075:AAFy3ZjXgfPpTCbfJzhLJo10M7za2zANxAc");

        #endregion

        public TelegramBot(List<Computer> pcs, List<User> users) : base(pcs, users)
        {
            Computers = pcs;
            Users = users;
            client.OnMessage += Client_OnMessage;
            client.OnCallbackQuery += Client_OnCallbackQuery;
            client.StartReceiving();
            LogBox.Log("Telegram клиент запущен!\n" +
                       "(при отсутствии интернет соединения telegram клиент будет в режиме ожидания)");
        }

        public override List<Computer> Computers { get; set; }
        public override List<User> Users { get; set; }
        public override List<Reservation> Reservations { get; set; }

        private void Client_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            

            switch (e.CallbackQuery.Data)
            {
                default:
                    client.SendTextMessageAsync(e.CallbackQuery.From.Id, "???");
                    client.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);
                    break;
            }
        }

        private void Client_OnMessage(object sender, MessageEventArgs e)
        {
            LogBox.Log(
                $"Получено {e.Message.Type.ToString().ToLower()} от {e.Message.From.Username}: \"{e.Message.Text}\"");
            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    client.SendTextMessageAsync(e.Message.Chat.Id,
                        $"Привет, {e.Message.From.Username}!\nТвой ID: {e.Message.From.Id}.");
                    break;
                default:
                    client.SendTextMessageAsync(e.Message.Chat.Id, "Незнакомая команда!",
                        ParseMode.Markdown, false, false, e.Message.MessageId,
                        new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData($"{e.Message}", "pepega")));
                    break;
            }
        }
    }
}
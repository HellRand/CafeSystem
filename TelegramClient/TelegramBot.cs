using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CafeSystem.TelegramClient
{
    internal class TelegramBot
    {
        #region SecretKey

        /// <summary>
        ///     Токен
        /// </summary>
        public TelegramBotClient client = new TelegramBotClient("1800173075:AAFy3ZjXgfPpTCbfJzhLJo10M7za2zANxAc");

        #endregion

        public TelegramBot()
        {
            client.OnMessage += Client_OnMessage;
            client.StartReceiving();

            if (client.IsReceiving)
            {
                LogBox.Log("Telegram клиент подключен.", LogBox.LogType.Warning);
            }
            else
            {
                LogBox.Log("Проверьте подключение к интернету!" +
                           "\nСистема подключится к Telegram клиенту автоматически, " +
                           "как только появится интернет соединение.", LogBox.LogType.Error);
                Reconnect();
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

        private async void Reconnect()
        {
            await Task.Run(() =>
            {
                var i = 0;
                while (!client.IsReceiving)
                {
                    if (++i % 5 == 0) LogBox.Log($"Попытка переподключения Telegram клиента... ({i})");
                    client.StartReceiving();
                    Thread.Sleep(5000);
                }

                LogBox.Log("Telegram клиент подключен успешно!", LogBox.LogType.Succes);
            });
        }
    }
}
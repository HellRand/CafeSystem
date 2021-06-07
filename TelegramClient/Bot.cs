using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CafeSystem.TelegramClient
{
    class Bot
    {
        #region SecretKey
        /// <summary>
        /// Токен
        /// </summary>
        public TelegramBotClient client = new TelegramBotClient("1800173075:AAFy3ZjXgfPpTCbfJzhLJo10M7za2zANxAc");
        #endregion
        
        public Bot()
        {
            client.OnMessage += Client_OnMessage;
            client.StartReceiving();
        }

        private void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Привет, {e.Message.From.Username}!\nТвой ID: {e.Message.From.Id}.");
                    break;
                default:
                    
                    break;
            }
            
        }
    }
}

namespace AspireBudgetTelegramBot.Options
{
    public class TelegramOptions
    {
        public string ApiToken { get; set; }
        public string Password { get; set; }
        
        /// <summary>
        /// True if tor is needed. Will use proxy at localhost:9050 
        /// </summary>
        public bool UseTorProxy { get; set; }
    }
}

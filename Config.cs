// Config.cs

namespace Catatonia
{
    public static class Config
    {
        /// <summary>
        /// Путь к серверу
        /// </summary>
        public static string serverUrl;
        public static string serverUrl2;

        static Config()
        {
            serverUrl = "http://192.168.1.199:5074/api/FillField/index";
            serverUrl2 = "http://192.168.1.199:5074/api/FillField/update";
        }
    }
}

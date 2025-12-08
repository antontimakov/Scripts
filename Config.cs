// Config.cs
using System;

namespace Catatonia
{
    public static class Config
    {
        /// <summary>
        /// Путь к серверу
        /// </summary>
        public static string serverUrl;

        static Config()
        {
            serverUrl = "http://192.168.1.199:5074/getdb";
        }
    }
}

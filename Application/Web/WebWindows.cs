// Windows.cs
using System;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

using Catatonia;
using Catatonia.Application.Models;

namespace Catatonia.Application.Web
{
    public class WebWindows
    {
        /// <summary>
        /// Хранит дату и время для результата
        /// </summary>
        DateTime result1;

        /// <summary>
        /// Ссылка на основнное статическое поле для ввода
        /// </summary>
        TextMeshProUGUI mIf;

        public async void getResult(DateTime re1, TextMeshProUGUI mIf1)
        {
            mIf = mIf1;
            result1 = re1;
            using HttpClient client = new();
            using HttpResponseMessage result = await client.GetAsync(Config.serverUrl);
            resultToResultClass(await result.Content.ReadAsStringAsync());
        }

        // TODO УБРАТЬ ОТСЮДА
        /// <summary>
        /// Устанавливает полученное с сервера значение в поле
        /// </summary>
        private void resultToResultClass(String resultFromServer)
        {
            ResultClass myObject;
            myObject = JsonUtility.FromJson<ResultClass>(resultFromServer);
            mIf.text = myObject.time_fishing;
            string format = "yyyy-MM-dd\\THH:mm:ss.fffff";
            result1 = DateTime.ParseExact(myObject.time_fishing, format, CultureInfo.InvariantCulture);
        }
    }
}

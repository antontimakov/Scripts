// Windows.cs
using System;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

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
            /*mIf = mIf1;
            result1 = re1;
            using HttpClient client = new();
            using HttpResponseMessage result = await client.GetAsync(Config.serverUrl);
            resultToResultClass(await result.Content.ReadAsStringAsync());*/
        }

        // TODO УБРАТЬ ОТСЮДА
        /// <summary>
        /// Устанавливает полученное с сервера значение в поле
        /// </summary>
        private void resultToResultClass(String resultFromServer)
        {
            /*ResultClass myObject;
            myObject = JsonUtility.FromJson<ResultClass>(resultFromServer);
            mIf.text = myObject.time_fishing;
            string format = "yyyy-MM-dd\\THH:mm:ss.fffff";
            result1 = DateTime.ParseExact(myObject.time_fishing, format, CultureInfo.InvariantCulture);*/
        }
        public IEnumerator PostRequest(string url, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                // Устанавливаем заголовки
                www.SetRequestHeader("Content-Type", "application/json");

                // Преобразуем данные в байты
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();

                // Отправляем запрос
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke(www.downloadHandler.text);
                }
                else
                {
                    onError?.Invoke(www.error);
                }
            }
        }
    }
}

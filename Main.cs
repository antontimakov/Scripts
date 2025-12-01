// Main.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System;
using System.Globalization;
using System.Diagnostics;
using MyProject.Models;

public class Main : MonoBehaviour
{
    /// <summary>
    /// Ссылка на основнное статическое поле для ввода
    /// </summary>
    InputField mIf;

    /// <summary>
    /// Ссылка на основную кнопку
    /// </summary>
    Button mB;

    /// <summary>
    /// Хранит дату и время для результата
    /// </summary>
    DateTime result1;

    /// <summary>
    /// Экземпляр класса DragAndDrop
    /// </summary>
    DragAndDrop dad;

    /// <summary>
    /// (Unity) Выполняет действия до обновления первого кадра
    /// </summary>
    void Start()
    {
        initVars();
        drowField();
    }

    /// <summary>
    /// Инициализация переменных
    /// </summary>
    private void initVars()
    {
        dad = new DragAndDrop(this);
        mIf = GameObject.Find("MainIf").GetComponent<InputField>();
        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mB.onClick.AddListener(startSetText);
        result1 = DateTime.Now;
    }

    /// <summary>
    /// Отрисовка поля
    /// </summary>
    private void drowField()
    {
        reppeatRight(GameObject.Find("grass"), 10);
        reppeatRight(GameObject.Find("ground"), 10);
    }

    /// <summary>
    /// Заменяет указанный объект новым
    /// </summary>
    /// <param name="goNew">Заменяющий объект</param>
    /// <param name="goOld">Заменяемый объект</param>
    public void chObj(GameObject goNew, GameObject goOld)
    {
        Instantiate(goNew, goOld.transform.position, Quaternion.identity);
        Destroy(goOld);
    }

    /// <summary>
    /// Повторяет переданный объект указанное кол-во раз
    /// </summary>
    /// <param name="go">Объект для повторения</param>
    /// <param name="nom">кол-во повторений</param>
    public void reppeatRight(GameObject go, int nom)
    {
        Vector2 offset = go.transform.position;
        for (int i = 0; i < nom; i++)
        {
            Vector2 offset2 = offset;
            offset2.x = offset.x + i * 2;
            Instantiate(go, offset2, Quaternion.identity);
        }
    }

    /// <summary>
    /// (Unity) Вызывается при каждом обновлении кадра
    /// </summary>
    void Update()
    {
        DateTime localDate = DateTime.Now;
        TimeSpan deltaTime = result1 - localDate;
        mIf.text = deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
        dad.Action();
    }

    /// <summary>
    /// Запускает заполнение текстового поля в зависимости от платформы
    /// </summary>
    public void startSetText()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            StartCoroutine(GetText());
        }
        else
        {
            getFromServerWin();
        }
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = new UnityWebRequest("http://192.168.1.199:5074/getdb");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            mIf.text = www.error;
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);
            resultToResultClass(www.downloadHandler.text);

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }

    /// <summary>
    /// Получает данные с сервера (Windows)
    /// </summary>
    private async void getFromServerWin()
    {
        using HttpClient client = new();
        using HttpResponseMessage result = await client.GetAsync("http://192.168.1.199:5074/getdb");
        resultToResultClass(await result.Content.ReadAsStringAsync());
    }

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
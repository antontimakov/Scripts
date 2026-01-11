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
using TMPro;

using Catatonia;
using Catatonia.Application;
using Catatonia.Application.Models;

public class Main : MonoBehaviour
{
    /// <summary>
    /// Ссылка на основнное статическое поле для ввода
    /// </summary>
    TextMeshProUGUI mIf;

    /// <summary>
    /// Ссылка на основную кнопку
    /// </summary>
    Button mB;

    /// <summary>
    /// Ссылка выбранный спрайт
    /// </summary>
    Sprite sCurrent;

    /// <summary>
    /// Хранит дату и время для результата
    /// </summary>
    //DateTime result1;
    //string result2;

    /// <summary>
    /// Экземпляр класса DragAndDrop
    /// </summary>
    DragAndDrop dad;

    /// <summary>
    /// Экземпляр класса GameField
    /// </summary>
    GameField gameFieldObj;

    /// <summary>
    /// Экземпляр класса Shop
    /// </summary>
    Shop shopObj;

    /// <summary>
    /// Экземпляр класса ActiveItem
    /// </summary>
    ActiveItem activeItemObj;

    /// <summary>
    /// Экземпляр класса Zoom
    /// </summary>
    Zoom zoomObj;

    public WebServer serverObj;

    /// <summary>
    /// (Unity) Выполняет действия до обновления первого кадра
    /// </summary>
    void Start()
    {
        initVars();
        activeItemObj.hideActiveItem();

        mB.onClick.AddListener(shopObj.showShop);
    }

    /// <summary>
    /// Инициализация переменных
    /// </summary>
    private void initVars()
    {

        Sprite sGrassLocal = Resources.Load<Sprite>("Sprites/grass");

        dad = new(this);
        serverObj = new();
        gameFieldObj = new(this, serverObj);
        activeItemObj = new(this)
        {
            sGrass = sGrassLocal
        };
        shopObj = new(this)
        {
            activeItemObj = this.activeItemObj,
            sGrass = sGrassLocal
        };
        zoomObj = new(this);

        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mIf = GameObject.Find("MainText").GetComponent<TextMeshProUGUI>();

        //result1 = DateTime.Now;
    }

    /// <summary>
    /// Копирует указанный объект в указанную позицию
    /// </summary>
    /// <param name="oldObj"></param>
    /// <param name="position"></param>
    public void mainCopyObj(GameObject oldObj, Vector2 position)
    {
        Instantiate(oldObj, position, Quaternion.identity);
    }

    /// <summary>
    /// Заменяет указанный объект новым
    /// </summary>
    /// <param name="newObj">Заменяющий объект</param>
    /// <param name="oldObj">Заменяемый объект</param>
    public void mainChangeObj(GameObject newObj, GameObject oldObj)
    {
        Instantiate(newObj, oldObj.transform.position, Quaternion.identity);
        Destroy(oldObj);
    }

    /// <summary>
    /// (Unity) Вызывается при каждом обновлении кадра
    /// </summary>
    void Update()
    {
        //DateTime localDate = DateTime.Now;
        //TimeSpan deltaTime = result1 - localDate;
        //mIf.text = deltaTime.Hours.ToString() + " h " + deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
        dad.Action();
        zoomObj.zoomGameField();
    }

    /// <summary>
    /// Запускает заполнение текстового поля в зависимости от платформы
    /// </summary>
    public void startSetText()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //StartCoroutine(GetText());
        }
        else
        {
            //getFromServerWin();
        }
    }

    /*IEnumerator GetText()
    {
        UnityWebRequest www = new UnityWebRequest(Config.serverUrl);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            mIf.text = www.error;
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            //WebWindows.resultToResultClass(www.downloadHandler.text);
        }
    }*/

    /// <summary>
    /// Получает данные с сервера (Windows)
    /// </summary>
    /*private async void getFromServerWin()
    {
        ww = new WebWindows();
        // Данные для отправки
        //ResultClass jsonData = new(){did="5",time_fishing=DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z")};
        //string postData = JsonUtility.ToJson(jsonData);
        string postData = "{}";

        StartCoroutine(ww.PostRequest(Config.serverUrl, postData,
            (response) =>
            {
                mIf.text = response; // Успешный ответ
                drowField(response);
            },
            (error) =>
            {
                mIf.text = "Error: " + error;
                UnityEngine.Debug.Log(error);
            }
        ));
    }*/
}
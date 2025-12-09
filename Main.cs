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

using Catatonia;
using Catatonia.Application;
using Catatonia.Application.Models;
using Catatonia.Application.Web;

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

    public float panSpeed; // Скорость передвижения камеры
    private Vector2 startMousePos; // Начальная позиция мыши при зажиме кнопки

    public float zoomSpeed; // Скорость изменения размеров
    public float minSize;   // Минимальный размер камеры
    public float maxSize;  // Максимальный размер камеры

    private Camera cam;

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
        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mB.onClick.AddListener(startSetText);
        result1 = DateTime.Now;
        cam = GameObject.Find("mainCamera").GetComponent<Camera>();
        mIf = GameObject.Find("MainIf").GetComponent<InputField>();
        
        panSpeed = 0.3f; // Скорость передвижения камеры
        zoomSpeed = 0.4f; // Скорость изменения размеров
        minSize = 5f;   // Минимальный размер камеры
        maxSize = 20f;  // Максимальный размер камеры
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
        mIf.text = deltaTime.Hours.ToString() + " h " + deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
        //dad.Action();

        // Проверяем, зажата ли левая кнопка мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Запоминаем начальную позицию мыши
            startMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // Рассчитываем разницу в положении мыши в двухмерных координатах
            Vector2 delta = (Vector2)Input.mousePosition - startMousePos;

            // Направление движения камеры (обратное направление мыши)
            Vector2 moveDirection = delta;

            // Смещаем камеру против направления мыши
            cam.transform.Translate(-moveDirection.normalized * panSpeed, Space.World);

            // Обновляем стартовую позицию мыши
            startMousePos = Input.mousePosition;
        }

        float scrollValue = Input.mouseScrollDelta.y;

        if (scrollValue != 0)
        {
            // Изменяем ортографический размер камеры
            cam.orthographicSize -= scrollValue * zoomSpeed; //  * Time.deltaTime
            // Ограничиваем диапазон размеров
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        }
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
    }

    /// <summary>
    /// Получает данные с сервера (Windows)
    /// </summary>
    private async void getFromServerWin()
    {
        WebWindows ww = new();
        ww.getResult(result1, mIf);
    }
}
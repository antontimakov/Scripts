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
    /// Путь к серверу
    /// </summary>
    string serverUrl;

    public float panSpeed = 10f; // Скорость передвижения камеры
    private Vector2 startMousePos; // Начальная позиция мыши при зажиме кнопки

    public float zoomSpeed = 10f; // Скорость изменения размеров
    public float minSize = 5f;   // Минимальный размер камеры
    public float maxSize = 20f;  // Максимальный размер камеры

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
        mIf = GameObject.Find("MainIf").GetComponent<InputField>();
        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mB.onClick.AddListener(startSetText);
        result1 = DateTime.Now;
        serverUrl = "http://192.168.1.199:5074/getdb";
        cam = GameObject.Find("mainCamera").GetComponent<Camera>();
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
            cam.transform.Translate(-moveDirection.normalized * panSpeed * Time.deltaTime, Space.World);

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
        UnityWebRequest www = new UnityWebRequest(serverUrl);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            mIf.text = www.error;
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            resultToResultClass(www.downloadHandler.text);
        }
    }

    /// <summary>
    /// Получает данные с сервера (Windows)
    /// </summary>
    private async void getFromServerWin()
    {
        using HttpClient client = new();
        using HttpResponseMessage result = await client.GetAsync(serverUrl);
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
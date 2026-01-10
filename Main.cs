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
    /// Ссылка на кнопку закрытия модального окна магазина
    /// </summary>
    Button bCloseShop;

    /// <summary>
    /// Ссылка на объект активного в данный момент элемента
    /// </summary>
    GameObject oActiveItem;

    /// <summary>
    /// Ссылка на кнопку активного в данный момент элемента
    /// </summary>
    Button bActiveItem;

    /// <summary>
    /// Ссылка на модальное окно магазина
    /// </summary>
    GameObject wShop;

    /// <summary>
    /// Ссылка на панель магазина
    /// </summary>
    GameObject pShop;

    /// <summary>
    /// Ссылка на первую кнопку магазина
    /// </summary>
    Button bShop1;

    /// <summary>
    /// Ссылка на вторую кнопку магазина
    /// </summary>
    Button bShop2;

    /// <summary>
    /// 
    /// </summary>
    Sprite sGrass;

    /// <summary>
    /// Ссылка выбранный спрайт
    /// </summary>
    Sprite sCurrent;

    /// <summary>
    /// Хранит дату и время для результата
    /// </summary>
    DateTime result1;
    string result2;

    /// <summary>
    /// Экземпляр класса DragAndDrop
    /// </summary>
    DragAndDrop dad;

    /// <summary>
    /// Экземпляр класса GameField
    /// </summary>
    GameField gameFieldObj;

    public float panSpeed; // Скорость передвижения камеры
    private Vector2 startMousePos; // Начальная позиция мыши при зажиме кнопки

    public float zoomSpeed; // Скорость изменения размеров
    public float minSize;   // Минимальный размер камеры
    public float maxSize;  // Максимальный размер камеры

    private Camera cam;

    public WebServer serverObj;

    /// <summary>
    /// (Unity) Выполняет действия до обновления первого кадра
    /// </summary>
    void Start()
    {
        initVars();
        //startSetText();
        hideShop();
        hideActiveItem();

        mB.onClick.AddListener(showShop);
        bCloseShop.onClick.AddListener(hideShop);
    }

    /// <summary>
    /// Инициализация переменных
    /// </summary>
    private void initVars()
    {
        dad = new(this);
        serverObj = new();
        gameFieldObj = new(this, serverObj)
        {
            grassPrefab = Resources.Load<GameObject>("Prefabs/grass"),
            groundPrefab = Resources.Load<GameObject>("Prefabs/ground")
        };

        mB = GameObject.Find("MainButton").GetComponent<Button>();
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        mIf = GameObject.Find("MainText").GetComponent<TextMeshProUGUI>();
        bCloseShop = GameObject.Find("ButtonCloseShop").GetComponent<Button>();
        wShop = GameObject.Find("ModalWindowShop");
        pShop = GameObject.Find("PanelShop");
        bShop1 = GameObject.Find("ShopButton1").GetComponent<Button>();
        bShop2 = GameObject.Find("ShopButton2").GetComponent<Button>();
        oActiveItem = GameObject.Find("ActiveItemButton");
        bActiveItem = oActiveItem.GetComponent<Button>();

        result1 = DateTime.Now;
        
        panSpeed = 0.3f; // Скорость передвижения камеры
        zoomSpeed = 0.4f; // Скорость изменения размеров
        minSize = 1f;   // Минимальный размер камеры
        maxSize = 20f;  // Максимальный размер камеры
    }
    public void mainCopyPrefab(GameObject prefab, Vector2 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    private void showShop() {
        wShop.SetActive(true);
        sGrass = Resources.Load<Sprite>("Sprites/grass");
        Sprite sGround = Resources.Load<Sprite>("Sprites/ground");
        bShop1.GetComponent<Image>().sprite = sGrass;
        bShop2.GetComponent<Image>().sprite = sGround;
        bShop1.onClick.AddListener(SetActiveItem);
    }

    private void hideShop() {
        wShop.SetActive(false);
    }

    private void SetActiveItem()
    {
        showActiveItem();
        bActiveItem.GetComponent<Image>().sprite = sGrass;
        hideShop();
    }

    private void showActiveItem() {
        oActiveItem.SetActive(true);
    }

    private void hideActiveItem() {
        oActiveItem.SetActive(false);
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
        //mIf.text = deltaTime.Hours.ToString() + " h " + deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
        dad.Action();

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

    /// <summary>
    /// Получает данные с сервера (Windows)
    /// </summary>
    public async void setServerWin(Transform itemTransform)
    {
        ElemModel jsonData = new(){
            elem_id = 1,
            elem_name = "grass",
            x = (int)itemTransform.position.x,
            y = (int)itemTransform.position.y
        };
        string postData = JsonUtility.ToJson(jsonData);
                UnityEngine.Debug.Log(postData);
        /*StartCoroutine(ww.PostRequest(Config.serverUrl2, postData,
            (response) =>
            {
                // Успешный ответ
                UnityEngine.Debug.Log(response);
                //response;
            },
            (error) =>
            {
                UnityEngine.Debug.Log(error);
            }
        ));*/
    }
}
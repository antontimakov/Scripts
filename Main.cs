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
    public TextMeshProUGUI mIf;

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
    public ActiveItem activeItemObj;

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

        mB.onClick.AddListener(shopObj.showShop);
    }

    /// <summary>
    /// Инициализация переменных
    /// </summary>
    private void initVars()
    {

        Sprite sGrassLocal = Resources.Load<Sprite>("Sprites/grass");

        serverObj = new();
        gameFieldObj = new(this, serverObj);
        dad = new(this, gameFieldObj);
        activeItemObj = new(this);
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
    public GameObject mainCopyObj(GameObject prefab, Vector2 position, ElemModel data = null)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        
        if (data != null)
        {
            var comp = obj.AddComponent<DataDb>();
            comp.serverData = data;
        }

        return obj;
    }


    /// <summary>
    /// Заменяет указанный объект новым
    /// </summary>
    /// <param name="newObj">Заменяющий объект</param>
    /// <param name="oldObj">Заменяемый объект</param>
    public void mainChangeObj(GameObject oldObj, GameObject newObj)
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
}
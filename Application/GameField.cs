// Application/GameField.cs
using System;
using System.Globalization;
using UnityEngine;
using Catatonia.Application.Models;

namespace Catatonia.Application;
public class GameField
{
    public GameObject grassPrefab;
    public GameObject groundPrefab;

    private Main mainObj;
    private WebServer serverObj;

    public GameField(Main mainObj, WebServer serverObj)
    {
        this.mainObj = mainObj;
        this.serverObj = serverObj;
        groundPrefab = Resources.Load<GameObject>("Prefabs/ground");
        grassPrefab = Resources.Load<GameObject>("Prefabs/grass");
        getDataAndDrowField();
        
    }

    /// <summary>
    /// Получение данных с сервера и отрисовка поля
    /// </summary>
    private void getDataAndDrowField()
    {
        mainObj.StartCoroutine(
            serverObj.PostRequest(
                Config.serverUrl,
                "{}",
                drowField,
                UnityEngine.Debug.Log
            )
        );
    }

    /// <summary>
    /// Отрисовка поля
    /// </summary>
    public void drowField(string fromServer)
    {
        ResultClass result = JsonUtility.FromJson<ResultClass>(fromServer);
        
        foreach (ElemModel elem in result.received)
        {
            if (DateTime.TryParse(elem.updated, null, DateTimeStyles.RoundtripKind, out DateTime updatedTime))
            {
                UnityEngine.Debug.Log(updatedTime);
                elem.updated_modefied = updatedTime;
            }
            else
            {
                UnityEngine.Debug.Log("Не удалось преобразовать строку в DateTime");
            }

            string grain = "";
            // Проверка, что элемент ещё живой
            if (updatedTime.AddSeconds(elem.elem_lifetime) > DateTime.UtcNow)
            {
                grain = "_grain";
            }
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + elem.elem_name + grain);

            if (prefab != null)
            {
                Vector2 position = new Vector2(elem.x, elem.y);
                GameObject newObj = mainObj.mainCopyObj(prefab, position, elem);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Prefab not found for elem_name: {elem.elem_name}");
            }
        }
        /*ResultClass myObject;
        myObject = JsonUtility.FromJson<ResultClass>(resultFromServer);
        mIf.text = myObject.time_fishing;
        string format = "yyyy-MM-dd\\THH:mm:ss.fffff";
        result1 = DateTime.ParseExact(myObject.time_fishing, format, CultureInfo.InvariantCulture);*/
    }

    /// <summary>
    /// Отправляет данные на сервер
    /// </summary>
    public void defineClickAction(GameObject obj)
    {
        ElemModel data = obj.GetComponent<DataDb>().serverData;
        if (data.elem_weed)
        {
            string oldElemName = data.elem_name;
            string newElemName = "ground";
            mainObj.mainChangeObj(obj, groundPrefab);
            SetServer(obj.transform, oldElemName, newElemName);
        }
        else if (data.elem_plantable)
        {
            string oldElemName = data.elem_name;
            string newElemName = mainObj.activeItemObj.ActiveSpriteName;
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + newElemName + "_grain");
            if (prefab != null)
            {
                mainObj.mainChangeObj(obj, prefab);
                SetServer(obj.transform, oldElemName, newElemName);
            }
        }
        else if (data.elem_harvestable)
        {
            string oldElemName = data.elem_name;
            string newElemName = "grass";
            mainObj.mainChangeObj(obj, grassPrefab);
            SetServer(obj.transform, oldElemName, newElemName);
        }
    }
    private void SetServer(Transform itemTransform, string oldElemName, string newElemName)
    {
        ClickModel jsonData = new(){
            old_elem_name = oldElemName,
            new_elem_name = newElemName,
            x = (int)itemTransform.position.x,
            y = (int)itemTransform.position.y
        };
        string postData = JsonUtility.ToJson(jsonData);
        mainObj.StartCoroutine(serverObj.PostRequest(Config.serverUrl2, postData,
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
        ));
    }
}

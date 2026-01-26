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
        ResultClass<StartModel> result = JsonUtility.FromJson<ResultClass<StartModel>>(fromServer);

        if (result == null)
        {
            UnityEngine.Debug.Log("result is null");
        }
        if (result.received == null)
        {
            UnityEngine.Debug.Log("result.received is null");
        }
        if (result.received.fieldElements == null)
        {
            UnityEngine.Debug.Log("result.received.fieldElements is null");
        }
        
        foreach (FillFieldDbr elem in result.received.fieldElements)
        {
            if (DateTime.TryParse(elem.updated, null, DateTimeStyles.RoundtripKind, out DateTime updatedTime))
            {
                //UnityEngine.Debug.Log(updatedTime);
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
    }

    /// <summary>
    /// Отправляет данные на сервер
    /// </summary>
    public void defineClickAction(GameObject obj)
    {
        DataDb dataDbObj = obj.GetComponent<DataDb>();
        if (dataDbObj != null){
            FillFieldDbr data = dataDbObj.serverData;
            if (data != null){
                GameObject oActiveItem = GameObject.Find("ActiveItemButton");
                if (data.elem_weed)
                {
                    string oldElemName = data.elem_name;
                    string newElemName = "ground";
                    FillFieldDbr newData = new FillFieldDbr()
                    {
                        elem_name = newElemName,
                        x = data.x,
                        y = data.y,
                        elem_plantable = true,
                        elem_harvestable = false,
                        elem_weed = false,
                        elem_lifetime = 0,
                        updated = "",
                        updated_modefied = DateTime.UtcNow
                    };
                    mainObj.mainChangeObj(obj, groundPrefab, newData);
                    SetServer(obj.transform, oldElemName, newElemName);
                }
                else if (data.elem_plantable && oActiveItem && oActiveItem.activeSelf)
                {
                    string oldElemName = data.elem_name;
                    string newElemName = mainObj.activeItemObj.ActiveSpriteName;
                    FillFieldDbr newData = new FillFieldDbr()
                    {
                        elem_name = newElemName,
                        x = data.x,
                        y = data.y,
                        elem_plantable = false,
                        elem_harvestable = true,
                        elem_weed = false,
                        elem_lifetime = 60,
                        updated = "",
                        updated_modefied = DateTime.UtcNow
                    };
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/" + newElemName + "_grain");
                    if (prefab != null)
                    {
                        mainObj.mainChangeObj(obj, prefab, newData);
                        SetServer(obj.transform, oldElemName, newElemName);
                    }
                }
                else if (data.elem_harvestable && data.updated_modefied.HasValue)
                {
                    DateTime increasedUpdated = data.updated_modefied.Value.AddSeconds(data.elem_lifetime);
                    if (increasedUpdated < DateTime.UtcNow)
                    {
                        string oldElemName = data.elem_name;
                        string newElemName = "grass";
                        FillFieldDbr newData = new FillFieldDbr()
                        {
                            elem_name = newElemName,
                            x = data.x,
                            y = data.y,
                            elem_plantable = false,
                            elem_harvestable = true,
                            elem_weed = true,
                            elem_lifetime = 0,
                            updated = "",
                            updated_modefied = DateTime.UtcNow
                        };
                        mainObj.mainChangeObj(obj, grassPrefab, newData);
                        SetServer(obj.transform, oldElemName, newElemName);
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("data is null");
            }
        }
        else
        {
            UnityEngine.Debug.Log("dataDbObj is null");
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

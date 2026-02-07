// Application/GameField.cs
using System;
using System.Globalization;
using UnityEngine;
using Catatonia.Application.Models;
using TMPro;

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
        UnityEngine.Debug.Log(fromServer);

        if (result == null)
        {
            UnityEngine.Debug.Log("result is null");
        }
        if (result.Received == null)
        {
            UnityEngine.Debug.Log("result.Received is null");
        }
        if (result.Received.FieldElements == null)
        {
            UnityEngine.Debug.Log("result.Received.FieldElements is null");
        }
        if (result.Received.UserInfo == null)
        {
            UnityEngine.Debug.Log("result.Received.UserInfo is null");
        }
        TextMeshProUGUI goldText = GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
        goldText.text = result.Received.UserInfo[0].Gold.ToString();
        foreach (FillFieldDbr elem in result.Received.FieldElements)
        {
            if (DateTime.TryParse(elem.UpdatedAt, null, DateTimeStyles.RoundtripKind, out DateTime UpdatedTime))
            {
                //UnityEngine.Debug.Log(updatedTime);
                elem.UpdatedAtModefied = UpdatedTime;
            }
            else
            {
                UnityEngine.Debug.Log("Не удалось преобразовать строку в DateTime");
            }

            string grain = "";
            // Проверка, что элемент ещё живой
            if (UpdatedTime.AddSeconds(elem.Lifetime) > DateTime.UtcNow)
            {
                grain = "_grain";
            }
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + elem.Name + grain);

            if (prefab != null)
            {
                Vector2 position = new Vector2(elem.X, elem.Y);
                GameObject newObj = mainObj.mainCopyObj(prefab, position, elem);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Prefab not found for elem_name: {elem.Name}");
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
                if (data.IsWeed)
                {
                    string OldElemName = data.Name;
                    string NewElemName = "ground";
                    FillFieldDbr newData = new FillFieldDbr()
                    {
                        Name = NewElemName,
                        X = data.X,
                        Y = data.Y,
                        IsPlantable = true,
                        IsHarvestable = false,
                        IsWeed = false,
                        Lifetime = 0,
                        UpdatedAt = "",
                        UpdatedAtModefied = DateTime.UtcNow
                    };
                    mainObj.mainChangeObj(obj, groundPrefab, newData);
                    SetServer(obj.transform, OldElemName, NewElemName);
                }
                else if (data.IsPlantable && oActiveItem && oActiveItem.activeSelf)
                {
                    string OldElemName = data.Name;
                    string NewElemName = mainObj.activeItemObj.ActiveSpriteName;
                    FillFieldDbr newData = new FillFieldDbr()
                    {
                        Name = NewElemName,
                        X = data.X,
                        Y = data.Y,
                        IsPlantable = false,
                        IsHarvestable = true,
                        IsWeed = false,
                        Lifetime = 60,
                        UpdatedAt = "",
                        UpdatedAtModefied = DateTime.UtcNow
                    };
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/" + NewElemName + "_grain");
                    if (prefab != null)
                    {
                        mainObj.mainChangeObj(obj, prefab, newData);
                        SetServer(obj.transform, OldElemName, NewElemName);
                    }
                }
                else if (data.IsHarvestable && data.UpdatedAtModefied.HasValue)
                {
                    DateTime increasedUpdated = data.UpdatedAtModefied.Value.AddSeconds(data.Lifetime);
                    if (increasedUpdated < DateTime.UtcNow)
                    {
                        string OldElemName = data.Name;
                        string NewElemName = "grass";
                        FillFieldDbr newData = new FillFieldDbr()
                        {
                            Name = NewElemName,
                            X = data.X,
                            Y = data.Y,
                            IsPlantable = false,
                            IsHarvestable = true,
                            IsWeed = true,
                            Lifetime = 0,
                            UpdatedAt = "",
                            UpdatedAtModefied = DateTime.UtcNow
                        };
                        mainObj.mainChangeObj(obj, grassPrefab, newData);
                        SetServer(obj.transform, OldElemName, NewElemName);
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
    private void SetServer(Transform itemTransform, string OldElemNamePar, string NewElemNamePar)
    {
        ClickModel jsonData = new(){
            OldElemName = OldElemNamePar,
            NewElemName = NewElemNamePar,
            X = (int)itemTransform.position.x,
            Y = (int)itemTransform.position.y
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

// Application/GameField.cs
using UnityEngine;
using Catatonia.Application.Models;

namespace Catatonia.Application;
public class GameField
{
    public GameObject grassPrefab;
    public GameObject groundPrefab;
    public GameObject grainPrefab;

    private Main mainObj;
    private WebServer serverObj;

    public GameField(Main mainObj, WebServer serverObj)
    {
        this.mainObj = mainObj;
        this.serverObj = serverObj;
        groundPrefab = Resources.Load<GameObject>("Prefabs/ground");
        grassPrefab = Resources.Load<GameObject>("Prefabs/grass");
        grainPrefab = Resources.Load<GameObject>("Prefabs/magic_plant");
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
            GameObject prefab = null;

            switch (elem.elem_name)
            {
                    case "magic_plant": prefab = grainPrefab; break;
                    case "grass": prefab = grassPrefab; break;
                    case "ground": prefab = groundPrefab; break;
            }

            if (prefab != null)
            {
                Vector2 position = new Vector2(elem.x, elem.y);
                GameObject newObj = mainObj.mainCopyObj(prefab, position, elem);
                //UnityEngine.Debug.Log(newObj.GetComponent<DataDb>().serverData.elem_name);
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

        //Vector2 offset2 = go.transform.position;
        /*Vector2 offset2 = new();
        offset2.x = 0;
        offset2.y = 0;
        Instantiate(go, offset2, Quaternion.identity);
        offset2.x = 2f;
        offset2.y = 0;
        Instantiate(go2, offset2, Quaternion.identity);*/
        //reppeatRight(GameObject.Find("grass"), 10);
        //reppeatRight(GameObject.Find("ground"), 10);
    }

    /// <summary>
    /// Отправляет данные на сервер
    /// </summary>
    public void setServerWin(Transform itemTransform)
    {
        ElemModel jsonData = new(){
            elem_id = 1,
            elem_name = "grass",
            x = (int)itemTransform.position.x,
            y = (int)itemTransform.position.y
        };
        string postData = JsonUtility.ToJson(jsonData);
                //UnityEngine.Debug.Log(postData);
        mainObj.StartCoroutine(serverObj.PostRequest(Config.serverUrl2, postData,
            (response) =>
            {
                // Успешный ответ
                //UnityEngine.Debug.Log(response);
                //response;
            },
            (error) =>
            {
                UnityEngine.Debug.Log(error);
            }
        ));
    }
}

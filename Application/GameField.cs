// Application/GameField.cs
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
        UnityEngine.Debug.Log(fromServer);
        ResultClass result = JsonUtility.FromJson<ResultClass>(fromServer);
        
        // Загружаем префабы (предварительно можно закэшировать)
        foreach (ElemModel elem in result.received)
        {
            GameObject prefab = null;

            if (elem.elem_name == "grass") prefab = grassPrefab;
            else if (elem.elem_name == "ground") prefab = groundPrefab;

            if (prefab != null)
            {
                Vector2 position = new Vector2(elem.x, elem.y);
                mainObj.mainCopyObj(prefab, position);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Prefab not found for elem_name: {elem.elem_name}");
            }
            UnityEngine.Debug.Log(elem.elem_name);
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

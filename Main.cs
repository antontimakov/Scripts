using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System;
using System.Globalization;
using System.Diagnostics;

public class Main : MonoBehaviour
{

    InputField mIf;
    Button mB;
    DateTime result1;
    DragAndDrop dad; // = new DragAndDrop(this);

    Main()
    {
        dad = new DragAndDrop(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        mIf = GameObject.Find("MainIf").GetComponent<InputField>();
        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mB.onClick.AddListener(Bc);

        multyObj(GameObject.Find("grass"));
        multyObj(GameObject.Find("ground"));
    }

    public void chObj(GameObject goNew, GameObject goOld)
    {
        Instantiate(goNew, goOld.transform.position, Quaternion.identity);
        Destroy(goOld);
    }

    public void multyObj(GameObject go)
    {
        Vector2 offset = go.transform.position;
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset2 = offset;
            offset2.x = offset.x + i * 2;
            Instantiate(go, offset2, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

        DateTime localDate = DateTime.Now;
        TimeSpan deltaTime = result1 - localDate;
        mIf.text = deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
        dad.Action();
        //public Camera cam;
        //if (Input.GetMouseButton(0))
        //{
        //    Camera.main.fieldOfView = 30;
        //}
    }

    public void Bc()
    {
        // web
        //StartCoroutine(GetText());

        // decktop
        GetTextDesc();
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = new UnityWebRequest("http://192.168.1.199:5074/getdb");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            mIf.text = www.error;
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);
            resultToResultClass(www.downloadHandler.text);

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }
    private async void GetTextDesc()
    {

        using HttpClient client = new();
        using HttpResponseMessage result = await client.GetAsync("http://192.168.1.199:5074/getdb");
        resultToResultClass(await result.Content.ReadAsStringAsync());
    }
    private void resultToResultClass(String resultFromServer)
    {
        UnityEngine.Debug.Log(resultFromServer);
        ResultClass myObject;
        myObject = JsonUtility.FromJson<ResultClass>(resultFromServer);
        mIf.text = myObject.time_fishing;
        string format = "yyyy-MM-dd\\THH:mm:ss.fffff";
        result1 = DateTime.ParseExact(myObject.time_fishing, format, CultureInfo.InvariantCulture);
        UnityEngine.Debug.Log(result1);
    }
}

public class ResultClass
{
    public int? did = null;
    public string? time_fishing = null; // DateTime
}


class DragAndDrop
{

    State state;
    GameObject item;
    Vector2 offset;
    Vector2 nullVector;
    Vector2 offsetCell;
    Main mainOo;

    public DragAndDrop(Main mainO)
    {
        state = State.none;
        item = null;
        nullVector = new Vector2(-3.43f, 4.2f);
        offsetCell = new Vector2(1.1f, -1.1f);
        mainOo = mainO;
    }
    public void Action()
    {
        switch (state)
        {
            case State.none:
                if (isMouseButtonPressed())
                {
                    pickup();
                }
                break;

            case State.drag:
                if (isMouseButtonPressed())
                {
                    drag();
                }
                else
                {
                    drop();
                }
                break;
        }
    }

    bool isMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    Vector2 getClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Transform GetItemAt(Vector2 position)
    {
        RaycastHit2D[] figuries = Physics2D.RaycastAll(position, position, 1f);
        if (figuries.Length > 0)
        {
            return figuries[0].transform;
        }
        return null;
    }
    void pickup()
    {
        Vector2 clickPosition = getClickPosition();
        Transform clickedItem = GetItemAt(clickPosition);
        if (clickedItem == null)
        {
            return;
        }
        item = clickedItem.gameObject;
        //UnityEngine.Debug.Log(item.name);
        offset = (Vector2)clickedItem.position - clickPosition;
        state = State.drag;
    }

    void drag()
    {
        item.transform.position = getClickPosition() + offset;
    }
    void drop()
    {
        item.transform.position = calcCellPosition(getClickPosition());
        item = null;
        mainOo.chObj(GameObject.Find("grass"), GameObject.Find("ground"));
        state = State.none;
    }

    Vector2 calcCellPosition(Vector2 ItemPosition)
    {
        // было
        //return ItemPosition + offset;
        Vector2 cp = getClickPosition();
        Vector2 res = nullVector;
        for (int i = 0; i < 7; ++i)
        {
            if (cp.x > (res.x/* + offset.x*/))
            {
                res.x += offsetCell.x;
            }
            if (cp.y < (res.y/* + offset.y*/))
            {
                res.y += offsetCell.y;
            }
        }
        return res;
    }

    enum State
    {
        none,
        //pick,
        drag//,
        //drop
    }
}
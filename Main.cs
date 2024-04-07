using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System;
using System.Globalization;
using System.Diagnostics;
//using System.Diagnostics;

public class Main : MonoBehaviour
{

    InputField mIf;
    Button mB;
    DateTime result1;

    // Start is called before the first frame update
    void Start()
    {
        mIf = GameObject.Find("MainIf").GetComponent<InputField>();
        mB = GameObject.Find("MainButton").GetComponent<Button>();
        mB.onClick.AddListener(Bc);
    }

    // Update is called once per frame
    void Update()
    {

        DateTime localDate = DateTime.Now;
        TimeSpan deltaTime = result1 - localDate;
        mIf.text = deltaTime.Minutes.ToString() + " m " + deltaTime.Seconds.ToString() + " s";
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
        UnityWebRequest www = new UnityWebRequest("http://localhost:8078/getdb");
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
        using HttpResponseMessage result = await client.GetAsync("http://192.168.1.105:8078/getdb");
        resultToResultClass(await result.Content.ReadAsStringAsync());
    }
    private void resultToResultClass(String resultFromServer)
    {
        ResultClass myObject = new();
        myObject = JsonUtility.FromJson<ResultClass>(resultFromServer);
        mIf.text = myObject.time;
        string format = "yyyy-MM-ddTHH:mm:ss.000Z";
        result1 = DateTime.ParseExact(myObject.time, format, CultureInfo.InvariantCulture);
        UnityEngine.Debug.Log(result1);
    }
}

public class ResultClass
{
    public string? did = null;
    public string? time = null;
}

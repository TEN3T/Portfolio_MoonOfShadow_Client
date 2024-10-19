using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class WebRequestManager
{
    #region ResponseCode
    public enum EResponse
    {
        eSuccess = 200,
        eNotFount = 404,
        eQueryError = 500,
    }

    #endregion

    private Crypto crypto;
    public WebRequestManager(Crypto crypto) {
        DebugManager.Instance.PrintDebug("[WebRequestManager] WebRequestManager Init");
        this.crypto = crypto;
    }

    private bool isPostUsing = false;
    private Queue<int> postQueue = new Queue<int>();

    /// <summary>
    /// API 종료 여부
    /// </summary>
    private bool isAPIFinished = false;

    /// <summary>
    /// API 수신 성공 여부
    /// </summary>
    private bool isSuccessApiReceived = false;

    /// <summary>
    /// 타임아웃
    /// </summary>
    const float TIMEOUT = 3.0f;


    public const string WEBSERVICE_HOST = "https://wr.blackteam.kr";
    //Singleton을 활용하여 1개의 인스턴스 유지 및 접근 효율성 증가

    public bool IsConnectInternet()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            DebugManager.Instance.PrintDebug("인터넷 끊힘");
            return false;
        }
        else return true;
    }



    private String GetDictToString(Dictionary<string, object> forms)
    {

        string jsonData = JsonConvert.SerializeObject(forms);
      
        Dictionary<string, string> postData = new Dictionary<string, string>
        {
            { "data",  crypto.Encrypt(jsonData)}
        };

        DebugManager.Instance.PrintDebug("[WebRequester] " + jsonData);
     
        return JsonConvert.SerializeObject(postData);
    }


    public string MakeUrlWithParam(string url, Dictionary<string, object> data) {
        if (data != null)
        {
            url += "?";
            foreach (string i in data.Keys)
            {
                url += i + "=" + data[i] + "&";
            }
            url = url.Substring(0, url.Length - 1);
        }
        return url;
    }



    public async Task<object> Get<T>(string url, Dictionary<string, object> data = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{WEBSERVICE_HOST}/{MakeUrlWithParam(url, data)}"))
        {
            DebugManager.Instance.PrintDebug("[RequestManager] Send get request to " + $"{WEBSERVICE_HOST}/{MakeUrlWithParam(url, data)}");
            float timeout = 0f;
            request.SendWebRequest();
            while (!request.isDone)
            {
                timeout += Time.deltaTime;
                if (timeout > TIMEOUT)
                    return default;
                else
                    await Task.Yield();
            }
            DebugManager.Instance.PrintDebug(request.downloadHandler.text);
            var jsonString = request.downloadHandler.text;
            var dataObj = JsonConvert.DeserializeObject<T>(jsonString);

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {request.error}");
            
            return dataObj;

        }

        return default;

    }


    public async Task<object> Post<T>(string url, Dictionary<string, object> data)
    {
        UnityWebRequest request = new UnityWebRequest($"{WEBSERVICE_HOST}/{url}", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(GetDictToString(data));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        using (request)
        {
            float timeout = 0f;
            request.SendWebRequest();
            while (!request.isDone)
            {
                timeout += Time.deltaTime;
                if (timeout > TIMEOUT)
                    return default;
                else
                    await Task.Yield();
            }

            var jsonString = request.downloadHandler.text;
            var dataObj = JsonConvert.DeserializeObject<T>(jsonString);

            DebugManager.Instance.PrintDebug("[WebAPI]Result of Request :\n"+ request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {request.error}");

            return dataObj;

        }
        return default;

    }
    public async Task<object> Patch<T>(string url, Dictionary<string, object> data)
    {
        UnityWebRequest request = new UnityWebRequest($"{WEBSERVICE_HOST}/{url}", "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(GetDictToString(data));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        using (request)
        {
            float timeout = 0f;
            request.SendWebRequest();
            while (!request.isDone)
            {
                timeout += Time.deltaTime;
                if (timeout > TIMEOUT)
                    return default;
                else
                    await Task.Yield();
            }

            var jsonString = request.downloadHandler.text;
            var dataObj = JsonConvert.DeserializeObject<T>(jsonString);

            DebugManager.Instance.PrintDebug("[WebAPI]Result of Request :\n" + request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {request.error}");

            return dataObj;

        }
        return default;

    }

}

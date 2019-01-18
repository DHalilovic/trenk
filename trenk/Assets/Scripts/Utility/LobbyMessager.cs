using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMessager : MonoBehaviour
{
    public string serverUrl;

    public void GetHost()
    {

    }

    public void AddSelfHost()
    {
        StartCoroutine(AddSelfHostCo());
    }

    IEnumerator AddSelfHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, new WWWForm()))
        {
            www.timeout = 1;
            Debug.Log("ABOUT TO SEND");
            yield return www.SendWebRequest();
            Debug.Log("SENT");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Posted");
            }
        }
    }

    public void RemoveSelfHost()
    {

    }
}

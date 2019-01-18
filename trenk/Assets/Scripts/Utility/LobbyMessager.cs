using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMessager : MonoBehaviour
{
    public string serverUrl;
    public int timeout;

    private string hostUrl;
    public string HostUrl{get { return hostUrl; } }

    public void GetRandomHost()
    {

    }

    public void AddSelfHost()
    {
        StartCoroutine(AddSelfHostCo());
    }

    public void RemoveSelfHost()
    {
        StartCoroutine(RemoveSelfHostCo());
    }

    IEnumerator GetRandomHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverUrl))
        {
            www.timeout = 1;
            Debug.Log("Sending...");
            yield return www.SendWebRequest();
            Debug.Log("Get Sent");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                hostUrl = www.downloadHandler.text;
                Debug.Log("Received " + hostUrl);
            }
        }
    }

    IEnumerator AddSelfHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, new WWWForm()))
        {
            www.timeout = timeout;
            Debug.Log("Sending...");
            yield return www.SendWebRequest();
            Debug.Log("Post Sent");

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

    IEnumerator RemoveSelfHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(serverUrl))
        {
            www.timeout = 1;
            Debug.Log("Sending...");
            yield return www.SendWebRequest();
            Debug.Log("Delete Sent");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Deleted");
            }
        }
    }




}

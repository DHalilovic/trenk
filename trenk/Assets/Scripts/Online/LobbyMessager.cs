using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMessager : MonoBehaviour
{
    public string HostEvent, ClientEvent;
    public string lobbyUrl;
    public int timeout;
    public string HostUrl { get; private set; }

    public void GetRandomHost()
    {
        StartCoroutine(GetRandomHostCo());
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
        using (UnityWebRequest www = UnityWebRequest.Get(lobbyUrl))
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
                HostUrl = www.downloadHandler.text;
                Debug.Log("Received " + HostUrl);

                int colonPos = HostUrl.LastIndexOf(':');

                EventManager.Instance.Raise("client",
                    new IpParam(HostUrl.Substring(0, colonPos), short.Parse(HostUrl.Substring(colonPos + 1)))
                );
            }
        }
    }

    IEnumerator AddSelfHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(lobbyUrl, new WWWForm()))
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
        using (UnityWebRequest www = UnityWebRequest.Delete(lobbyUrl))
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

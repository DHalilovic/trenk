﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMessager : MonoBehaviour
{
    public string lobbyUrl = "http://127.0.0.1:8080";
    public short defaultPort = 8080;

    public int timeout;

    public void GetHost()
    {
        StartCoroutine(GetHostCo());
    }

    IEnumerator GetHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(lobbyUrl))
        {
            www.timeout = timeout;
            Debug.Log("Sending...");
            yield return www.SendWebRequest();
            Debug.Log("Get Sent");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                EventManager.Instance.Raise("lobby-error", new StringParam(www.error));
            }
            else
            {
                string url = www.downloadHandler.text;
                Debug.Log("Received " + url);

                // If no url received, this system is to host
                if (string.IsNullOrEmpty(url))
                {
                    Debug.Log("___Host");

                    EventManager.Instance.Raise("try-connect", new IpParam(true, url, defaultPort));
                }
                else // Otherwise request connecting to provided host
                {
                    Debug.Log("___Client");

                    EventManager.Instance.Raise("try-connect", new IpParam(false, url, defaultPort));
                }
            }
        }
    }
}

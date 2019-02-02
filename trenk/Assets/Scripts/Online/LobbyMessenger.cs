﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CountdownTimer))]
public class LobbyMessenger : MonoBehaviour
{
    public string lobbyUrl = "http://127.0.0.1:8080";
    public short defaultPort = 8080;
    public int lobbyTimeout, matchTimeout;

    private CountdownTimer timer;
    private Action<IEventParam> tryLobbyListener, tryConnectTimeoutListener;

    private void Awake()
    {
        timer = GetComponent<CountdownTimer>();
        tryLobbyListener = new Action<IEventParam>((e) => GetHost());
        tryConnectTimeoutListener = new Action<IEventParam>((e) => RemoveSelfHost());
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("try-lobby", tryLobbyListener);
            EventManager.Instance.Subscribe("try-connect-timeout", tryConnectTimeoutListener);
        }

    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("try-lobby", tryLobbyListener);
            EventManager.Instance.Unsubscribe("try-connect-timeout", tryConnectTimeoutListener);
        }
    }

    public void GetHost()
    {
        StartCoroutine(GetHostCo());
    }

    IEnumerator GetHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(lobbyUrl))
        {
            www.timeout = lobbyTimeout;
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
                Debug.Log("Received " + url.Length);

                // If no url received, this system is to host
                if (string.IsNullOrEmpty(url) || url.Length < 1)
                {
                    Debug.Log("___Host");

                    EventManager.Instance.Raise("try-connect", new IpParam(true, url, defaultPort));
                    timer.Launch(matchTimeout, "try-tick", "try-connect-timeout", new IntParam(timer.ClockTime), new BoolParam(true));
                }
                else // Otherwise request connecting to provided host
                {
                    Debug.Log("___Client");

                    EventManager.Instance.Raise("try-connect", new IpParam(false, url, defaultPort));
                    timer.Launch(matchTimeout, "try-tick", "try-connect-timeout", new IntParam(timer.ClockTime), new BoolParam(false));
                }
            }
        }
    }

    public void RemoveSelfHost()
    {
        StartCoroutine(RemoveSelfHostCo());
    }

    IEnumerator RemoveSelfHostCo()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(lobbyUrl))
        {
            www.timeout = lobbyTimeout;
            Debug.Log("Sending...");
            yield return www.SendWebRequest();
            Debug.Log("Delete Sent");

            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error); 
        }
    }
}
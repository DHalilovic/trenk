using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CountdownTimer))]
public class LobbyMessenger : MonoBehaviour
{
    public string lobbyUrl = "http://mas.lvc.edu:9999/trenk/hosts";
    public short defaultPort = 9999;
    public int lobbyTimeout, matchTimeout;

    private CountdownTimer timer;
    private Action<IEventParam> tryLobbyListener, tryConnectTimeoutListener, connectListener;

    private void Awake()
    {
        EventManager.Instance.Subscribe("connect", null);

        timer = GetComponent<CountdownTimer>();

        tryLobbyListener = new Action<IEventParam>((e) => GetHost());
        tryConnectTimeoutListener = new Action<IEventParam>((e) => RemoveSelfHost());
        connectListener = (e) => { Debug.Log("Stop timeout timer"); timer.Stop(); }; // TODO Coroutine stopping broken
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("try-lobby", tryLobbyListener);
            EventManager.Instance.Subscribe("try-connect-timeout", tryConnectTimeoutListener);
            EventManager.Instance.Subscribe("connect", connectListener);
        }

    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("try-lobby", tryLobbyListener);
            EventManager.Instance.Unsubscribe("try-connect-timeout", tryConnectTimeoutListener);
            EventManager.Instance.Unsubscribe("connect", connectListener);
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

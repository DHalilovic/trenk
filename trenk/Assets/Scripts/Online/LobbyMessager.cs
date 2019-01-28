using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMessager : MonoBehaviour
{
    public string lobbyUrl = "http://127.0.0.1:8080";
    public int timeout;
    public string HostUrl { get; private set; }

    private Action<IEventParam> connectListener;

    private void Awake()
    {
        connectListener = new Action<IEventParam>(OnConnect);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("try-connect", connectListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("try-connect", connectListener);
        }
    }

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

    private void OnConnect(IEventParam e)
    {
        BoolParam p = (BoolParam) e;
        
        // If host, remove own ip from server list
        if (p.val)
            RemoveSelfHost();
    }

    IEnumerator GetRandomHostCo()
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
            }
            else
            {
                HostUrl = www.downloadHandler.text;
                Debug.Log("Received " + HostUrl);

                int colonPos = HostUrl.LastIndexOf(':');

                EventManager.Instance.Raise("try-connect",
                    new IpParam(false, HostUrl.Substring(0, colonPos), short.Parse(HostUrl.Substring(colonPos + 1)))
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

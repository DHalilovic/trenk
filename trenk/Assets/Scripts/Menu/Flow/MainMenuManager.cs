using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transitioner))]
public class MainMenuManager : MonoBehaviour
{
    public LobbyMessenger messenger;
    public GameObject mainCanvas;
    public Button matchButton, trainButton;
    public GameObject connectCanvas;
    public Text connectText;
    public Button errorConfirmButton;

    private Transitioner transitioner;
    private Transitionable mainTrans, connectTrans;
    private Action<IEventParam> onLobbyErrorListener, onTryConnectListener, onTryConnectTimeoutListener, connectListener;


    private void Awake()
    {
        onLobbyErrorListener = new Action<IEventParam>(
            (e) =>
            {
                connectText.text = "Error: " + ((StringParam)e).val;
                errorConfirmButton.gameObject.SetActive(true);
            });

        onTryConnectListener = new Action<IEventParam>(
            (e) =>
            {
                IpParam p = (IpParam)e;

                connectText.text = (p.host) ? "Hosting match" : "Connecting to " + p.ip;
            });

        onTryConnectTimeoutListener = new Action<IEventParam>(
            (e) =>
            {
                BoolParam p = (BoolParam)e;

                connectText.text = "Failed to Connect";
                errorConfirmButton.gameObject.SetActive(true);
            });

        connectListener = (e) =>
            {
                Debug.Log("Connect successful");
                connectText.text = "Starting Match...";
                EventManager.Instance.Raise("request-scene", new IntParam(2));
            };

        transitioner = GetComponent<Transitioner>();
        mainTrans = mainCanvas.GetComponent<Transitionable>();
        connectTrans = connectCanvas.GetComponent<Transitionable>();
    }

    private void OnEnable()
    {
        matchButton.onClick.AddListener(
            () =>
            {
                EventManager.Instance.Raise("try-lobby", new IntParam(1));
                connectText.text = "Reaching Lobby...";
                errorConfirmButton.gameObject.SetActive(false);
                transitioner.GoOneWay(mainTrans, connectTrans);
            });

        trainButton.onClick.AddListener(
            () =>
            {
                EventManager.Instance.Raise("request-scene", new IntParam(1));
            });

        errorConfirmButton.onClick.AddListener(() => transitioner.GoOneWay(connectTrans, mainTrans));

        EventManager.Instance.Subscribe("lobby-error", onLobbyErrorListener);
        EventManager.Instance.Subscribe("try-connect", onTryConnectListener);
        EventManager.Instance.Subscribe("try-connect-timeout", onTryConnectTimeoutListener);
        EventManager.Instance.Subscribe("connect", connectListener);
    }

    private void OnDisable()
    {
        matchButton.onClick.RemoveAllListeners();
        trainButton.onClick.RemoveAllListeners();
        errorConfirmButton.onClick.RemoveAllListeners();

        EventManager.Instance.Unsubscribe("lobby-error", onLobbyErrorListener);
        EventManager.Instance.Unsubscribe("try-connect", onTryConnectListener);
        EventManager.Instance.Unsubscribe("try-connect-timeout", onTryConnectTimeoutListener);
        EventManager.Instance.Unsubscribe("connect", connectListener);

    }


}

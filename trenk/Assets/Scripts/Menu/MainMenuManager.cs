using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ITransitioner))]
public class MainMenuManager : MonoBehaviour
{
    public LobbyMessenger messenger;
    public GameObject mainCanvas;
    public Button matchButton, trainButton;
    public GameObject connectingCanvas;
    public Text connectingText;
    public Button errorConfirmButton;

    private ITransitioner transitioner;
    private Action<IEventParam> onLobbyErrorListener, onTryConnectListener, onConnectErrorListener;


    private void Awake()
    {
        transitioner = GetComponent<ITransitioner>();
        onLobbyErrorListener = new Action<IEventParam>(
            (e) =>
            {
                connectingText.text = "Error: " + ((StringParam)e).val;
                errorConfirmButton.gameObject.SetActive(true);
            });
    }

    private void OnEnable()
    {
        matchButton.onClick.AddListener(
            () =>
            {
                EventManager.Instance.Raise("try-lobby", new IntParam(1));
                connectingText.text = "Reaching Lobby...";
                errorConfirmButton.gameObject.SetActive(false);
                OpenOneWay(mainCanvas, connectingCanvas);
            });

        trainButton.onClick.AddListener(
            () =>
            {
                EventManager.Instance.Raise("scene", new IntParam(1));
            });

        errorConfirmButton.onClick.AddListener(() => OpenOneWay(connectingCanvas, mainCanvas));

        EventManager.Instance.Subscribe("lobby-error", onLobbyErrorListener);
    }

    private void OnDisable()
    {
        matchButton.onClick.RemoveAllListeners();
        trainButton.onClick.RemoveAllListeners();
        errorConfirmButton.onClick.RemoveAllListeners();

        EventManager.Instance.Unsubscribe("lobby-error", onLobbyErrorListener);

    }

    public void OpenOneWay(GameObject origin, GameObject target)
    {
        transitioner.TransitionOut(origin);
        transitioner.TransitionIn(target);
    }
}

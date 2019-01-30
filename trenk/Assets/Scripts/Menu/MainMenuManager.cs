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

    private ITransitioner transitioner;

    private void Awake()
    {
        transitioner = GetComponent<ITransitioner>();
    }

    private void OnEnable()
    {
        matchButton.onClick.AddListener(
            () =>
            {            
                OpenOneWay(mainCanvas, connectingCanvas);
            });

        trainButton.onClick.AddListener(
            () =>
            {
                EventManager.Instance.Raise("scene", new IntParam(1));
            });
    }

    public void OpenOneWay(GameObject origin, GameObject target)
    {
        transitioner.TransitionOut(origin);
        transitioner.TransitionIn(target);
    }
}

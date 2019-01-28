using System;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private Action<IEventParam> connectListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize listeners
        connectListener = new Action<IEventParam>(OnConnect);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("connect", connectListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("connect", connectListener);
        }
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    private void OnConnect(IEventParam e)
    {
        LoadScene(2); // Load multiplayer arena
    }
}

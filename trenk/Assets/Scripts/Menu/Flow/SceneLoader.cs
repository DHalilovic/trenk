using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public int Requested { get; private set; }

    private Action<IEventParam> loadSceneListener, requestListener, loadRequestListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize listeners
        loadSceneListener = OnLoadScene;
        requestListener = OnRequestScene;
        loadRequestListener = OnLoadRequest;
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("load-scene-direct", loadSceneListener);
            EventManager.Instance.Subscribe("request-scene", requestListener);
            EventManager.Instance.Subscribe("load-scene-request", loadRequestListener);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("load-scene-direct", loadSceneListener);
            EventManager.Instance.Subscribe("request-scene", requestListener);
            EventManager.Instance.Subscribe("load-scene-request", loadRequestListener);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    private void OnLoadScene(IEventParam e)
    {
        IntParam p = (IntParam)e;

        LoadScene(p.val); // Load multiplayer arena
    }

    private void OnRequestScene(IEventParam e)
    {
        IntParam p = (IntParam)e;

        if (p.val > -1)
            Requested = p.val;
    }

    private void OnLoadRequest(IEventParam e)
    {
        LoadScene(Requested);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventManager.Instance.Raise("in-scene", null);
        Debug.Log("Scene loaded");
    }
}

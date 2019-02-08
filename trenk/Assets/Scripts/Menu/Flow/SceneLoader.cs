using System;
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
        requestListener = OnLoadRequest;
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("load-scene-direct", loadSceneListener);
            EventManager.Instance.Subscribe("load-scene-request", loadRequestListener);
            EventManager.Instance.Subscribe("request-scene", requestListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("load-scene-direct", loadSceneListener);
            EventManager.Instance.Subscribe("load-scene-request", loadRequestListener);
            EventManager.Instance.Subscribe("request-scene", requestListener);
        }
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

    public void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventManager.Instance.Raise("in-scene", new IntParam(Requested));
    }
}

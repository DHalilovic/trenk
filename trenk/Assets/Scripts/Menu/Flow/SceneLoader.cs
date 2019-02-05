using System;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private Action<IEventParam> sceneListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize listeners
        sceneListener = new Action<IEventParam>(OnLoadScene);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("scene", sceneListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("scene", sceneListener);
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
}

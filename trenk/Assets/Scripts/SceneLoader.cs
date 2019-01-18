using System;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehavior
{

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}

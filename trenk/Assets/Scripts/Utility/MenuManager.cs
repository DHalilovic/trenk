using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject[] panes;

    public void OpenSinglePane(int index)
    {
        bool found = false;

        for (int i = 0; i < panes.Length; i++)
        {
            if (!found && i == index)
            {
                panes[i].SetActive(true);
                found = true;
            }
            else
                panes[i].SetActive(false);
        }
    }
}

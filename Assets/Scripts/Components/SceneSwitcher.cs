using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//For Button use
public class SceneSwitcher : MonoBehaviour
{
    public string from;
    public string to;
    private bool isdown = false;
    private void OnMouseDown() 
    {
        if(isdown) { return; }
        isdown = true;
        Switch();
    }

    public void Switch()
    {
        GLOBAL.UnloadAndLoadScene(from, to, LoadSceneMode.Additive);
    }
}

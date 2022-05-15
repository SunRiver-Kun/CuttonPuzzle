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
        if(isdown || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) { return; }
        isdown = true;
        GLOBAL.UnloadAndLoadScene(from, to, LoadSceneMode.Additive);
    }
}

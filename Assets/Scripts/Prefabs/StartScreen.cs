using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public static void NewGame()
    {
        if (Directory.Exists(GLOBAL.dataPath))
        {
            //Debug.Log("Exists");
            Directory.Delete(GLOBAL.dataPath, true);
        }
        LoadGame();
    }

    public static void ContinueGame()
    {
        LoadGame();
    }

    public static void ExitGame()
    {
        GLOBAL.Quit();
    }

    private static void LoadGame()
    {
        SceneManager.LoadScene("Persistent", LoadSceneMode.Single);
        SceneManager.LoadScene("H1", LoadSceneMode.Additive);
    }

    public Button continueButton;
    public Color ableColor;
    public Color disableColor;
    private void Start()
    {
        bool hasdata = Directory.Exists(GLOBAL.dataPath) && Directory.GetFiles(GLOBAL.dataPath).Length > 0;
        continueButton.interactable = hasdata;
        var text = continueButton.GetComponentInChildren<Text>();
        if(text!=null) { text.color = hasdata ? ableColor : disableColor; }
    }
}

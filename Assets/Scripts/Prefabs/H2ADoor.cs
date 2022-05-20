using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H2ADoor : MonoBehaviour, ISaveAndLoad
{
    public GameObject ToH3;
    
    public void OnSave()
    {
        GLOBAL.SaveData(this, SmallGameManger.win.ToString());
    }

    public void OnLoad()
    {
        string datastr = GLOBAL.LoadData(this);
        bool iswin = datastr=="" ? SmallGameManger.win : bool.Parse(datastr) || SmallGameManger.win;
        SmallGameManger.win = iswin;
        if(iswin)
        {
            gameObject.SetActive(false);
            ToH3?.SetActive(true);
        }
    }
}


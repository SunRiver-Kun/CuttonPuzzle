using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour, ISaveAndLoad
{
    bool isdestoryed = false;

    public void OnSave()
    {
        GLOBAL.SaveData(this, isdestoryed.ToString());
    }

    public void OnLoad()
    {
        string data = GLOBAL.LoadData(this);
        if (data == "") { return; }
        isdestoryed = bool.Parse(data);

        if (isdestoryed) { gameObject.SetActive(false); }
    }

    private void OnMouseDown()
    {
        Command.c_give("Mail");
        isdestoryed = true;
        gameObject.SetActive(false);
    }
}

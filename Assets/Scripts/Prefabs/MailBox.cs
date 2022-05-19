using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour, ISaveAndLoad, IItemCallBack
{
    public Sprite openSprite;
    bool hasKey = false;
    public bool OnItemAction(InventoryItem item)
    {
        if (item.itemName != "Key" || hasKey) { return false; }
        hasKey = true;
        Open();
        return true;
    }

    public void OnLoad()
    {
        string datastr = GLOBAL.LoadData(this);
        if (datastr == "") { return; }
        hasKey = bool.Parse(datastr);

        if (hasKey) { Open(); }
    }

    public void OnSave()
    {
        GLOBAL.SaveData(this, hasKey.ToString());
    }

    void Open()
    {
        var render = GetComponent<SpriteRenderer>();
        if (render) { render.sprite = openSprite; }

        transform.Find("Mail")?.gameObject.SetActive(true);
    }
}

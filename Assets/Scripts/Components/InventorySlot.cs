using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public InventoryItem item;

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        if(icon!=null)
        {
            if(item==null) { icon.enabled = false; return; }
            icon.sprite = item.icon;
            icon.SetNativeSize();
            icon.enabled = true;
        }
    }
}

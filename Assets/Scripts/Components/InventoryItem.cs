using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//本来InventoryItem应该写成组件形式的，不过算了，这个项目用不上

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public string tooltip;
    //public Inventory keeper;  GLOBAL.ThePlayer.GetComponent<Inventory>()
}

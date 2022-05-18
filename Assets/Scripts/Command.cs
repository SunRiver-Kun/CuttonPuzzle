using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承自MonoBehaviour，也方便UI使用
public class Command : MonoBehaviour
{
    public static void c_give(string itemname)
    {
        Inventory inventory = GLOBAL.ThePlayer?.GetComponent<Inventory>();
        if (inventory == null) { return; }
        inventory.AddItem(itemname);
    }

    public static void c_remove()
    {
        GLOBAL.ThePlayer?.GetComponent<Inventory>()?.RemoveCurrentItem();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot slot;  

    public InventoryItem AddItem(string itemname)
    {
        if(GLOBAL.AllItems==null) { Debug.Log("No GLOBAL.AllItems find");  return null; }
        InventoryItem item;
        if(GLOBAL.AllItems.TryGetItem(itemname, out item))
        {
            items.Add(item);
            if(slot!=null) { slot.SetItem(item); }
        }
        else
        {
            Debug.Log("No find item: " + itemname);
        }
        return item;
    }

    public void RemoveItem(InventoryItem item)
    {
        for(int i=items.Count-1; i>=0; --i)
        {
            if(items[i] == item)
            {
                if(slot!=null && slot.item==item) { slot.SetItem(items.Count==1 ? null : items[(i+1)%items.Count]); }
                items.RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveItem(string itemname, int count = 1)
    {
        for(int i=items.Count-1; i>=0 && count>0; --i)
        {
            if(items[i].itemName.Equals(itemname))
            {
                if(slot!=null && slot.item==items[i]) { slot.SetItem(items.Count==1 ? null : items[(i+1)%items.Count]); }
                items.RemoveAt(i);
                --count;
            }
        }
    }

    public void Clear()
    {
        if(slot!=null) { slot.SetItem(null); }
        items.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveAndLoad
{
    [SerializeField]
    List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot slot;

    public InventoryItem AddItem(string itemname)
    {
        if (GLOBAL.AllItems == null) { Debug.Log("No GLOBAL.AllItems find"); return null; }
        InventoryItem item;
        if (GLOBAL.AllItems.TryGetItem(itemname, out item))
        {
            items.Add(item);
            if (slot != null) { slot.SetItem(item); }
        }
        else
        {
            Debug.Log("No find item: " + itemname);
        }
        return item;
    }

    public void RemoveItem(InventoryItem item)
    {
        for (int i = items.Count - 1; i >= 0; --i)
        {
            if (items[i] == item)
            {
                if (slot != null && slot.item == item) { slot.SetItem(items.Count == 1 ? null : items[(i + 1) % items.Count]); }
                items.RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveItem(string itemname, int count = 1)
    {
        for (int i = items.Count - 1; i >= 0 && count > 0; --i)
        {
            if (items[i].itemName.Equals(itemname))
            {
                if (slot != null && slot.item == items[i]) { slot.SetItem(items.Count == 1 ? null : items[(i + 1) % items.Count]); }
                items.RemoveAt(i);
                --count;
            }
        }
    }

    public InventoryItem GetItem(string itemname)
    {
        foreach (var v in items)
        {
            if (v.itemName == itemname)
                return v;
        }
        return null;
    }

    public void Clear()
    {
        if (slot != null) { slot.SetItem(null); }
        items.Clear();
    }



    [System.Serializable]
    class Data
    {
        public List<string> itemnames = new List<string>();
        public string slotCurrentItem = "";
    }
    public void OnSave()
    {
        if (items.Count == 0) { return; }
        Data data = new Data();
        foreach (var v in items)
        {
            data.itemnames.Add(v.itemName);
        }
        if (slot != null && slot.item != null) { data.slotCurrentItem = slot.item.itemName; }
        GLOBAL.SaveData(this, JsonUtility.ToJson(data));
    }

    public void OnLoad()
    {
        string datastr = GLOBAL.LoadData(this);
        if (datastr == "") { return; }
        var data = JsonUtility.FromJson<Data>(datastr);
        foreach (var v in data.itemnames)
        {
            AddItem(v);
        }
        if (slot) { slot.SetItem(GetItem(data.slotCurrentItem)); }
    }
}

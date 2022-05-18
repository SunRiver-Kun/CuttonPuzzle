using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveAndLoad
{
    [SerializeField]
    List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot slot;
    public int currentItemIndex = 0;
    public InventoryItem AddItem(string itemname)
    {
        if (GLOBAL.AllItems == null) { Debug.Log("No GLOBAL.AllItems find"); return null; }
        InventoryItem item;
        if (GLOBAL.AllItems.TryGetItem(itemname, out item))
        {
            items.Add(item);
            if (slot != null) { slot.SetItem(item); }
            currentItemIndex = items.Count - 1;
        }
        else
        {
            Debug.Log("No find item: " + itemname);
        }
        return item;
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

    public void RemoveCurrentItem()
    {
        if (items.Count <= 1)
        {
            currentItemIndex = 0;
            slot?.SetItem(null);
        }
        else
        {
            items.RemoveAt(currentItemIndex);
            currentItemIndex = currentItemIndex % items.Count;
            slot?.SetItem(items[currentItemIndex]);
        }
    }

    public void Clear()
    {
        if (slot != null) { slot.SetItem(null); }
        items.Clear();
        currentItemIndex = 0;
    }

    public void NextItem()
    {
        if (items.Count == 0)
        {
            currentItemIndex = 0;
            slot?.SetItem(null);
        }
        else
        {
            currentItemIndex = (currentItemIndex + 1) % items.Count;
            slot?.SetItem(items[currentItemIndex]);
        }
    }

    public void LastItem()
    {
        if (items.Count == 0)
        {
            currentItemIndex = 0;
            slot?.SetItem(null);
        }
        else
        {
            currentItemIndex = items.Count - 1 - (items.Count - currentItemIndex ) % items.Count;
            slot?.SetItem(items[currentItemIndex]);
        }
    }

    [System.Serializable]
    class Data
    {
        public List<string> itemnames = new List<string>();
        public int currentItemIndex;
    }
    public void OnSave()
    {
        Data data = new Data();
        data.currentItemIndex = currentItemIndex;
        foreach (var v in items)
        {
            data.itemnames.Add(v.itemName);
        }
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
        if(items.Count==0)
        {
            currentItemIndex = 0;
            slot?.SetItem(null);
        }
        else
        {
            currentItemIndex = data.currentItemIndex % items.Count;
            slot?.SetItem(items[currentItemIndex]);
        }
    }
}

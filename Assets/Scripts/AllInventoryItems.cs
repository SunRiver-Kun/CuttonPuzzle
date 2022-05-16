using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllInventoryItems", menuName = "CottonPuzzle/AllInventoryItems", order = 0)]
public class AllInventoryItems : ScriptableObject
{
    [SerializeField]
    List<InventoryItem> items = new List<InventoryItem>();

    public InventoryItem GetItem(string itemname)
    {
        foreach (var v in items)
        {
            if(v.itemName.Equals(itemname))
                return v;
        }
        return null;
    }

    public bool TryGetItem(string itemname, out InventoryItem item)
    {
        item = GetItem(itemname);
        return item!=null;
    }
}
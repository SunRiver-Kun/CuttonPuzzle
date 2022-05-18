public interface ISaveAndLoad 
{
    void OnSave();
    void OnLoad();
}

public interface IItemCallBack 
{
    bool OnItemAction(InventoryItem item);    //返回true，则删除玩家身上对应的物品
}

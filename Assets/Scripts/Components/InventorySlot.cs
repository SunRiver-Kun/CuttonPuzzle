using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Sprite handImage;
    public GameObject toolTip;
    public Image icon;

    [System.NonSerialized]
    public InventoryItem item;  //编辑器一序列化就不为空了

    public static bool IsValidItem(InventoryItem item) => item!=null && !string.IsNullOrEmpty(item.itemName);

    GameObject handitem;

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        if (icon != null)
        {
            if (!IsValidItem(item)) { icon.enabled = false; return; }
            icon.sprite = item.icon;
            icon.SetNativeSize();
            icon.enabled = true;
        }
    }

    public void OnStartDragItem()
    {
        if(!IsValidItem(item)) { return; }

        Image image;
        if (handitem == null)
        {
            handitem = new GameObject("handitem", typeof(RectTransform));
            handitem.transform.SetParent(GLOBAL.HUD?.transform);
            handitem.AddComponent<Image>().raycastTarget = false;

            GameObject hand = new GameObject("hand", typeof(RectTransform));
            hand.transform.SetParent(handitem.transform);
            image = hand.AddComponent<Image>();
            image.raycastTarget = false;
            image.sprite = handImage;
            image.SetNativeSize();
        }
        image = handitem.GetComponent<Image>();
        image.sprite = icon?.sprite;
        image.SetNativeSize();
        handitem.SetActive(true);
    }

    public void OnEndDragItem()
    {
        handitem.SetActive(false);
        var mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(new Vector2(mousepos.x,mousepos.y));
        IItemCallBack cb = hit ? hit.gameObject.GetComponent<IItemCallBack>() : null;
        if(cb==null || !IsValidItem(item)) { return; }
        if( cb.OnItemAction(item) )
        {
            GLOBAL.ThePlayer.GetComponent<Inventory>()?.RemoveCurrentItem();
        }
    }

    public void OnPointEnter()
    {   
        if(toolTip==null || !IsValidItem(item)) { return; }
        toolTip.SetActive(true);
        toolTip.GetComponentInChildren<Text>().text = item.tooltip;
    }

    public void OnPointExit()
    {
        if(toolTip==null) { return; }
        toolTip.SetActive(false);
    }

    private void Update()
    {
        //Debug.Log(Input.mousePosition);
        
        if (handitem == null || !handitem.activeSelf) { return; }
        handitem.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) { OnEndDragItem(); }
    }
}

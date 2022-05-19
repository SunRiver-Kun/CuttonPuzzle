using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnceClickAction : MonoBehaviour, ISaveAndLoad
{
    [Tooltip("是否只能调用一次OnClicked事件")]
    public bool onceClick = true;
    public UnityEvent onClicked = new UnityEvent();
    [Tooltip("当点击过，下一次加载场景时调用")]
    public UnityEvent onSecondLoad = new UnityEvent();

    bool isclicked = false;
    public void OnLoad()
    {
        string datastr = GLOBAL.LoadData(this);
        if(datastr=="") { return; }
        isclicked = bool.Parse(datastr);
        if(isclicked) { onSecondLoad.Invoke(); }
    }

    public void OnSave()
    {
        GLOBAL.SaveData(this, isclicked.ToString());
    }

    private void OnMouseDown() 
    {
        if(onceClick && isclicked) { return; }
        isclicked = true;
        onClicked.Invoke();
    }
}

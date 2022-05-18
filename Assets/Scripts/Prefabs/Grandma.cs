using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grandma : MonoBehaviour, ISaveAndLoad, IItemCallBack
{
    public GameObject talkScreen;
    [Min(0.001f)]
    public float showTime = 2f;
    public bool hasMail = false;
    public List<string> noMailStr = new List<string>();
    public List<string> hasMailStr = new List<string>();

    float time = 0f;
    int strindex = -1;  //strindex=(strindex+1)%strs.count  return strs[strindex]
    Text text;
    Image[] images;

    public void OnSave()
    {
        GLOBAL.SaveData(this, hasMail.ToString());
    }

    public void OnLoad()
    {
        string datastr = GLOBAL.LoadData(this);
        if (datastr == "") { return; }
        hasMail = bool.Parse(datastr);
    }

    public bool OnItemAction(InventoryItem item)
    {
        if(item.itemName == "Mail")
        {
            hasMail = true;
            strindex = -1;
            OnMouseDown();
            return true;
        }
        return false;
    }

    private void OnMouseDown()
    {
        if (!talkScreen || !text) { return; }
        time = showTime;
        talkScreen.SetActive(true);
        text.text = hasMail ? GetStr(hasMailStr, ref strindex) : GetStr(noMailStr, ref strindex);
    }

    private static string GetStr(List<string> strs, ref int index)
    {
        if (strs.Count == 0) { return ""; }
        index = (index + 1) % strs.Count;
        return strs[index];
    }

    private void Start()
    {
        text = talkScreen?.GetComponentInChildren<Text>();
        images = talkScreen?.GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        if (talkScreen!=null && talkScreen.activeSelf)
        {
            time -= Time.deltaTime;
            if (images != null)
            {
                foreach (var v in images)
                {
                    Color color = v.color;
                    color.a = Mathf.Clamp01(time / showTime);
                    v.color = color;
                }
            }
            if (time < 0) { talkScreen.SetActive(false); }
        }
    }

}

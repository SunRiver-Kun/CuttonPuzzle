using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallGameManger : MonoBehaviour
{
    public static bool win = false;

    static SmallGameSlot[] slots;

    public GameObject linePrefab;
    public float lengthScale = 1f;

    public AudioClip bgm;

    public static void ResetSlots()
    {
        win = false;
        foreach (var v in slots)
        {
            v.ResetSlot();
        }
    }
    public void SpawnLines()
    {
        GameObject lines = new GameObject("Lines");
        Transform parent = lines.transform;
        foreach (var v in slots)
        {
            if (!v.Hasneighbors()) { continue; }
            foreach (var n in v.neighbors)
            {
                GameObject line = GameObject.Instantiate(linePrefab, parent);
                line.transform.localPosition = v.transform.position;

                Vector3 direction = n.transform.position - v.transform.position;
                direction.z = 0;
                Vector3 scale = line.transform.localScale;
                scale.x *= lengthScale * direction.magnitude;
                line.transform.localScale = scale;

                line.transform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
            }
        }
    }

    private bool IsWin()
    {
        if(slots==null || slots.Length==0) { return true; }
        foreach (var v in slots)
        {
            if(!v.IsSuccess())
                return false;
        }
        return true;
    }

    private void Start()
    {
        slots = FindObjectsOfType<SmallGameSlot>();
        SpawnLines();

        GLOBAL.PlayBGM(bgm);
    }

    private void Update() 
    {
        if(win) { return; }
        
        win = IsWin();
        if(win)
        {
            FindObjectOfType<SceneSwitcher>()?.Switch();
        }
    }

    private void OnDisable() 
    {
        GLOBAL.PlayDefaultBGM();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;

//Awake -> OnEnable -> OnLoad -> Start -> Update -> OnSave -> OnDisable -> OnDestroy -> OnAppicationQuit

public class GLOBAL : MonoBehaviour
{

    public static GameObject HUD;
    public static GameObject ThePlayer;
    public static AllInventoryItems AllItems;

    public static bool autoFade = true;
    public static bool autoSaveAndLoad = true;
    public static bool isQuiting { get; private set; }
    public static bool printDetailInfo = false;


    private static GLOBAL Instance;

    //为了实现在加载的数据没读完前，有手动调用OnSave的情况，需要两个容器
    //alldata[objname][componentname] -> data:string   objname必须唯一
    private static Dictionary<string, List<(string, string)>> savedata = new Dictionary<string, List<(string, string)>>();
    private static Dictionary<string, List<(string, string)>> loaddata = new Dictionary<string, List<(string, string)>>();


    //编辑器调整
    [Header("GLOBAL")]
    public AllInventoryItems allItems;
    [Header("Fade")]
    public Image fade;
    [Min(0.001f)]
    public float fadeSpeed = 10f;

    //注：被禁用或删除的物体是广播不到的（被禁用的组件还是可以收到），解决方法是手动调用OnSave()
    //OnSave()中调用GLOBAL.SaveData(this, json)来保存数据   json可以从JsonUtility.ToJson(data)中得到，当然json也就是个字符串，你随便传个字符串也行
    public static void SaveData<T>(T component, string json) where T : Component
    {
        string objname = GetGameObjectName(component);
        List<(string, string)> data;
        savedata.TryGetValue(objname, out data);
        if (data == null)
        {
            data = new List<(string, string)>();
            savedata.Add(objname, data);
        }
        data.Add((typeof(T).Name, json));
    }

    //OnLoad()中调用LoadData(this)来获取数据，找不到返回"" 
    public static string LoadData<T>(T component) where T : Component
    {
        string objname = GetGameObjectName(component);
        List<(string, string)> data;
        if (!loaddata.TryGetValue(objname, out data) || data == null) { return ""; };
        string compname = typeof(T).Name;
        string res = "";
        for (int i = data.Count - 1; i >= 0; --i)
        {
            var v = data[i];
            if (v.Item1 == compname)
            {
                res = v.Item2;
                data.RemoveAt(i);
                break;
            }
        }
        if (data.Count == 0) { loaddata.Remove(objname); }
        return res;
    }

    //注：文件保存格式和文件编码必须统一！ 
    //如果用广播被禁用的物体就接收不到，所以换了种写法
    //保存场景数据
    public static void Save(Scene scene)
    {
        if (scene == null) { return; }
        if (printDetailInfo) { Debug.Log("Saving scene: " + scene.name); }
        savedata.Clear();
        GameObject[] objs = scene.GetRootGameObjects();
        foreach (GameObject inst in objs)
        {
            foreach (var v in inst.GetComponentsInChildren<ISaveAndLoad>(true))
            {
                v.OnSave();
            }
            //inst.BroadcastMessage("OnSave", SendMessageOptions.DontRequireReceiver);
        }
        if (printDetailInfo) { PrintAllData(savedata); }
        if (savedata.Count == 0) { return; }

        string path = GetSceneDataPath(scene);
        FileInfo info = new FileInfo(path);
        if (!info.Directory.Exists) { info.Directory.Create(); }
        using (var stream = File.Open(path, FileMode.Create))   //OpenWriter并不会自动清空
        {
            BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8);
            foreach (var v in savedata)
            {
                if (v.Value == null || v.Value.Count == 0) { continue; }

                writer.Write(v.Key);
                writer.Write(v.Value.Count);
                foreach (var data in v.Value)
                {
                    writer.Write(data.Item1);
                    writer.Write(data.Item2);
                }
            }
            writer.Close();
        }
        Debug.Log("Saving scene success: " + scene.name);
    }

    //加载场景数据
    public static void Load(Scene scene)
    {
        if (scene == null) { return; }
        if (printDetailInfo) { Debug.Log("Loading scene: " + scene.name); }
        string path = GetSceneDataPath(scene);
        if (!File.Exists(path)) { return; }

        loaddata.Clear();  //加载之前要清空缓存数据
        using (var stream = File.OpenRead(path))
        {
            BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8);
            bool eof = false;
            while (!eof)   //还是用try catch好了，避免某人手动改数据，然后读取错误
            {
                try
                {
                    string objname = reader.ReadString();
                    int datalength = reader.ReadInt32();
                    List<(string, string)> data = new List<(string, string)>();
                    for (int i = 0; i < datalength; ++i)
                    {
                        (string, string) v;
                        v.Item1 = reader.ReadString();
                        v.Item2 = reader.ReadString();
                        data.Add(v);
                    }
                    loaddata.Add(objname, data);
                }
                catch (System.Exception)
                {
                    eof = true;
                    reader.Close();
                }
            }
        }

        if (printDetailInfo) { PrintAllData(loaddata); }
        if (loaddata.Count == 0) { return; }

        GameObject[] objs = scene.GetRootGameObjects();
        foreach (GameObject inst in objs)
        {
            foreach (var v in inst.GetComponentsInChildren<ISaveAndLoad>(true))
            {
                v.OnLoad();
            }
            //inst.BroadcastMessage("OnLoad", SendMessageOptions.DontRequireReceiver);
        }
        Debug.Log("Loading scene success: " + scene.name);
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public static void SaveAll()
    {
        for (int i = SceneManager.sceneCount - 1; i >= 0; --i)
        {
            Save(SceneManager.GetSceneAt(i));
        }
    }
    public static void SaveAndQuit()
    {
        if (isQuiting) { return; }
        isQuiting = true;
        SaveAll();
        Quit();
    }

    //场景加载和卸载，只是添加了Fade。保存在CustomSceneManagerAPI.cs
    public static void LoadScene(string scenename, LoadSceneMode mode = LoadSceneMode.Single)
    {
        Instance.StartCoroutine(Instance.__LoadScene(scenename, mode));
    }

    public static void UnloadScene(string scenename)
    {
        Instance.StartCoroutine(Instance.__UnloadScene(scenename));
    }

    public static void UnloadAndLoadScene(string unloadscene, string loadscene, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        Instance.StartCoroutine(Instance.__UnloadAndLoadScene(unloadscene, loadscene, mode));
    }

    //全局初始化
    private void Awake()
    {
        //初始化全局变量
        Assert.IsNull(Instance);
        Instance = this;
        Assert.IsNull(HUD);
        HUD = GameObject.FindGameObjectWithTag("HUD");
        Assert.IsNull(ThePlayer);
        ThePlayer = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNull(AllItems);
        AllItems = allItems;
    }

    private static void PrintAllData(Dictionary<string, List<(string, string)>> totaldata)
    {
        foreach (var v in totaldata)
        {
            Debug.Log("Start: " + v.Key + " Total: " + v.Value.Count);
            foreach (var data in v.Value)
            {
                Debug.Log(data.Item1 + " = " + data.Item2);
            }
            Debug.Log("End: " + v.Key);
        }
    }

    //通过组件获得存储时GameObject的名字
    private static string GetGameObjectName(Component component)
    {
        return component.transform.parent == null ? component.gameObject.name : GetGameObjectName(component.transform.parent) + "." + component.gameObject.name;
    }

    //获取场景存档数据文件名
    private static string GetSceneDataPath(Scene scene)
    {
        return Application.dataPath + "/local/data/" + scene.name + ".data";
    }

    private IEnumerator FadeTo(float alpha, bool keepactive = true)
    {
        if (fade == null) { yield break; }
        fade.gameObject.SetActive(true);

        alpha = Mathf.Clamp01(alpha);
        Color color = fade.color;
        while (!Mathf.Approximately(color.a, alpha))
        {
            color.a = Mathf.MoveTowards(color.a, alpha, Time.deltaTime * fadeSpeed);
            fade.color = color;
            yield return null;
        }

        fade.gameObject.SetActive(keepactive);
    }

    private IEnumerator __LoadScene(string scenename, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(scenename, mode);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        if (autoFade) { yield return FadeTo(0, false); }
    }

    private IEnumerator __UnloadScene(string scenename)
    {
        if (autoFade) { yield return FadeTo(1, true); }
        yield return SceneManager.UnloadSceneAsync(scenename);
        if (SceneManager.sceneCount > 0) { SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1)); }
    }

    private IEnumerator __UnloadAndLoadScene(string unloadscenename, string loadscenename, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        if (!string.IsNullOrEmpty(unloadscenename)) { yield return __UnloadScene(unloadscenename); }
        if (!string.IsNullOrEmpty(loadscenename)) { yield return __LoadScene(loadscenename, mode); }
    }
}

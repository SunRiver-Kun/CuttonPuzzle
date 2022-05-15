using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GLOBAL : MonoBehaviour
{

    public static GameObject HUD;
    public static GameObject ThePlayer;

    public static bool autoFade = true;
    public static bool autoSaveAndLoad = true;

    private static GLOBAL Instance;
    //totaldata[objname][componentname] -> data:string   objname必须唯一
    private static Dictionary<string, List<(string, string)>> totaldata = new Dictionary<string, List<(string, string)>>();
    private static bool isquiting = false;

    //编辑器调整
    public Image fade;
    [Min(0.001f)]
    public float fadeSpeed = 10f;

    //OnSave()中调用GLOBAL.SaveData(this, json)来保存数据   json可以从JsonUtility.ToJson(data)中得到，当然json也就是个字符串，你随便传个字符串也行
    public static void SaveData<T>(T component, string json) where T : Component
    {
        string objname = GetGameObjectName(component);
        List<(string, string)> data;
        totaldata.TryGetValue(objname, out data);
        if (data == null)
        {
            data = new List<(string, string)>();
            totaldata.Add(objname, data);
        }
        data.Add((typeof(T).Name, json));
    }

    //OnLoad()中调用LoadData(this)来获取数据，找不到返回"" 
    public static string LoadData<T>(T component) where T : Component
    {
        string objname = GetGameObjectName(component);
        List<(string, string)> data;
        if (!totaldata.TryGetValue(objname, out data) || data == null) { return ""; };
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
        if (data.Count == 0) { totaldata.Remove(objname); }
        return res;
    }

    //保存场景数据
    public static void Save(Scene scene)
    {
        if (scene == null) { return; }
        Debug.Log("Saving scene: " + scene.name);
        totaldata.Clear();
        GameObject[] objs = scene.GetRootGameObjects();
        foreach (GameObject inst in objs)
        {
            inst.BroadcastMessage("OnSave", SendMessageOptions.DontRequireReceiver);
        }
        if (totaldata.Count == 0) { return; }

        string path = GetSceneDataPath(scene);
        FileInfo info = new FileInfo(path);
        if (!info.Directory.Exists) { info.Directory.Create(); }
        using (var stream = File.OpenWrite(path))
        {
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (var v in totaldata)
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
        Debug.Log("Loading scene: " + scene.name);
        string path = GetSceneDataPath(scene);
        if (!File.Exists(path)) { return; }

        totaldata.Clear();
        using (var stream = File.OpenRead(path))
        {
            BinaryReader reader = new BinaryReader(stream);
            while (reader.PeekChar() != -1)
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
                totaldata.Add(objname, data);
            }
            reader.Close();
        }
        if (totaldata.Count == 0) { return; }

        GameObject[] objs = scene.GetRootGameObjects();
        foreach (GameObject inst in objs)
        {
            inst.BroadcastMessage("OnLoad", SendMessageOptions.DontRequireReceiver);
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

    public static void SaveAndQuit()
    {
        if (isquiting) { return; }
        isquiting = true;

        for (int i = SceneManager.sceneCount - 1; i >= 0; --i)
        {
            Save(SceneManager.GetSceneAt(i));
        }
        Quit();
    }

    //场景加载
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

        //自定义场景加载
        for (int i = SceneManager.sceneCount - 1; i >= 0; --i)
        {
            Load(SceneManager.GetSceneAt(i));
        }
        SceneManagerAPI.overrideAPI = new CustomSceneManagerAPI();
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
        if (autoFade) { yield return FadeTo(0, false); }
    }

    private IEnumerator __UnloadScene(string scenename)
    {
        if (autoFade) { yield return FadeTo(1, true); }
        yield return SceneManager.UnloadSceneAsync(scenename);
    }

    private IEnumerator __UnloadAndLoadScene(string unloadscenename, string loadscenename, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        yield return __UnloadScene(unloadscenename);
        yield return __LoadScene(loadscenename, mode);
    }
}

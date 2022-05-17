using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class CustomSceneManagerAPI : SceneManagerAPI
{
    // 编辑器直接进入PlayMode时并不会自动调用LoadScene，所有应该用SceneManager.sceneLoaded += fn
    // protected override AsyncOperation LoadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
    // {
    //     var res = base.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);
    //     res.completed += (AsyncOperation _) =>
    //     {
    //         if (GLOBAL.autoSaveAndLoad) { GLOBAL.Load(sceneBuildIndex < 0 ? SceneManager.GetSceneByName(sceneName) : SceneManager.GetSceneByBuildIndex(sceneBuildIndex)); }
    //     };
    //     return res;
    // }

    //Awake -> OnEnable -> OnLoad -> Start -> Update -> OnSave -> OnDisable -> OnDestroy -> OnAppicationQuit
    public static event Action<Scene, LoadSceneMode> sceneloaded;
    public static event Action<Scene> beforeSceneUnloaded;
    //SceneManager.sceneUnloaded  发生在OnDestroy之后
    protected static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GLOBAL.autoSaveAndLoad) { GLOBAL.Load(scene); }
        if (sceneloaded != null) { sceneloaded(scene, mode); }
    }
    protected override AsyncOperation UnloadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
    {
        Scene scene = sceneBuildIndex < 0 ? SceneManager.GetSceneByName(sceneName) : SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
        if (GLOBAL.autoSaveAndLoad) { GLOBAL.Save(scene); }
        if (beforeSceneUnloaded != null) { beforeSceneUnloaded(scene); }
        return base.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded 发生在OnDestroy后，所有要从调用UnLoadScene入手
        SceneManagerAPI.overrideAPI = new CustomSceneManagerAPI();

        //编辑器直接结束并不会自动调用Unload场景，所以要加上这个
#if UNITY_EDITOR
        Application.wantsToQuit += () =>
{
    if (GLOBAL.isQuiting) { return true; }  //按右上角退出，避免多次保存
    if (GLOBAL.autoSaveAndLoad) { GLOBAL.SaveAll(); }
    return true;
};
#endif
    }
}
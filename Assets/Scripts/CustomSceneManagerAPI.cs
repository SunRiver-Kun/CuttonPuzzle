using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManagerAPI : SceneManagerAPI
{
    protected override AsyncOperation LoadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
    {
        var res = base.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);
        res.completed += (AsyncOperation _) =>
        {
            if (GLOBAL.autoSaveAndLoad) { GLOBAL.Load(sceneBuildIndex < 0 ? SceneManager.GetSceneByName(sceneName) : SceneManager.GetSceneByBuildIndex(sceneBuildIndex)); }
        };
        return res;
    }

    protected override AsyncOperation UnloadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
    {
        if (GLOBAL.autoSaveAndLoad) { GLOBAL.Save(sceneBuildIndex < 0 ? SceneManager.GetSceneByName(sceneName) : SceneManager.GetSceneByBuildIndex(sceneBuildIndex)); }
        return base.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
    }
}
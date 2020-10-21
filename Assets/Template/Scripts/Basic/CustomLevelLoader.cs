using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomLevelLoader : MonoBehaviour
{
    public static bool NeedPause = false;
    public static bool AppPause = false;

    static string SceneNameToLoad = "";
    static int SceneIndexToLoad = -1;
    static bool IsSameScene = false;

    public LocalizeUIText Text;

    public string StartScene = "Menu";

    [Header("Pause Events")]
    public bool HandleBannerClick = true;
    public bool HandleFullscreenImpression = true;
    public bool HandleAppPause = true;

    [Header("Editor")]
    public float LoadWaitTime = 0;

    string SceneName = "";
    int SceneIndex = -1;

    // Use this for initialization
    void Awake()
    {
        //Debug.Log("load");
        NeedPause = false;
        AppPause = false;

        if (SceneNameToLoad == "" && SceneIndexToLoad == -1)
            SceneName = StartScene;
        else if (SceneNameToLoad != "")
            SceneName = SceneNameToLoad;
        else
            SceneIndex = SceneIndexToLoad;


        Time.timeScale = 1;
        if (UiInit.InitComplete)
        {
            Do();
        }
        else
            CustomCode.CustomResourcesManager.EndInit += Do;
    }

    void Do()
    {
        Subscribe();
        StartCoroutine("DoRoutine");
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        if (HandleAppPause)
        {
            AMEvents.AMEvents.PauseEvent += SetNeedAppPause;
            AMEvents.AMEvents.ResumeEvent += UnsetNeedAppPause;
        }
        if (HandleBannerClick)
            AMEvents.Ad.CrossClicked += SetNeedPause;
        if (HandleFullscreenImpression)
            AMEvents.Ad.Interstitial.Impression += SetNeedPause;

#if UNITY_EDITOR
        if (HandleAppPause)
        {
            StaticEventHelper.PauseEvent += SetNeedAppPause;
            StaticEventHelper.ResumeEvent += UnsetNeedAppPause;
        }
        if (HandleBannerClick)
        {
            StaticEventHelper.ShowBannerEvent += SetNeedPause;
        }
        if (HandleFullscreenImpression)
        {
            StaticEventHelper.ShowBannerEvent += SetNeedPause;
        }
#endif
    }

    void Unsubscribe()
    {
        if (HandleAppPause)
        {
            AMEvents.AMEvents.PauseEvent -= SetNeedAppPause;
            AMEvents.AMEvents.ResumeEvent -= UnsetNeedAppPause;
        }
        if (HandleBannerClick)
            AMEvents.Ad.CrossClicked -= SetNeedPause;
        if (HandleFullscreenImpression)
            AMEvents.Ad.Interstitial.Impression -= SetNeedPause;

#if UNITY_EDITOR
        if (HandleAppPause)
        {
            StaticEventHelper.PauseEvent -= SetNeedAppPause;
            StaticEventHelper.ResumeEvent -= UnsetNeedAppPause;
        }
        if (HandleBannerClick)
        {
            StaticEventHelper.ShowBannerEvent -= SetNeedPause;
        }
        if (HandleFullscreenImpression)
        {
            StaticEventHelper.ShowBannerEvent -= SetNeedPause;
        }
#endif
    }

    void SetNeedAppPause()
    {
        NeedPause = true;
        AppPause = true;
    }

    void UnsetNeedAppPause()
    {
        NeedPause = false;
        AppPause = false;
    }

    void SetNeedPause()
    {
        NeedPause = true;
    }

    void UnsetNeedPause()
    {
        NeedPause = false;
    }

    IEnumerator DoRoutine()
    {
        if (!Localizator.isInited)
            Localizator.Init();
        UiInit.InitComplete = true;
        if (Text)
            Text.ForceUpdate();

        if (!IsSameScene)
        {
            var unloader = Resources.UnloadUnusedAssets();
            while (!unloader.isDone)
                yield return null;
        }

        System.GC.Collect();
        //Debug.LogError(SceneName);
        Invoke("Load", Application.isEditor ? LoadWaitTime : 0.1f);
    }


    void Load()
    {
#if UNITY_5
		LoadAsync();
#else

        if (Application.HasProLicense())
            LoadAsync();
        else
            LoadNormal();
#endif
    }

    void LoadAsync()
    {
        //Debug.LogError(SceneName);
        if (SceneName != "")
        {
            Application.LoadLevelAsync(SceneName);
        }
        else
        {
            Application.LoadLevelAsync(SceneIndex);
        }
    }

    void LoadNormal()
    {
        if (SceneName != "")
        {
            Application.LoadLevel(SceneName);
        }
        else
        {
            Application.LoadLevel(SceneIndex);
        }
    }

    public static void LoadLevel(string scene_name)
    {
        SceneNameToLoad = scene_name;
        SceneIndexToLoad = -1;
        LoadStatic();
    }

    public static void LoadLevel(int index)
    {
        SceneNameToLoad = "";
        SceneIndexToLoad = index;
        LoadStatic();
    }

    static void LoadStatic()
    {
        if ((Application.loadedLevelName == SceneNameToLoad) || (Application.loadedLevel == SceneIndexToLoad))
            IsSameScene = true;
        Application.LoadLevel("Loading");
    }
}

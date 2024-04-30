using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour,ISaveable
{
   // public PlayerStatBar playerStatBar;
   // public PlayerController inputControl;
    public Transform playerTrans;//获取player的坐标
    public Vector3 firstPosition;//初始坐标
    public Vector3 menuPosition;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unloadedSceneEvent;


    [Header("场景")]
    public GameSceneSO firstLoadScene;//第一个场景
    public GameSceneSO menuScene;
    public GameSceneSO currentLoadScene;//当前加载的场景
    private GameSceneSO sceneToLoad;//第二个场景
    private Vector3 positionToGo;//player即将传送的位置
    private bool fadeScreen;//是否渐入渐出
    private bool isLoading;//是否场景加载ing
    public float fadeDuration;

    private void Awake()
    {
        // Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        //currentLoadScene = firstLoadScene;
        //currentLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
      
    }

    private void Start()
    {
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
        //  OnLoadRequestEvent(menuScene, menuPosition, true);
        //  NewGame();
    }
   
    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
        

    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

   
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        // OnLoadRequestEvent(sceneToLoad,firstPosition, true);
       loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }
    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
   private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadScene != null)
            StartCoroutine(UnLoadPreviousScene());//调用协程
        else
            LoadNewScene();//为空则加载新场景
      //  Debug.Log(sceneToLoad.sceneReference.SubObjectName);

    }

    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScreen)
        {
            //画布逐渐变黑，卸载场景
            fadeEvent.FadeIn(fadeDuration);

        }
        yield return new WaitForSeconds(fadeDuration);

        //广播事件调整血条显示
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);
        
         yield return currentLoadScene.sceneReference.UnLoadScene();//卸载场景
        //关闭人物
        playerTrans.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
       var loadingOption= sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);//加载新场景
        loadingOption.Completed += OnLoadComplete;
    }
    /// <summary>
    /// 场景加载后完成
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadScene = sceneToLoad;
        playerTrans.position = positionToGo;//player坐标改变成新场景的正确坐标
        //移动坐标后再启动player
        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            //画布逐渐变透明，显现场景
            fadeEvent.FadeOut(fadeDuration);
        }
        isLoading = false;

        if (currentLoadScene.sceneType != SceneType.Menu)
            //场景加载完成后事件
            afterSceneLoadedEvent.RaiseEvent();
     

    }

    public DataDifination GetDataID()
    {
        return GetComponent<DataDifination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
      
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDifination>().ID;
        if(data.characterPosDict.ContainsKey(playerID))//如果包含这个player的ID,即存在这个player
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();//先保存储存点读取的坐标以免它刷新
            sceneToLoad = data.GetSaveScene();//保存该player的储存点的scene

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
       
    }
}

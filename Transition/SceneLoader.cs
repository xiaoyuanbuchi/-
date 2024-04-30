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
    public Transform playerTrans;//��ȡplayer������
    public Vector3 firstPosition;//��ʼ����
    public Vector3 menuPosition;

    [Header("�¼�����")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("�㲥")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unloadedSceneEvent;


    [Header("����")]
    public GameSceneSO firstLoadScene;//��һ������
    public GameSceneSO menuScene;
    public GameSceneSO currentLoadScene;//��ǰ���صĳ���
    private GameSceneSO sceneToLoad;//�ڶ�������
    private Vector3 positionToGo;//player�������͵�λ��
    private bool fadeScreen;//�Ƿ��뽥��
    private bool isLoading;//�Ƿ񳡾�����ing
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
    /// ���������¼�����
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
            StartCoroutine(UnLoadPreviousScene());//����Э��
        else
            LoadNewScene();//Ϊ��������³���
      //  Debug.Log(sceneToLoad.sceneReference.SubObjectName);

    }

    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScreen)
        {
            //�����𽥱�ڣ�ж�س���
            fadeEvent.FadeIn(fadeDuration);

        }
        yield return new WaitForSeconds(fadeDuration);

        //�㲥�¼�����Ѫ����ʾ
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);
        
         yield return currentLoadScene.sceneReference.UnLoadScene();//ж�س���
        //�ر�����
        playerTrans.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
       var loadingOption= sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);//�����³���
        loadingOption.Completed += OnLoadComplete;
    }
    /// <summary>
    /// �������غ����
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadScene = sceneToLoad;
        playerTrans.position = positionToGo;//player����ı���³�������ȷ����
        //�ƶ������������player
        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            //�����𽥱�͸�������ֳ���
            fadeEvent.FadeOut(fadeDuration);
        }
        isLoading = false;

        if (currentLoadScene.sceneType != SceneType.Menu)
            //����������ɺ��¼�
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
        if(data.characterPosDict.ContainsKey(playerID))//����������player��ID,���������player
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();//�ȱ��洢����ȡ������������ˢ��
            sceneToLoad = data.GetSaveScene();//�����player�Ĵ�����scene

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
       
    }
}

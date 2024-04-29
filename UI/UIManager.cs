using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;
    public Potion potion;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public PlayerControllerEventSO HpRecoverEvent;
    public SceneLoadEventSO UnLoadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("广播")]
    public VoidEventSO pauseEvent;


    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public Button settingBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;
    private void Awake()
    {
        settingBtn.onClick.AddListener(TogglePausePanel);//添加事件
    }
    private void OnEnable()
    {
        healthEvent.OnEventeRaised += OnHealthEvent;
        HpRecoverEvent.OnEventeRaised += OnHpRecoverEvent;
        UnLoadedSceneEvent.LoadRequestEvent += OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }

    private void OnUnLoadedSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        var isMenu = sceneToLoad.sceneType == SceneType.Menu;
        playerStatBar.gameObject.SetActive(!isMenu);
    }

    private void OnDisable()
    {
        healthEvent.OnEventeRaised -= OnHealthEvent;
        UnLoadedSceneEvent.LoadRequestEvent -= OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void TogglePausePanel()
    {
        if(pausePanel.activeInHierarchy)//在hierarchy中是激活的那就点击关掉，反之开启
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;//游戏开启
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0;//游戏暂停
        }
    }
    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);//gameover的时候结束面板显示
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);//加载数据的时候，结束界面就不显示了
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(persentage);
        playerStatBar.OnPowerChange(character);
    }
    private void OnHpRecoverEvent(PlayerController playerController)
    {
        potion.checkCanRecover(playerController);
    }
}

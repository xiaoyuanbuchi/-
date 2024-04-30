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

    [Header("�¼�����")]
    public CharacterEventSO healthEvent;
    public PlayerControllerEventSO HpRecoverEvent;
    public SceneLoadEventSO UnLoadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("�㲥")]
    public VoidEventSO pauseEvent;


    [Header("���")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public Button settingBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;
    private void Awake()
    {
        settingBtn.onClick.AddListener(TogglePausePanel);//����¼�
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
        if(pausePanel.activeInHierarchy)//��hierarchy���Ǽ�����Ǿ͵���ص�����֮����
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;//��Ϸ����
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0;//��Ϸ��ͣ
        }
    }
    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);//gameover��ʱ����������ʾ
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);//�������ݵ�ʱ�򣬽�������Ͳ���ʾ��
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

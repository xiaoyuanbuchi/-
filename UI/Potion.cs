using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public Character character;
    public Sprite[] potionPic;//用于存储血瓶素材
    public int potionLevel;//血瓶等级
    public float initialPotionRecover;//初始恢复量
    public float initialRecoverSpeed;//初始恢复速度
    public int maxPotionNumOfUse;//最大使用次数

    private Image image;
    private PlayerController currentPlayerController;
    private AudioSource audioSource;
    private int currentPotionNumOfUse;//剩余使用次数
    private float potionRecover;//实际恢复值
    private float potionRecoverSpeed;//实际恢复速度
    private float targetHP;//目标血量
    private bool canRecover;//判断能否恢复
    private void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        currentPotionNumOfUse = maxPotionNumOfUse;
        potionRecover = potionLevel * initialPotionRecover;
        potionRecoverSpeed = potionLevel * initialRecoverSpeed;
        image.sprite = potionPic[(potionLevel - 1) * 4 + currentPotionNumOfUse];
    }
    private void Update()
    {
        if (currentPotionNumOfUse >= 0 && canRecover)
            Recover();
    }
    public void Recover()
    {
        character.currentHealth += potionRecoverSpeed * Time.deltaTime;
        character.OnHealthChange?.Invoke(character);
        if (character.currentHealth >= targetHP || !currentPlayerController.isDrinking)
        {
            audioSource.Stop();
            canRecover = false;
            currentPlayerController.toRun();//恢复角色速度
            currentPlayerController.isDrinking = false;
        }
    }
    public void checkCanRecover(PlayerController playerController)
    {
        currentPlayerController = playerController;
        bool HpIsFull = character.currentHealth == character.maxHealth;
        if (currentPotionNumOfUse == 0 || !currentPlayerController.isDrinking || HpIsFull)//判断能否使用血瓶
            return;

        targetHP = character.currentHealth + potionRecover;//获取目标血量

        if (targetHP > character.maxHealth)
            targetHP = character.maxHealth;
        currentPlayerController.toWalk();//使用血瓶时减慢移速
        currentPotionNumOfUse--;
        canRecover = true;
        image.sprite = potionPic[(potionLevel - 1) * 4 + currentPotionNumOfUse];//更改血瓶图片
        audioSource.Play();
    }
}

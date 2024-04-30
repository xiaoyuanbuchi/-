using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public Character character;
    public Sprite[] potionPic;//���ڴ洢Ѫƿ�ز�
    public int potionLevel;//Ѫƿ�ȼ�
    public float initialPotionRecover;//��ʼ�ָ���
    public float initialRecoverSpeed;//��ʼ�ָ��ٶ�
    public int maxPotionNumOfUse;//���ʹ�ô���

    private Image image;
    private PlayerController currentPlayerController;
    private AudioSource audioSource;
    private int currentPotionNumOfUse;//ʣ��ʹ�ô���
    private float potionRecover;//ʵ�ʻָ�ֵ
    private float potionRecoverSpeed;//ʵ�ʻָ��ٶ�
    private float targetHP;//Ŀ��Ѫ��
    private bool canRecover;//�ж��ܷ�ָ�
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
            currentPlayerController.toRun();//�ָ���ɫ�ٶ�
            currentPlayerController.isDrinking = false;
        }
    }
    public void checkCanRecover(PlayerController playerController)
    {
        currentPlayerController = playerController;
        bool HpIsFull = character.currentHealth == character.maxHealth;
        if (currentPotionNumOfUse == 0 || !currentPlayerController.isDrinking || HpIsFull)//�ж��ܷ�ʹ��Ѫƿ
            return;

        targetHP = character.currentHealth + potionRecover;//��ȡĿ��Ѫ��

        if (targetHP > character.maxHealth)
            targetHP = character.maxHealth;
        currentPlayerController.toWalk();//ʹ��Ѫƿʱ��������
        currentPotionNumOfUse--;
        canRecover = true;
        image.sprite = potionPic[(potionLevel - 1) * 4 + currentPotionNumOfUse];//����ѪƿͼƬ
        audioSource.Play();
    }
}

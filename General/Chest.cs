using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;//��ͼƬ

    public Sprite closeSprite;//�պ�ͼƬ

    public bool isDone;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;//�жϷŴ򿪵�ͼƬ���ǹرյ�ͼƬ

        
    }
    public void TriggerAction()
    {
        if(!isDone)
        {
            OpenChest();
        }
    }
    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        this.gameObject.tag = "Untagged";//����󣬽�ͷ����ʶȥ��
    }
}

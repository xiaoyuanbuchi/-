using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour,IInteractable
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;

    [Header("变量参数")]
    public SpriteRenderer spriteRenderer;

    public GameObject lightobj;
    public Sprite darkSprite;
    public Sprite lightSprite;

    public bool isDone;

 

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightobj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if(!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightobj.SetActive(true);
            //todo：保存数据
            saveDataEvent.RaiseEvent();
            this.gameObject.tag = "Untagged";
        }
    }
}

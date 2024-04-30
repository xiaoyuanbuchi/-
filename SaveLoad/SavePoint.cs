using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour,IInteractable
{
    [Header("�㲥")]
    public VoidEventSO saveDataEvent;

    [Header("��������")]
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
            //todo����������
            saveDataEvent.RaiseEvent();
            this.gameObject.tag = "Untagged";
        }
    }
}

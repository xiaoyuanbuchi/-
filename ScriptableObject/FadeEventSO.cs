using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName ="Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float, bool> OnEventRaised;
    /// <summary>
    /// �𽥱��
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black, duration, true);
    }
    /// <summary>
    /// �𽥱�͸��(������
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear, duration, false);
    }

    public void RaiseEvent(Color target, float duration, bool fadeIn) //fadein����false��͸������֮
    {
        OnEventRaised?.Invoke(target, duration, fadeIn);
    }

       
}

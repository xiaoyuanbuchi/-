using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventeRaised;
    public void RaiseEvent(Character character)
    {
        OnEventeRaised?.Invoke(character);
    }
}

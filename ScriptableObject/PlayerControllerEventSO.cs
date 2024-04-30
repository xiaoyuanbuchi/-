using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Event/PlayerControllerEventSO")]
public class PlayerControllerEventSO : ScriptableObject
{
    public UnityAction<PlayerController> OnEventeRaised;
    public void RaiseEvent(PlayerController playerController)
    {
        OnEventeRaised?.Invoke(playerController);
    }
}

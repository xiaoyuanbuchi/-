using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    private Character currentCharacter;
    private bool isRecovering;
    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
            healthDelayImage.fillAmount -= Time.deltaTime/4;

        if (healthDelayImage.fillAmount < healthImage.fillAmount)
            healthDelayImage.fillAmount = healthImage.fillAmount;

        if (isRecovering)
        {
            float persentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = persentage;
            if (persentage >= 1)
                isRecovering = false;
        }
    }
    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }
    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}

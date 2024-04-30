using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAudioController : MonoBehaviour
{
    protected AudioSource audioSource;
    public AudioClip jumpClip;
    public float jumpSoundVol;
    public AudioClip moveClip;
    public float moveSoundVol;
    public AudioClip sildeClip;
    public float sildeSoundVol;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayJumpClip()
    {
        PlayClip(jumpClip, jumpSoundVol);
    }
    public void PlayMoveClip(bool isMoveing)
    {
        if (!audioSource.isPlaying && isMoveing == true)
            audioSource.Play();
    }
    public void PlaySildeClip()
    {
        PlayClip(sildeClip, sildeSoundVol);
    }

    private void PlayClip(AudioClip audioclip,float soundVol)
    {
        audioSource.PlayOneShot(audioclip, soundVol);
    }
}

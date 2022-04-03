using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Light FlameThrower")]
    [SerializeField] AudioClip lightFTclip;
    [SerializeField][Range(0f, 1f)] float lightFTclipVolume = 1f;

    [Header("FlameThrower Firing")]
    [SerializeField] AudioClip fireFTClip;
    [SerializeField][Range(0f, 1f)] float fireFTVolume = 1f;

    [SerializeField] AudioSource audioSource;

    public void PlayLightFTclip()
    {
        if (lightFTclip == null) { return; }
        PlayClip(lightFTclip, lightFTclipVolume, true);
    }

    public void PlayFireFTClip()
    {
        if (fireFTClip == null) { return; }
        PlayClip(fireFTClip, fireFTVolume, true);
    }

    void PlayClip(AudioClip audioclip, float volume, bool loop)
    {
        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.loop = false;
    }
}

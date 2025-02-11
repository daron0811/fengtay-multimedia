using System.Collections.Generic;
using Unity.VisualScripting;
using UnityCommunity.UnitySingleton;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource audioSource;
    public AudioSource bgmSource;
    public List<AudioClip> audioClips;
    /*
    00: 一般按鈕
    01: 說明按鈕音效
    02: 拖曳成功
    03: 拖曳失敗
    04:放到正確的食材料理
    05:試吃掃描逼逼聲
    06:合格
    07:不合格
    */
    public List<AudioClip> bgmClips;

    public void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayBGM(int index)
    {
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void PlayAudioOnce(int index)
    {
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //單例模式
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource bgmAudioSource;

    [SerializeField] List<AudioSource> sfxAudioSource;
    Queue<AudioSource> sfxQueue;

    [SerializeField] List<AudioGroup> audioGroup;

    [Serializable]
    private class AudioGroup
    {
        public string audioName;
        public AudioClip clip;
    }

    void Awake()
    {
        //單例模式建構
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //將所有用於SFX的音效播放器統一放入一個Queue中方便管理
        sfxQueue = new Queue<AudioSource>(sfxAudioSource);
        
    }
    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(WaitAll());
    }

    IEnumerator WaitAll()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnPlayBGM += PlayAudioBGM;
        FightManager.Instance.OnPlayAudioForChip += PlayAudioForChip;
        FightManager.Instance.OnPlayAudioForName += PlayAudioForName;        
    }

    void OnDisable()
    {
        if (FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayBGM -= PlayAudioBGM;

            FightManager.Instance.OnPlayAudioForChip -= PlayAudioForChip;
            FightManager.Instance.OnPlayAudioForName -= PlayAudioForName;
        }        
    }    

    //播放BGM
    public void PlayAudioBGM(AudioClip clip)
    {
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
        if (clip != null)
        {
            
        }
    }

    //根據AudioClip播放SFX
    public void PlayAudioForChip(AudioClip clip)
    {
        if (clip == null || sfxQueue.Count == 0)
        {
            return;
        }

        AudioSource src = sfxQueue.Dequeue();
        src.clip = clip;
        src.Play();

        StartCoroutine(ReturnAfterPlay(src));
    }

    //根據name播放SFX
    public void PlayAudioForName(string name)
    {
        if (string.IsNullOrEmpty(name) || sfxQueue.Count == 0)
        {
            return;
        }
        
        AudioGroup group = audioGroup.Find(g => g.audioName == name);
        if (group != null && group.clip != null)
        {
            AudioSource src = sfxQueue.Dequeue();
            src.clip = group.clip;
            src.Play();

            StartCoroutine(ReturnAfterPlay(src));
        }         
    }

    IEnumerator ReturnAfterPlay(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        sfxQueue.Enqueue(src);
    }
}

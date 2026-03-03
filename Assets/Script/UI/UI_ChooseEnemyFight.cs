using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChooseEnemyFight : MonoBehaviour
{
    [SerializeField] AudioClip bgm;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());  
    }

    IEnumerator Wait()
    {
        yield return new WaitUntil(() => AudioManager.Instance != null);
        UpdateAudio_Bgm();                
    }

    void UpdateAudio_Bgm()
    {
        AudioManager.Instance.PlayAudioBGM(bgm);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FightOver : MonoBehaviour
{
    [SerializeField] GameObject background;

    [SerializeField] Text winText;

    [SerializeField] Text lostText;

    [SerializeField] Button yesButton;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());  
    }

    IEnumerator Wait()
    {
        background.SetActive(false);
        winText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);

        yield return new WaitUntil(() => FightManager.Instance != null);
        FightManager.Instance.OnFightOver += OpenFightOver;                
    }

    void OpenFightOver(bool b)
    {
        StartCoroutine(OpenFightOverPlay(b));
    }

    IEnumerator OpenFightOverPlay(bool b)
    {
        yield return new WaitForSeconds(2);

        AudioManager.Instance.PlayAudioBGM(null);

        background.SetActive(true);

        if (b == true)
        {
            //玩家贏
            AudioManager.Instance.PlayAudioForName("FightWin");
            winText.gameObject.SetActive(true);
        }
        else
        {
            //玩家輸
            AudioManager.Instance.PlayAudioForName("FightLost");
            lostText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        yesButton.gameObject.SetActive(true);
    }

    public void OnCloseFightOver()
    {
        background.SetActive(false);
        winText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
    }
}

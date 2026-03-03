using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject blocker;
    [SerializeField] Button OpenTeamButton;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnPlayerControlChange += UpdateUI_Blocker;
        FightManager.Instance.OnCanChangeCardChange += UpdateUI_OpenTeamButton;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayerControlChange -= UpdateUI_Blocker;
            FightManager.Instance.OnCanChangeCardChange -= UpdateUI_OpenTeamButton;
        }
    }

    void UpdateUI_Blocker(bool playerControl)
    {
        //玩家控制改變時觸發
        blocker.SetActive(!playerControl);
    }

    void UpdateUI_OpenTeamButton(bool canChangeCard)
    {
        //玩家更換卡片改變時觸發
        OpenTeamButton.interactable = canChangeCard;
    }
}

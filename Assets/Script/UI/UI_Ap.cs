using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Ap : MonoBehaviour
{
    [SerializeField] GameObject ap1Light;
    [SerializeField] GameObject ap2Light;
    [SerializeField] GameObject ap3Light;
    [SerializeField] GameObject ap4Light;
    [SerializeField] GameObject ap5Light;
    [SerializeField] GameObject ap6Light;

    CharacterModel currentPlayer;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnPlayerChange += ResetPlayer;       
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayerChange -= ResetPlayer;
        }
    }

    void ResetPlayer(CharacterModel player)
    {
        if (currentPlayer != null)
        {
            currentPlayer.OnNowApChange -= UpdateUI_Ap;
        }

        currentPlayer = player;

        currentPlayer.OnNowApChange += UpdateUI_Ap;
        UpdateUI_Ap(currentPlayer.nowAp);
    }

    void UpdateUI_Ap(int nowAp)
    {
        //我方的當前AP量改變時觸發
        
        //根據當前AP量開關相應的AP量UI
        ap1Light.SetActive(nowAp > 0);
        ap2Light.SetActive(nowAp > 1);
        ap3Light.SetActive(nowAp > 2);
        ap4Light.SetActive(nowAp > 3);
        ap5Light.SetActive(nowAp > 4);
        ap6Light.SetActive(nowAp > 5);
    }
}

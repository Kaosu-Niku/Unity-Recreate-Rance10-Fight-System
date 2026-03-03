using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Turn : MonoBehaviour
{
    [SerializeField] Text turnText;
    [SerializeField] Text playerTurnText;
    [SerializeField] Text enemyTurnText;
    [SerializeField] Animator animator;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());  
    }

    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnNowTurnChange += UpdateUI_Turn;
        FightManager.Instance.OnPlayerTurnStart += UpdateUI_PlayerTurn;
        FightManager.Instance.OnEnemyTurnStart += UpdateUI_EnemyTurn;               
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnNowTurnChange -= UpdateUI_Turn;
            FightManager.Instance.OnPlayerTurnStart -= UpdateUI_PlayerTurn;
            FightManager.Instance.OnEnemyTurnStart -= UpdateUI_EnemyTurn;
        }
    }

    void UpdateUI_Turn(int nowTurn, int overTurn, bool haveOver)
    {
        //當前回合數改變時觸發
        if (haveOver)
        {
            turnText.text = $"{nowTurn}/{overTurn}";
        }
        else
        {
            turnText.text = $"{nowTurn}";
        }
        
    }

    void UpdateUI_PlayerTurn()
    {
        //輪到我方回合時觸發
        animator.Play("Turn_Start_Player", 0, 0);
    }

    void UpdateUI_EnemyTurn()
    {
        //輪到敵人回合時觸發
        animator.Play("Turn_Start_Enemy", 0, 0);
    }
}

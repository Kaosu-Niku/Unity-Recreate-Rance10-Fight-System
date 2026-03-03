using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Name : MonoBehaviour
{
    [SerializeField] Text enemyNameText;
    [SerializeField] Text playerNameText;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnEnemyChange += UpdateUI_EnemyName;
        FightManager.Instance.OnPlayerChange += UpdateUI_PlayerName;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnEnemyChange -= UpdateUI_EnemyName;
            FightManager.Instance.OnPlayerChange -= UpdateUI_PlayerName;
        }
    }

    void UpdateUI_EnemyName(CharacterModel enemy)
    {
        //敵人改變時觸發

        //顯示對應敵人的名稱文字
        enemyNameText.text = enemy.displayName;
    }

    void UpdateUI_PlayerName(CharacterModel player)
    {
        //我方改變時觸發

        //顯示對應我方的名稱文字
        playerNameText.text = player.displayName;
    }
}

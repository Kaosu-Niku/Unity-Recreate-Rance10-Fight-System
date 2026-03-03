using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MISS : MonoBehaviour
{
    [SerializeField] Text enemyMissText;
    [SerializeField] Text playerMissText;
    [SerializeField] Animator animator;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => BattleManager.Instance != null);

        BattleManager.Instance.OnAttackMiss += UpdateUI_Miss;
    }

    void OnDisable()
    {
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.OnAttackMiss -= UpdateUI_Miss;
        }
    }

    void UpdateUI_Miss(CharacterModel character)
    {
        //攻擊被無效時觸發，顯示MISS
        if (character == FightManager.Instance.enemy)
        {
            enemyMissText.text = "MISS";
            animator.Play("Enemy_Miss", 0, 0);
            return;
        }

        if(character == FightManager.Instance.player)
        {
            playerMissText.text = "MISS";
            animator.Play("Player_Miss", 0, 0);
            return;
        }
    }
}

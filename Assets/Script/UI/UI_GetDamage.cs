using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetDamage : MonoBehaviour
{
    [SerializeField] Text enemyGetDamageText;
    [SerializeField] Text playerGetDamageText;
    [SerializeField] Animator animator;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => BattleManager.Instance != null);

        BattleManager.Instance.OnDefenseGetDamage += UpdateUI_GetDamage;
    }

    void OnDisable()
    {
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.OnDefenseGetDamage -= UpdateUI_GetDamage;
        }
    }

    void UpdateUI_GetDamage(CharacterModel character, float damage)
    {
        //對防守方造成傷害時觸發
        
        //顯示對應的傷害量 (四捨五入取整數)
        if(character == FightManager.Instance.enemy)
        {
            enemyGetDamageText.text = Mathf.RoundToInt(damage).ToString();
            animator.Play("Enemy_Get_Damage", 0, 0);
            return;
        }

        if(character == FightManager.Instance.player)
        {
            playerGetDamageText.text = Mathf.RoundToInt(damage).ToString();
            animator.Play("Player_Get_Damage", 0, 0);
            return;
        }
    }
}

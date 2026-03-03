using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyImage : MonoBehaviour
{
    [SerializeField] Image enemyImage;
    [SerializeField] Animator animator;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnEnemyChange += UpdateUI_EnemyImage;
        FightManager.Instance.OnFightOver += UpdateUI_EnemyImageLost;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnEnemyChange -= UpdateUI_EnemyImage;
            FightManager.Instance.OnFightOver -= UpdateUI_EnemyImageLost;
        }
    }

    void UpdateUI_EnemyImage(CharacterModel enemy)
    {
        //敵人改變時觸發
        enemyImage.sprite = enemy.image;
        animator.Rebind();
        animator.Play("EnemyImage_Repeat", 0, 0);
    }

    void UpdateUI_EnemyImageLost(bool b)
    {
        //戰鬥結束並且是玩家贏時觸發
        if(b == true)
        {
            animator.Play("EnemyImage_Lost", 0, 0);
        }       
    }
}

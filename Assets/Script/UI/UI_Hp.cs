using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hp : MonoBehaviour
{
    [SerializeField] Text enemyHpText;
    [SerializeField] GameObject enemyHpSlider;
    [SerializeField] GameObject enemyHpSliderLag;
    float enemyOldHpPercent = 1;
    CharacterModel currentEnemy;
    Coroutine enemyHpSliderLagCoroutine;

    [SerializeField] Text playerHpText;
    [SerializeField] GameObject playerHpSlider;
    [SerializeField] GameObject playerHpSliderLag;    
    float playerOldHpPercent = 1;
    CharacterModel currentPlayer;
    Coroutine playerHpSliderLagCoroutine;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnEnemyChange += ResetEnemy;
        FightManager.Instance.OnPlayerChange += ResetPlayer;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnEnemyChange -= ResetEnemy;
            FightManager.Instance.OnPlayerChange -= ResetPlayer;
        }
    }

    void ResetEnemy(CharacterModel enemy)
    {
        if (currentEnemy != null)
        {
            currentEnemy.OnNowHpChange -= UpdateUI_EnemyHp;
        }

        currentEnemy = enemy;

        currentEnemy.OnNowHpChange += UpdateUI_EnemyHp;
        UpdateUI_EnemyHp(currentEnemy.nowHp, currentEnemy.initHp);
    }

    void ResetPlayer(CharacterModel player)
    {
        if (currentPlayer != null)
        {
            currentPlayer.OnNowHpChange -= UpdateUI_PlayerHp;
        }

        currentPlayer = player;

        currentPlayer.OnNowHpChange += UpdateUI_PlayerHp;
        UpdateUI_PlayerHp(currentPlayer.nowHp, currentPlayer.initHp);
    }

    void UpdateUI_EnemyHp(float enemyHp, float enemyMaxHp)
    {
        //敵人當前血量改變時觸發
        float hpPercent = Mathf.Clamp(enemyHp / enemyMaxHp, 0f, 1f);
        enemyHpText.text = $"{(int)enemyHp}";

        //主血條立即更新UI血條比例
        enemyHpSlider.transform.localScale = new Vector3(hpPercent, 1, 1);

        //延遲血條透過給協程平滑動態更新血條比例
        if (enemyHpSliderLagCoroutine != null)
        {
            StopCoroutine(enemyHpSliderLagCoroutine);
        }            
        enemyHpSliderLagCoroutine = StartCoroutine(DelayHpChange(enemyHpSliderLag.transform, enemyOldHpPercent, hpPercent, 1));
        enemyOldHpPercent = hpPercent;
    }

    void UpdateUI_PlayerHp(float playerHp, float playerMaxHp)
    {
        //我方當前血量改變時觸發
        float hpPercent = Mathf.Clamp(playerHp / playerMaxHp, 0f, 1f);
        playerHpText.text = $"{(int)playerHp}";

        //主血條立即更新UI血條比例
        playerHpSlider.transform.localScale = new Vector3(hpPercent, 1, 1);

        //延遲血條透過給協程平滑動態更新血條比例
        if (playerHpSliderLagCoroutine != null)
        {
            StopCoroutine(playerHpSliderLagCoroutine);
        }

        playerHpSliderLagCoroutine = StartCoroutine(DelayHpChange(playerHpSliderLag.transform, playerOldHpPercent, hpPercent, 1));
        playerOldHpPercent = hpPercent;
    }

    IEnumerator DelayHpChange(Transform lagBarTransform, float fromPercent, float toPercent, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 線性插值縮放，t從0~1
            float currentPercent = Mathf.Lerp(fromPercent, toPercent, t);
            lagBarTransform.localScale = new Vector3(currentPercent, 1, 1);
            yield return null;
        }
        // 確保最後設定為目標值
        lagBarTransform.localScale = new Vector3(toPercent, 1, 1);
    }
}

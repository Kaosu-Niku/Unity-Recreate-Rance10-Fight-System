using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Combo : MonoBehaviour
{
    [SerializeField] Text comboNumberText;
    [SerializeField] Text comboText;

    void Start()
    {
        comboNumberText.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => BattleManager.Instance != null);

        BattleManager.Instance.OnComboChange += UpdateUI_Combo;
    }

    void OnDisable()
    {
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.OnComboChange -= UpdateUI_Combo;
        }
    }

    void UpdateUI_Combo(int count)
    {
        if(count > 0)
        {
            comboNumberText.gameObject.SetActive(true);
            comboNumberText.text = count.ToString();
            comboText.gameObject.SetActive(true);
        }
        else
        {
            comboNumberText.gameObject.SetActive(false);
            comboText.gameObject.SetActive(false);
        }
    }
}

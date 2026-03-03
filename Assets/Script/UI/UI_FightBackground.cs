using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FightBackground : MonoBehaviour
{
    [SerializeField] Image fightBackgroundImage;

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnFightBackgroundImageChange += UpdateUI_FightBackgroundImage;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnFightBackgroundImageChange -= UpdateUI_FightBackgroundImage;
        }
    }

    void UpdateUI_FightBackgroundImage(Sprite backgroundSprite)
    {
        //戰鬥背景圖片改變時觸發

        //根據戰鬥背景圖片顯示對應的圖片
        fightBackgroundImage.sprite = backgroundSprite;
    }
}

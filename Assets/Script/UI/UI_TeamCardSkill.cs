using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_TeamCardSkill : MonoBehaviour
{
    [SerializeField] GameObject cardBack; //卡片背景;

    [SerializeField] GameObject outsideSkillBack1; //外面的卡片技能背景1;
    [SerializeField] GameObject outsideSkillBack2; //外面的卡片技能背景2;
    [SerializeField] Button skillButton1; //卡片技能按鈕1 
    [SerializeField] Button skillButton2; //卡片技能按鈕2

    [SerializeField] Image outsideSkillIcon1; //外面的卡片技能圖片1 
    [SerializeField] Image outsideSkillIcon2; //外面的卡片技能圖片2 
    [SerializeField] Image skillIcon1; //卡片技能圖片1 
    [SerializeField] Image skillIcon2; //卡片技能圖片2 

    [SerializeField] Text outsideSkillName1; //外面的卡片技能名稱1 
    [SerializeField] Text outsideSkillName2; //外面的卡片技能名稱2 
    [SerializeField] Text skillName1; //卡片技能名稱1 
    [SerializeField] Text skillName2; //卡片技能名稱2 

    [SerializeField] Text outsideSkillAp1; //外面的卡片技能消耗AP數1 
    [SerializeField] Text outsideSkillAp2; //外面的卡片技能消耗AP數2
    [SerializeField] Text skillAp1; //卡片技能消耗AP數1 
    [SerializeField] Text skillAp2; //卡片技能消耗AP數2 

    [SerializeField] Text skillDepiction1; //卡片技能敘述1 
    [SerializeField] Text skillDepiction2; //卡片技能敘述2 

    CardModel myCard; //屬於此UI的卡片資料

    void Start()
    {
        cardBack.gameObject.SetActive(false);
    }

    public void UpdateUI_TeamCards(CardModel card)
    {
        //外部手動調用觸發

        //根據參數指定的卡片顯示卡片技能圖片、卡片技能名稱、卡片技能消耗AP數、卡片技能敘述
        if (card != null)
        {
            myCard = card;

            if (myCard.attack1 != null)
            {
                outsideSkillBack1.SetActive(true);
                skillButton1.gameObject.SetActive(true);
                AttackModel attack1 = myCard.attack1;
                outsideSkillIcon1.sprite = attack1.image;
                outsideSkillName1.text = attack1.displayName;
                outsideSkillAp1.text = $"{attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}";
                skillIcon1.sprite = attack1.image;
                skillName1.text = attack1.displayName;
                skillAp1.text = $"{attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}AP";
                skillDepiction1.text = attack1.depiction;
            }
            else
            {
                outsideSkillBack1.SetActive(false);
                skillButton1.gameObject.SetActive(false);
            }

            if (myCard.attack2 != null)
            {
                outsideSkillBack2.SetActive(true);
                skillButton2.gameObject.SetActive(true);
                AttackModel attack2 = myCard.attack2;
                outsideSkillIcon2.sprite = attack2.image;
                outsideSkillName2.text = attack2.displayName;
                outsideSkillAp2.text = $"{attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}";
                skillIcon2.sprite = attack2.image;
                skillName2.text = attack2.displayName;
                skillAp2.text = $"{attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}AP";
                skillDepiction2.text = attack2.depiction;
            }
            else
            {
                outsideSkillBack2.SetActive(false);
                skillButton2.gameObject.SetActive(false);
            }
        }
    }
}

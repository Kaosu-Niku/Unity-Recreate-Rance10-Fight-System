using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CardSkill : MonoBehaviour
{
    [SerializeField] int CardIndex; //此卡片技能UI屬於第幾順位

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

    CharacterModel currentPlayer;
    CardModel myCard; //屬於此UI的卡片資料
    bool canUse = true; //卡片是否能使用

    void Start()
    {
        cardBack.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null && FightManager.Instance != null && BattleManager.Instance != null);

        PlayerManager.Instance.OnFightCardsChange += UpdateUI_FightCards;
        UpdateUI_FightCards(PlayerManager.Instance.GetFightCards());

        FightManager.Instance.OnPlayerTurnStart += PlayerTurnStartResetUse;

        BattleManager.Instance.OnAttackOver += UpdateUI_CardsClickCheck;
    }

    void OnDisable()
    {
        if(PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnFightCardsChange -= UpdateUI_FightCards;            
        }

        if (FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayerTurnStart -= PlayerTurnStartResetUse;         
        }

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnAttackOver -= UpdateUI_CardsClickCheck;
        }
    }

    void UpdateUI_FightCards(IReadOnlyList<CardModel> cards)
    {
        //玩家出戰的卡片改變時觸發

        //根據玩家出戰的卡片顯示卡片技能圖片、卡片技能名稱、卡片技能消耗AP數、卡片技能敘述

        if (cards.Count - 1 >= CardIndex)
        {
            if (cards[CardIndex] != null)
            {

                //在更新卡片資訊前，需要先比對當回合記錄的戰鬥卡片組資料和更新後傳遞過來的戰鬥卡片組資料
                //這是為了確定哪些是原先就在場上的卡片，哪些是更新後出戰的卡片
                //因為更新後出戰的卡片在當回合是不能使用技能的，需等待到下一回合
                bool haveOld = false;
                foreach (CardModel oldCard in PlayerManager.Instance.GetCurrentCards())
                {
                    if(cards[CardIndex] == oldCard)
                    {
                        haveOld = true; 
                        break;
                    }
                }

                if(haveOld == false)
                {
                    canUse = false;
                }

                myCard = cards[CardIndex];          

                if (myCard.attack1 != null)
                {
                    outsideSkillBack1.SetActive(true);
                    skillButton1.gameObject.SetActive(true);

                    //更新後才出戰的卡片，技能不可主動使用
                    if (canUse == false)
                    {
                        skillButton1.interactable = false;
                    }

                    //自動施放類型的技能不可主動使用
                    if (myCard.attack1.autoAttack == true)
                    {
                        skillButton1.interactable = false;
                    }

                    //攻擊條件不符合的技能不可主動使用
                    if (myCard.attack1.AttackConditionCheck() == false)
                    {
                        skillButton1.interactable = false;
                    }

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

                    //更新後才出戰的卡片，技能不可主動使用
                    if (canUse == false)
                    {
                        skillButton2.interactable = false;
                    }

                    //自動施放類型的技能不可主動使用
                    if (myCard.attack2.autoAttack == true)
                    {
                        skillButton2.interactable = false;
                    }

                    //攻擊條件不符合的技能不可主動使用
                    if (myCard.attack2.AttackConditionCheck() == false)
                    {
                        skillButton2.interactable = false;
                    }

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

                return;
            }
        }

        outsideSkillBack1.SetActive(false);
        skillButton1.gameObject.SetActive(false);
        outsideSkillBack2.SetActive(false);
        skillButton2.gameObject.SetActive(false);
    }

    void UpdateUI_CardsClickCheck()
    {
        //一次攻擊流程結束時觸發

        if (canUse == false)
            return;

        if (myCard == null)
            return;
        
        if (myCard.attack1 != null)
        {
            //比對卡片技能所需消耗AP量和玩家當前AP量，如當前AP量不足則技能不可主動使用
            if (myCard.attack1.useAP > FightManager.Instance.player.nowAp)
            {
                skillButton1.interactable = false;
            }
            else
            {
                skillButton1.interactable = true;
            }

            //自動施放類型的技能不可主動使用
            if (myCard.attack1.autoAttack == true)
            {
                skillButton1.interactable = false;
            }

            //攻擊條件不符合的技能不可主動使用
            if (myCard.attack1.AttackConditionCheck() == false)
            {
                skillButton1.interactable = false;
            }
        }

        if (myCard.attack2 != null)
        {
            //比對卡片技能所需消耗AP量和當前AP量，如當前AP量不足則技能不可主動使用
            if (myCard.attack2.useAP > FightManager.Instance.player.nowAp)
            {
                skillButton2.interactable = false;
            }
            else
            {
                skillButton2.interactable = true;
            }

            //自動施放類型的技能不可主動使用
            if (myCard.attack2.autoAttack == true)
            {
                skillButton2.interactable = false;
            }

            //攻擊條件不符合的技能不可主動使用
            if (myCard.attack2.AttackConditionCheck() == false)
            {
                skillButton2.interactable = false;
            }
        }      
    }

    void PlayerTurnStartResetUse()
    {
        //玩家回合開始時觸發

        //重置卡片使用權限
        canUse = true;

        if (myCard == null)
            return;

        if (myCard.attack1 != null)
        {
            //復原疊加的AP量
            myCard.attack1.RecoveUseAP();

            outsideSkillAp1.text = $"{myCard.attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}";
            skillAp1.text = $"{myCard.attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}AP";

            skillButton1.interactable = true;

            //自動施放類型的技能不可主動使用
            if (myCard.attack1.autoAttack == true)
            {
                skillButton1.interactable = false;
            }

            //攻擊條件不符合的技能不可主動使用
            if (myCard.attack1.AttackConditionCheck() == false)
            {
                skillButton1.interactable = false;
            }
        }

        if (myCard.attack2 != null)
        {
            //復原疊加的AP量
            myCard.attack2.RecoveUseAP();

            outsideSkillAp2.text = $"{myCard.attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}";
            skillAp2.text = $"{myCard.attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}AP";

            skillButton2.interactable = true;

            //自動施放類型的技能不可主動使用
            if (myCard.attack2.autoAttack == true)
            {
                skillButton2.interactable = false;
            }

            //攻擊條件不符合的技能不可主動使用
            if (myCard.attack2.AttackConditionCheck() == false)
            {
                skillButton2.interactable = false;
            }
        }
    }

    //使用卡片技能1
    public void OnUseAttack1()
    {
        if (myCard == null)
            return;

        if (myCard.attack1 != null)
        {
            if(myCard.attack1.attackMyself == false)
            {
                FightManager.Instance.PlayerAttackEnemy(myCard, myCard.attack1);
            }
            else
            {
                FightManager.Instance.PlayerAttackMyself(myCard, myCard.attack1);
            }

            //疊加AP
            myCard.attack1.AddUseAP();

            //使玩家當回合無法再次更換卡片
            FightManager.Instance.SetCanChangeCard(false);        

            //當回合不可再使用此卡片的所有技能，必須直到下次輪到玩家回合後重置技能使用權限
            canUse = false;

            outsideSkillAp1.text = $"{myCard.attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}";
            skillAp1.text = $"{myCard.attack1.useAP}{(myCard.attack1.stackAP ? "+" : "")}AP";

            skillButton1.interactable = false;

            if (myCard.attack2 != null)
            {
                skillButton2.interactable = false;
            } 
        }  
    }

    //使用卡片技能2
    public void OnUseAttack2()
    {
        if (myCard == null)
            return;

        if (myCard.attack2 != null)
        {
            if (myCard.attack2.attackMyself == false)
            {
                FightManager.Instance.PlayerAttackEnemy(myCard, myCard.attack2);
            }
            else
            {
                FightManager.Instance.PlayerAttackMyself(myCard, myCard.attack2);
            }

            //疊加AP
            myCard.attack2.AddUseAP();

            //使玩家當回合無法再次更換卡片
            FightManager.Instance.SetCanChangeCard(false);         

            //當回合不可再使用此卡片的所有技能，必須直到下次輪到玩家回合後重置技能使用權限
            canUse = false;

            outsideSkillAp2.text = $"{myCard.attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}";
            skillAp2.text = $"{myCard.attack2.useAP}{(myCard.attack2.stackAP ? "+" : "")}AP";

            skillButton2.interactable = false;

            if (myCard.attack1 != null)
            {
                skillButton1.interactable = false;
            }
        }
    }
}

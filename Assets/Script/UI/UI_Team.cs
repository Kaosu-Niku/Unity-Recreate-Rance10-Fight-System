using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Team : MonoBehaviour
{
    [SerializeField] GameObject teamCardTemplate; //卡片UI模板

    [SerializeField] ScrollRect scrollRect; //ScrollRect的元件
    [SerializeField] RectTransform contentParent; // ScrollRect 的 Content

    //上次調用的陣營index
    int lastCampIndex = 0;

    //各陣營
    List<CardModel> ranceTeamCards = new List<CardModel>();
    List<CardModel> rezasTeamCards = new List<CardModel>();
    List<CardModel> hermannTeamCards = new List<CardModel>();
    List<CardModel> zethTeamCards = new List<CardModel>();
    List<CardModel> freeCityTeamCards = new List<CardModel>();
    List<CardModel> japanTeamCards = new List<CardModel>();
    List<CardModel> otherTeamCards = new List<CardModel>();
    List<CardModel> demihumanTeamCards = new List<CardModel>();
    List<CardModel> monsterTeamCards = new List<CardModel>();
    List<CardModel> godAndDemonTeamCards = new List<CardModel>();

    // 物件池
    List<GameObject> cardPool = new List<GameObject>();
    int poolSize = 50;

    void Awake()
    {
        //物件池預先生成指定數量的卡片
        for (int i = 0; i < poolSize; i++)
        {
            GameObject c = Instantiate(teamCardTemplate, contentParent, false);
            c.SetActive(false);
            cardPool.Add(c);
        }
    }

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);

        PlayerManager.Instance.OnHoldCardsChange += UpdateUI_HoldCards;
        UpdateUI_HoldCards(PlayerManager.Instance.GetHoldCards());
        UpdateUI_TeamCards(lastCampIndex);
    }

    void OnDisable()
    {
        if(PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnHoldCardsChange -= UpdateUI_HoldCards;
        }
    }

    void UpdateUI_HoldCards(IReadOnlyList<CardModel> cards)
    {
        //玩家持有的卡片改變時觸發

        //根據陣營將持有的卡片分類至各個陣營陣列中保存
        ranceTeamCards.Clear();
        rezasTeamCards.Clear();
        hermannTeamCards.Clear();
        zethTeamCards.Clear();
        freeCityTeamCards.Clear();
        japanTeamCards.Clear();
        otherTeamCards.Clear();
        demihumanTeamCards.Clear();
        monsterTeamCards.Clear();
        godAndDemonTeamCards.Clear();

        foreach (CardModel card in cards)
        {
            switch (card.camp)
            {
                case Camp.Rance:
                    ranceTeamCards.Add(card);
                    break;
                case Camp.Rezas:
                    rezasTeamCards.Add(card);
                    break;
                case Camp.Hermann:
                    hermannTeamCards.Add(card);
                    break;
                case Camp.Zeth:
                    zethTeamCards.Add(card);
                    break;
                case Camp.FreeCity:
                    freeCityTeamCards.Add(card);
                    break;
                case Camp.Japan:
                    japanTeamCards.Add(card);
                    break;
                case Camp.Other:
                    otherTeamCards.Add(card);
                    break;
                case Camp.Demihuman:
                    demihumanTeamCards.Add(card);
                    break;
                case Camp.Monster:
                    monsterTeamCards.Add(card);
                    break;
                case Camp.GodAndDemon:
                    godAndDemonTeamCards.Add(card);
                    break;
            }
        }    
    }

    public void UpdateUI_TeamCards(int campIndex)
    {
        //可外部手動調用觸發

        lastCampIndex = campIndex;

        //根據參數所選的陣營，於介面中顯示出持有的所有卡片中，屬於該陣營的所有卡片
        List<CardModel> whichTeam = new List<CardModel>();
        
        switch (campIndex)
        {
            case 0:
                whichTeam = ranceTeamCards;
                break;
            case 1:
                whichTeam = rezasTeamCards;
                break;
            case 2:
                whichTeam = hermannTeamCards;
                break;
            case 3:
                whichTeam = zethTeamCards;
                break;
            case 4:
                whichTeam = freeCityTeamCards;
                break;
            case 5:
                whichTeam = japanTeamCards;
                break;
            case 6:
                whichTeam = otherTeamCards;
                break;
            case 7:
                whichTeam = demihumanTeamCards;
                break;
            case 8:
                whichTeam = monsterTeamCards;
                break;
            case 9:
                whichTeam = godAndDemonTeamCards;
                break;
        }

        foreach (GameObject c in cardPool)
        {
            c.SetActive(false);
        }

        //確保每次切換陣營時，ScrollRect 的版面會重新滾動到最上方
        scrollRect.verticalNormalizedPosition = 1f;

        //開始放入卡片
        for (int i = 0; i < whichTeam.Count; i++)
        {
            GameObject c;

            //從物件池獲取一個卡片，如果物件池內的卡片數量不足，則立刻再新增一個卡片加入物件池
            if (i < cardPool.Count)
            {
                c = cardPool[i];
            }
            else
            {
                c = Instantiate(teamCardTemplate, contentParent, false);
                cardPool.Add(c);
            }
            c.SetActive(true);

            //獲取卡片的指定腳本以觸發卡片內容設置
            CardModel card = whichTeam[i];
            UI_TeamCard tc = c.GetComponent<UI_TeamCard>();
            if (tc != null)
            {
                tc.UpdateUI_TeamCards(card);
            }
            UI_TeamCardSkill tcs = c.GetComponentInChildren<UI_TeamCardSkill>();
            if (tcs != null)
            {
                tcs.UpdateUI_TeamCards(card);
            }

            //主動調用Layout計算使版面刷新
            Canvas.ForceUpdateCanvases();
        }
    }
}

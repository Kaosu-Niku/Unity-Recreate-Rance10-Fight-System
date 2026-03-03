using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //單例模式
    public static PlayerManager Instance { get; private set; }

    [SerializeField] CharacterSO playerCharacterSO; //玩家的角色資源
    public CharacterModel player { get; private set; }

    private bool _playerReadyCheck = false; //玩家資源準備完成確認
    public bool playerReadyCheck { 
        get => _playerReadyCheck; 
        private set
        {
            if (_playerReadyCheck != value)
            {
                _playerReadyCheck = value;
            }
        }
    }

    private List<CardModel> holdCards = new(); //玩家持有的卡片
    public IReadOnlyList<CardModel> GetHoldCards()
    {
        return holdCards;
    }
    public event Action<IReadOnlyList<CardModel>> OnHoldCardsChange; //玩家持有的卡片改變事件
    //(由於集合類型的內部元素變化並不會觸發屬性set，因此集合類型不適合用屬性set觸發event，只能手動觸發event)

    private List<CardModel> fightCards = new(); //玩家出戰的卡片
    public IReadOnlyList<CardModel> GetFightCards()
    {
        return fightCards;
    }
    public event Action<IReadOnlyList<CardModel>> OnFightCardsChange; //玩家出戰的卡片改變事件
    //(由於集合類型的內部元素變化並不會觸發屬性set，因此集合類型不適合用屬性set觸發event，只能手動觸發event)  

    private List<CardModel> initCards = new(); //紀錄玩家首次出戰卡片的暫存資料 (每場戰鬥開始紀錄一次)
    private List<CardModel> currentCards = new(); //紀錄玩家當回合出戰卡片的暫存資料 (隨著戰鬥過程不斷紀錄)
    public IReadOnlyList<CardModel> GetCurrentCards()
    {
        return currentCards;
    }

    void Awake()
    {
        //單例模式建構
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        player = new CharacterModel(playerCharacterSO);
        playerReadyCheck = true;

        //預設讓玩家直接擁有所有的卡片，之後做存檔功能時才更改邏輯
        for (int i = 0; i < CardPoolManager.Instance.GetPoolCount(); i++)
        {
            CardModel card = CardPoolManager.Instance.GetCard(i);

            if (card != null) {
                holdCards.Add(card);
            }
        }
        OnHoldCardsChange?.Invoke(GetHoldCards());

        //確認存檔內容來添加出戰卡片
        if (SaveManager.Instance != null && SaveManager.Instance.saveData != null)
        {
            for (int i = 0; i < SaveManager.Instance.saveData.FightCardPoolIndex.Count; i++)
            {
                CardModel card = null;

                int poolIndex = SaveManager.Instance.saveData.FightCardPoolIndex[i];
                if (holdCards.Count + 1 > poolIndex) {
                    card = holdCards[poolIndex];
                }

                if (card != null)
                {
                    PutFightCard(card);
                }
            }
        }     
    }

    //重設玩家角色的初始血量
    public void ResetInitHp()
    {
        float totalHp = 0;
        //遍歷所有出戰卡片
        for (int i = 0; i < fightCards.Count; i++)
        {
            //添加出戰卡片的血量 (以4倍的量計算)
            totalHp += fightCards[i].hp * 4;

            //遍歷所有持有卡片確認所屬陣營是否與出戰卡片相同，如相同則添加持有卡片的血量 (以1倍的量計算)
            for (int i2 = 0; i2 < holdCards.Count; i2++)
            {
                if(holdCards[i2].camp == fightCards[i].camp)
                {
                    totalHp += holdCards[i2].hp;
                }
            }
        }
        player.SetInitHp(totalHp);
    }

    //重設玩家角色的AP量
    public void ResetAp()
    {
        player.AddAP(-99);
    }

    //重設玩家角色的效果
    public void ResetBuff()
    {
        for(int i = 0; i < player.buffManager.GetBuffs().Count; i++)
        {
            player.DeleteRandomBuff();
        }        
    }

    //重設玩家角色的狀態
    public void ResetState()
    {
        for (int i = 0; i < player.stateManager.GetStates().Count; i++)
        {
            player.DeleteRandomState();
        }
    }

    //根據指定的卡片尋找與其相同的持有卡片，如果找不到則回傳null (ex: 變身卡)
    public CardModel FindHoldCard(CardModel fightCard)
    {
        if (fightCard == null)
            return null;

        return holdCards.Find(c => c.GetCardSO == fightCard.GetCardSO);
    }

    //將指定卡片放入出戰卡片
    public void PutFightCard(CardModel putCard)
    {
        if (putCard == null)
            return;

        if (putCard.canFight == false)
            return;

        //同一陣營的卡片只能出戰一張

        //如場上已存在同陣營卡片則直接覆蓋成新的出戰卡片
        for (int i = 0; i < fightCards.Count; i++)
        {
            if (fightCards[i].camp == putCard.camp)
            {
                fightCards[i] = putCard;

                OnFightCardsChange?.Invoke(GetFightCards());
                OnHoldCardsChange?.Invoke(GetHoldCards());
                return;
            }
        }

        //如果沒有存在同陣營卡片則可以直接出戰 (出戰卡片最多只能7張)
        if (fightCards.Count >= 7)
            return;

        fightCards.Add(putCard);

        OnFightCardsChange?.Invoke(GetFightCards());
        OnHoldCardsChange?.Invoke(GetHoldCards());
    }

    //將指定卡片移出出戰卡片
    public void PullFightCard(CardModel pullCard)
    {
        if (pullCard == null)
            return;

        if (FindHoldCard(pullCard) == null)
            return;

        fightCards.Remove(pullCard);

        OnFightCardsChange?.Invoke(GetFightCards());
        OnHoldCardsChange?.Invoke(GetHoldCards());
    }

    //根據提供的陣營回傳一個該陣營目前可出戰的一個持有卡片 (若一個都沒有則回傳null)
    public CardModel CampReturnCard(Camp camp)
    {
        List<CardModel> campCards = new List<CardModel>();
        foreach(var card in holdCards)
        {
            if(card.camp == camp && card.canFight == true)
            {
                campCards.Add(card);
            }
        }

        if (campCards.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, campCards.Count);
            return campCards[r];
        }
        else
        {
            return null;
        }
    }

    //DOWN效果
    public void Down(CardSO downCard)
    {
        if (GetFightCards().Count <= 0)
            return;

        CardModel outCard = null;
        int outCardIndex = 0;

        //先隨機挑選一個出戰卡片
        int r = UnityEngine.Random.Range(0, GetFightCards().Count);
        outCardIndex = r;
        outCard = fightCards[r];

        //若有指定要DOWN的卡片且該卡片正好出戰中，就改為挑選該張出戰卡片
        if (downCard != null)
        {
            
            for (int i = 0; i < fightCards.Count; i++)
            {
                if (fightCards[i].GetCardSO == downCard)
                {
                    outCardIndex = i;
                    outCard = fightCards[i];
                    break;
                }
            }
        }

        if(outCard != null)
        {
            //令此卡不可再出戰
            if (FindHoldCard(outCard) != null)
            {
                FindHoldCard(outCard).SetCanFight(false);
            }

            //重新找一張與此卡同陣營的另一張卡代替上場
            Camp camp = outCard.camp;
            CardModel newCard = CampReturnCard(camp);
            if (newCard != null)
            {
                fightCards[outCardIndex] = newCard;
            }
            else
            {
                PullFightCard(fightCards[outCardIndex]);
            }

            OnFightCardsChange?.Invoke(GetFightCards());
            OnHoldCardsChange?.Invoke(GetHoldCards());
        }
    }

    //將所有目前的出戰卡片紀錄起來 (每場戰鬥開始紀錄一次)
    public void SetInitCards()
    {
        initCards = new();
        foreach (var card in fightCards)
        {
            initCards.Add(card);
        }
    }

    //將所有目前的出戰卡片紀錄起來 (隨著戰鬥過程不斷紀錄)
    public void SetCurrentCards()
    {
        currentCards = new();
        foreach (var card in fightCards)
        {
            currentCards.Add(card);
        }
    }

    //將與指定的卡片相同的持有卡片設置為不可出戰狀態
    public void HoldCardSetCantFight(CardModel card)
    {
        if (card == null)
            return;

        if (FindHoldCard(card) == null)
            return;

        FindHoldCard(card).SetCanFight(false);
    }

    //所有的持有卡片設置為可出戰狀態
    public void AllHoldCardSetCanFight()
    {
        foreach (var holdCard in holdCards)
        {
            holdCard.SetCanFight(true);
        }
        OnFightCardsChange?.Invoke(GetFightCards());
        OnHoldCardsChange?.Invoke(GetHoldCards());
    }

    //確認所有被紀錄過的出戰卡片，並比對相應的持有卡片，將其設置為不可出戰狀態
    public void CheckCurrentCardSetHoldCardCantFight()
    {
        foreach (var currentCard in currentCards)
        {
            HoldCardSetCantFight(currentCard); ;
        }
    }

    //遍歷所有持有卡片的技能，重置消耗AP量
    public void AllAttackResetUseAP()
    {
        foreach (CardModel holdCard in GetHoldCards())
        {
            if (holdCard.attack1 != null)
            {
                holdCard.attack1.ResetUseAP();
            }

            if (holdCard.attack2 != null)
            {
                holdCard.attack2.ResetUseAP();
            }
        }
    }

    //將所有當前的出戰卡片重新再放置一次 (此操作是為了正常移除變身卡，否則一場遊戲結束後未被移除的變身卡依舊會保持出戰，這是錯誤的狀態)
    public void ResetFightCard()
    {
        fightCards.Clear();
        List<int> FightCardPoolIndexList = new();

        foreach (CardModel initCard in initCards)
        {
            if (FindHoldCard(initCard) != null)
            {
                int index = CardPoolManager.Instance.GetIndex(FindHoldCard(initCard).GetCardSO);
                FightCardPoolIndexList.Add(index);

                PutFightCard(FindHoldCard(initCard));
            }
        }

        //存檔內容添加所有出戰卡片對應的卡片池Index
;        SaveManager.Instance.ResetFightCardPoolIndex(FightCardPoolIndexList);

    }

    //遍歷所有出戰卡片的技能，如是會自動施放的技能則施放(概率施放)
    public void AllAttackUseAuto()
    {
        foreach(CardModel card in GetFightCards())
        {
            if (card.attack1 != null)
            {
                if (card.attack1.autoAttack == true)
                {
                    float r = UnityEngine.Random.Range(0, 100);
                    if (r < card.attack1.autoProbability)
                    {
                        if (card.attack1.attackMyself == false)
                        {
                            FightManager.Instance.PlayerAttackEnemy(card, card.attack1);
                        }
                        else
                        {
                            FightManager.Instance.PlayerAttackMyself(card, card.attack1);
                        }                        
                    }                    
                }
            }

            if (card.attack2 != null)
            {
                if (card.attack2.autoAttack == true)
                {
                    float r = UnityEngine.Random.Range(0, 100);
                    if (r < card.attack2.autoProbability)
                    {
                        if (card.attack2.attackMyself == false)
                        {
                            FightManager.Instance.PlayerAttackEnemy(card, card.attack2);
                        }
                        else
                        {
                            FightManager.Instance.PlayerAttackMyself(card, card.attack2);
                        }
                    }
                }
            }
        }
    }
}

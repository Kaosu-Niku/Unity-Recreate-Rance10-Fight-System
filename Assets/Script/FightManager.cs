using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    //單例模式
    public static FightManager Instance { get; private set; }

    MapCellFightSO mapCellFightSO; //戰鬥地圖格資源

    public bool haveOver { get; private set; } //是否有回合數限制
    public int overTurn { get; private set; } //限制回合數

    private int _nowTurn; //當前回合數
    public int nowTurn
    {
        get => _nowTurn;
        private set
        {
            if (_nowTurn != value)
            {
                _nowTurn = value;
                OnNowTurnChange?.Invoke(_nowTurn, overTurn, haveOver);
            }
        }
    }
    public event Action<int, int, bool> OnNowTurnChange; //當前回合數改變事件
                                                         
    public bool turnOver { get; private set; } //回合是否結束

    CharacterModel _enemy; //敵人
    public CharacterModel enemy
    {
        get => _enemy;
        private set
        {
            if (_enemy != value)
            {
                _enemy = value;
                OnEnemyChange?.Invoke(_enemy);
            }
        }
    }
    public event Action<CharacterModel> OnEnemyChange; //敵人改變事件
    public void OnEnemyChangeInvoke()
    {
        OnEnemyChange?.Invoke(_enemy);
    }

    private EnemyActionModel enemyAction; //敵人行動
    public List<AttackModel> enemyNowAction { get; private set; } //敵人當前行動
    public event Action<IReadOnlyList<AttackModel>> OnEnemyNowActionChange; //敵人當前行動改變事件 

    //隨機移除敵人當前行動序列中的一個攻擊
    public void RandomDeleteEnemyAction()
    {
        if (enemyNowAction == null || enemyNowAction.Count == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, enemyNowAction.Count);
        enemyNowAction.RemoveAt(randomIndex);
        OnEnemyNowActionChange?.Invoke(enemyNowAction);
    }

    //重設敵人行動數據
    public void ChangeEnemyAction(EnemyActionSO enemyActionSO)
    {
        enemyAction = new EnemyActionModel(enemyActionSO);

        enemyNowAction = new List<AttackModel>(enemyAction.TurnReturnAction(nowTurn));
        OnEnemyNowActionChange?.Invoke(enemyNowAction);
        foreach (var action in enemyNowAction)
        {
            action.UpdateAttack();
        }
    }
    
    public CharacterModel _player { get; private set; } //我方
    public CharacterModel player
    {
        get => _player;
        private set
        {
            if (_player != value)
            {
                _player = value;
                OnPlayerChange?.Invoke(_player);
            }
        }
    }
    public event Action<CharacterModel> OnPlayerChange; //我方改變事件

    [SerializeField] AttackSO playerDefenseSO; //供我方使用的防禦技能
    AttackModel playerDefense;

    public event Action<bool> OnPlayerControlChange; //我方控制改變事件
    private bool _playerControl; //我方是否可以控制
    public bool playerControl
    {
        get => _playerControl;
        private set
        {
            if (_playerControl != value)
            {
                _playerControl = value;
                OnPlayerControlChange?.Invoke(_playerControl);
            }
        }
    }

    public event Action<bool> OnCanChangeCardChange; //我方更換卡片改變事件
    private bool _canChangeCard; //我方是否可以更換卡片
    public bool canChangeCard
    {
        get => _canChangeCard;
        private set
        {
            if (_canChangeCard != value)
            {
                _canChangeCard = value;
                OnCanChangeCardChange?.Invoke(_canChangeCard);
            }
        }
    }
    public void SetCanChangeCard(bool b)
    {
        canChangeCard = b;
    }

    public event Action OnPlayerTurnStart; //輪到我方回合事件
    public event Action OnEnemyTurnStart; //輪到敵方回合事件

    public event Action<bool> OnFightOver; //戰鬥結束事件 (true = 玩家贏、false = 玩家輸)

    public event Action<Sprite> OnFightBackgroundImageChange; //戰鬥背景圖片改變事件

    //與戰鬥相關的所有音效播放的觸發統一經由戰鬥系統的接口來調用以統一管理
    public event Action<AudioClip> OnPlayBGM; //播放BGM事件   
    public event Action<AudioClip> OnPlayAudioForChip; //根據chip播放SFX事件
    public event Action<string> OnPlayAudioForName; //根據name播放SFX事件
    public void PlayBGM(AudioClip audioClip)
    {
        OnPlayBGM?.Invoke(audioClip);
    }
    public void PlayAudioForChip(AudioClip audioClip)
    {
        OnPlayAudioForChip?.Invoke(audioClip);
    }
    public void PlayAudioForName(string sfxName)
    {
        OnPlayAudioForName?.Invoke(sfxName);
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

    //重置一場新戰鬥
    public void ResetNewFight(MapCellFightSO so)
    {
        if (so == null)
            return;

        mapCellFightSO = so;

        //戰鬥數據初始化
        haveOver = mapCellFightSO.haveOver;
        overTurn = mapCellFightSO.overTurn;
        nowTurn = 0;
        playerDefense = new AttackModel(playerDefenseSO);
        playerControl = false;
        canChangeCard = false;
        turnOver = false;

        //播放BGM
        OnPlayBGM?.Invoke(mapCellFightSO.bgm);

        //更改戰鬥背景圖片
        OnFightBackgroundImageChange?.Invoke(mapCellFightSO.background);

        //由於執行順序問題，可能發生PlayerManager晚於此腳本掛載
        //導致該腳本初始化存取PlayerManager.player得到null的狀況
        //跑協程即可確保必定能等待到PlayerManager.player準備完成
        StartCoroutine(WaitPlayer());
    }

    IEnumerator WaitPlayer()
    {
        yield return new WaitUntil(() => PlayerManager.Instance.playerReadyCheck == true);        

        GetPlayer();

        //敵人數據初始化
        enemy = new CharacterModel(mapCellFightSO.characterSO);
        //監聽血量變動事件，用於判斷戰鬥勝負
        enemy.OnNowHpChange += OnFightWin;
        //為敵人立即添加地圖格資源所設置的所有效果
        foreach (BuffSO buff in mapCellFightSO.enemyBuffSOs)
        {
            enemy.AddBuff(buff);
        }
        //為敵人立即添加地圖格資源所設置的所有狀態
        foreach (StateSO state in mapCellFightSO.enemyStateSOs)
        {
            enemy.AddState(state);
        }

        //敵人行動數據初始化
        enemyAction = new EnemyActionModel(mapCellFightSO.enemyActionSO);
        //初次取得敵人行動
        enemyNowAction = new List<AttackModel>(enemyAction.TurnReturnAction(1));
        OnEnemyNowActionChange?.Invoke(enemyNowAction);

        //首先開始我方回合
        playerCoroutine = StartCoroutine(PlayerTurn());
    }

    void GetPlayer()
    {
        //重置我方角色的初始血量
        PlayerManager.Instance.ResetInitHp();
        //重置我方角色的AP量
        PlayerManager.Instance.ResetAp();
        //重置我方角色的效果和狀態
        PlayerManager.Instance.ResetBuff();
        PlayerManager.Instance.ResetState();
        //我方數據初始化
        player = PlayerManager.Instance.player;
        //監聽血量變動事件，用於判斷戰鬥勝負
        player.OnNowHpChange += OnFightLost;

        //紀錄首次出戰卡片
        PlayerManager.Instance.SetInitCards();
    }

    Coroutine playerCoroutine;
    Coroutine enemyCoroutine;

    //我方回合協程
    IEnumerator PlayerTurn()
    {
        //我方回合開始
        turnOver = false;

        //當前回合數+1        
        nowTurn += 1;
        //如果該場戰鬥有設定回合限制，當前回合數超出限制回合數，就判定玩家戰鬥失敗
        if (nowTurn > overTurn)
        {
            if (haveOver)
            {
                //戰鬥失敗
            }
        }
        FightLogManager.Instance.WriteFightLog($"{nowTurn} Round");

        yield return null;

        //通知我方回合開始
        OnPlayerTurnStart?.Invoke();

        PlayAudioForName("PlayerTurnStart");

        yield return null;

        //執行我方角色在回合開始時的行動
        player.OnTurnStart();

        yield return new WaitForSeconds(1);

        //觸發我方的自動施放類型的卡片技能
        PlayerManager.Instance.AllAttackUseAuto();

        playerControl = false;

        yield return new WaitForSeconds(0.25f);

        //觸發我方會自動攻擊的效果(概率施放)
        foreach (BuffModel buff in player.buffManager.GetBuffs())
        {
            if (buff != null && player.buffManager.AutoAttackFrom(buff) != null)
            {
                float r = UnityEngine.Random.Range(0, 100);
                if (r < buff.AutoAttackProbability())
                {
                    AttackSO a = player.buffManager.AutoAttackFrom(buff);
                    AttackModel am = new AttackModel(a);                    

                    if (player.buffManager.AutoAttackDamageFromCard(buff) != null)
                    {
                        CardSO c = player.buffManager.AutoAttackDamageFromCard(buff);
                        CardModel cm = new CardModel(c);

                        buff.BuffTrigger();

                        yield return StartCoroutine(PlayerAttackEnemyEnumerator(cm, am));

                        playerControl = false;

                        yield return null;
                    }   
                }                
            }                       
        }

        yield return null;

        PlayerManager.Instance.SetCurrentCards();

        yield return null;

        //睡眠狀態確認
        turnOver = player.OnSleep();        

        yield return null;

        //玩家控制權
        playerControl = true;
        canChangeCard = true;

        if (turnOver == true)
        {
            playerControl = false;
            canChangeCard = false;
        }        

        yield return new WaitUntil(() => turnOver == true);

        //執行我方角色在回合結束時的行動
        player.OnTurnOver();

        //我方回合結束，輪到敵人回合        
        playerControl = false;
        canChangeCard = false;
        PlayerManager.Instance.CheckCurrentCardSetHoldCardCantFight();

        enemyCoroutine = StartCoroutine(EnemyTurn());
        if (playerCoroutine != null)
        {
            StopCoroutine(playerCoroutine);
            playerCoroutine = null;
        }
    }

    //敵人回合協程
    IEnumerator EnemyTurn()
    {
        print("敵人回合開始");
        //敵人回合開始
        turnOver = false;

        yield return null;

        //通知敵人回合開始
        OnEnemyTurnStart?.Invoke();        

        PlayAudioForName("EnemyTurnStart");

        yield return null;

        //執行敵人角色在回合開始時的行動
        enemy.OnTurnStart();

        yield return new WaitForSeconds(1);

        //觸發敵方會自動攻擊的效果(概率施放)
        foreach (BuffModel buff in enemy.buffManager.GetBuffs())
        {
            if (buff != null && enemy.buffManager.AutoAttackFrom(buff) != null)
            {
                float r = UnityEngine.Random.Range(0, 100);
                if (r < buff.AutoAttackProbability())
                {
                    AttackSO a = enemy.buffManager.AutoAttackFrom(buff);
                    AttackModel am = new AttackModel(a);

                    if (enemy.buffManager.AutoAttackDamageFromCharacter(buff) != null)
                    {
                        buff.BuffTrigger();

                        yield return StartCoroutine(EnemyAttackPlayerEnumerator(am));

                        yield return null;
                    }
                }
            }
        }

        yield return null;

        //睡眠狀態確認
        turnOver = enemy.OnSleep();

        yield return null;

        if (turnOver == false)
        {
            //敵人執行所有的當前行動
            while (enemyNowAction.Count > 0)
            {
                AttackModel currentAttack = enemyNowAction[0];
                yield return StartCoroutine(EnemyAttackPlayerEnumerator(currentAttack));
                enemyNowAction.RemoveAt(0);
                OnEnemyNowActionChange?.Invoke(enemyNowAction);
                yield return null;
            }                       
        }

        yield return new WaitForSeconds(1);

        //執行完所有行動後更新下一回合的行動
        enemyNowAction = new List<AttackModel>(enemyAction.TurnReturnAction(nowTurn + 1));
        OnEnemyNowActionChange?.Invoke(enemyNowAction);
        foreach (var action in enemyNowAction)
        {
            action.UpdateAttack();
        }

        yield return null;

        //執行敵人角色在回合結束時的行動
        enemy.OnTurnOver();

        yield return null;

        turnOver = true;

        yield return new WaitUntil(() => turnOver == true);

        //敵人回合結束，輪到我方回合
        playerCoroutine = StartCoroutine(PlayerTurn());
        if (enemyCoroutine != null)
        {
            StopCoroutine(enemyCoroutine);
            enemyCoroutine = null;
        }
    }

    //我方攻擊敵人
    public void PlayerAttackEnemy(CardModel card, AttackModel attack)
    {
        playerControl = false;
        StartCoroutine(PlayerAttackEnemyEnumerator(card, attack));
    }
    IEnumerator PlayerAttackEnemyEnumerator(CardModel card, AttackModel attack)
    {
        yield return StartCoroutine(BattleManager.Instance.UseAttack(player, enemy, card, attack));
        playerControl = true;
    }

    //我方攻擊自己
    public void PlayerAttackMyself(CardModel card, AttackModel attack)
    {
        playerControl = false;
        StartCoroutine(PlayerAttackMyselfEnumerator(card, attack));
    }
    IEnumerator PlayerAttackMyselfEnumerator(CardModel card, AttackModel attack)
    {
        yield return StartCoroutine(BattleManager.Instance.UseAttack(player, player, card, attack));
        playerControl = true;
    }

    //敵人攻擊我方
    IEnumerator EnemyAttackPlayerEnumerator(AttackModel attack)
    {
        yield return StartCoroutine(BattleManager.Instance.UseAttack(enemy, player, null, attack));
    }

    //主動結束回合
    public void OnTurnOver()
    {
        turnOver = true;     
    }    

    //使用我方防禦技能
    public void OnUsePlayerDefense()
    {
        if(player.nowAp > 0)
        {
            StartCoroutine(UsePlayerDefenseEnumerator());
        }                    
    }

    public IEnumerator UsePlayerDefenseEnumerator()
    {
        playerControl = false;
        yield return StartCoroutine(BattleManager.Instance.UseAttack(player, enemy, null, playerDefense));
        playerControl = true;
    }

    //停止戰鬥
    public void OnStopFight()
    {
        playerControl = false;

        //停止我方和敵方的回合協程
        if (playerCoroutine != null)
        {
            StopCoroutine(playerCoroutine);
            playerCoroutine = null;
        }

        if (enemyCoroutine != null)
        {
            StopCoroutine(enemyCoroutine);
            enemyCoroutine = null;
        }

        enemy.OnNowHpChange -= OnFightWin;
        player.OnNowHpChange -= OnFightLost;

        //重置所有卡片為可出戰狀態
        PlayerManager.Instance.AllHoldCardSetCanFight();

        //重置所有卡片的消耗AP量
        PlayerManager.Instance.AllAttackResetUseAP();

        //重置所有出戰的卡片
        PlayerManager.Instance.ResetFightCard();
    }

    //玩家戰鬥勝利
    public void OnFightWin(float enemyNowHp, float enemyInitHp)
    {
        if(enemyNowHp <= 0)
        {
            playerControl = false;

            //停止我方和敵方的回合協程
            if (playerCoroutine != null)
            {
                StopCoroutine(playerCoroutine);
                playerCoroutine = null;
            }

            if (enemyCoroutine != null)
            {
                StopCoroutine(enemyCoroutine);
                enemyCoroutine = null;
            }

            enemy.OnNowHpChange -= OnFightWin;
            player.OnNowHpChange -= OnFightLost;

            //重置所有卡片為可出戰狀態
            PlayerManager.Instance.AllHoldCardSetCanFight();

            //重置所有卡片的消耗AP量
            PlayerManager.Instance.AllAttackResetUseAP();

            //重置所有出戰的卡片
            PlayerManager.Instance.ResetFightCard();

            //通知戰鬥結束
            OnFightOver?.Invoke(true);

            //存檔內容添加擊敗的BOSS的ID
            SaveManager.Instance.SetBeatEnemyID(enemy.id);
        }
    }

    //玩家戰鬥失敗
    public void OnFightLost(float playerNowHp, float playerInitHp)
    {
        if (playerNowHp <= 0)
        {
            playerControl = false;

            //停止我方和敵方的回合協程
            if (playerCoroutine != null)
            {
                StopCoroutine(playerCoroutine);
                playerCoroutine = null;
            }

            if (enemyCoroutine != null)
            {
                StopCoroutine(enemyCoroutine);
                enemyCoroutine = null;
            }

            enemy.OnNowHpChange -= OnFightWin;
            player.OnNowHpChange -= OnFightLost;

            //重置所有卡片為可出戰狀態
            PlayerManager.Instance.AllHoldCardSetCanFight();

            //重置所有卡片的消耗AP量
            PlayerManager.Instance.AllAttackResetUseAP();

            //重置所有出戰的卡片
            PlayerManager.Instance.ResetFightCard();

            //通知戰鬥結束
            OnFightOver?.Invoke(false);
        }       
    }
}

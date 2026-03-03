using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModel
{
    AttackSO attackSO; //資源
    //用於移除指定攻擊時比對參考資源
    public bool CheckAttackSO(AttackSO attackSO)
    {
        return this.attackSO == attackSO;
    }

    public string displayName { get; private set; } //名稱

    public string depiction { get; private set; } //敘述

    public Sprite image { get; private set; } //圖片

    public AudioClip source { get; private set; } //音效

    public float playTime; //演出時間

    public bool attackMyself { get; private set; } //是否攻擊自己

    public bool onceUse { get; private set; } //是否限制使用一次

    //確認條件判斷攻擊是否能使用
    public bool AttackConditionCheck()
    {
        bool canUse = true;
       
        foreach (var attackCondition in attackSO.attackConditions)
        {
            //(經過一定回合數)
            canUse = attackCondition.OnTurnAttackCondition(canUse, FightManager.Instance.nowTurn);
            //(指定COMBO數)
            canUse = attackCondition.OnComboAttackCondition(canUse, BattleManager.Instance.combo);
            //(特定卡片有出戰)
            canUse = attackCondition.OnHaveFightCardAttackCondition(canUse, PlayerManager.Instance.GetFightCards());

            if (FightManager.Instance.player != null)
            {
                //(我方血量低於一定比例)
                canUse = attackCondition.OnHpScaleAttackCondition(canUse, FightManager.Instance.player.nowHp / FightManager.Instance.player.initHp);
                //(我方持有特定效果)
                canUse = attackCondition.OnHaveBuffAttackCondition(canUse, FightManager.Instance.player.buffManager.GetBuffs());
            }

            if (FightManager.Instance.enemy != null)
            {
                //(敵方持有特定狀態)
                canUse = attackCondition.OnHaveStateAttackCondition(canUse, FightManager.Instance.enemy.stateManager.GetStates());
                //(敵方持有任何狀態)
                canUse = attackCondition.OnHaveAnyStateAttackCondition(canUse, FightManager.Instance.enemy.stateManager.GetStates());
            } 
        }

        return canUse;
    }

    public bool turnUseSkill { get; private set; } //上回合是否使用過技能

    int initUseAP; //初始設定的消耗AP量

    public int useAP { get; private set; } //消耗的AP量

    //疊加消耗的AP量
    public void AddUseAP()
    {
        turnUseSkill = true;
        if (useAP + 1 < 11)
        {
            if(stackAP == false)
            {
                useAP = initUseAP + 1;               
            }
            else
            {
                //設定為AP疊加的技能是永久疊層
                useAP += 1;
            }            
        }
    }

    //復原疊加的AP量
    public void RecoveUseAP()
    {
        //不是AP疊加的技能才能復原疊加的AP量
        if (stackAP == false)
        {
            //上回合沒有使用過技能才能復原疊加的AP量
            if (turnUseSkill == false)
            {
                useAP = initUseAP;
            }   
        }

        turnUseSkill = false;
    }

    //重置消耗的AP量為預設量 (適用於戰鬥結束)
    public void ResetUseAP()
    {
        useAP = initUseAP;
    }

    public bool stackAP { get; private set; } //是否每次使用後都會使AP量消耗量增加

    public int recoverAP { get; private set; } //恢復的AP量

    public bool useAttack { get; private set; } //是否會造成傷害

    public AttackType attackType { get; private set; } //攻擊類型分類

    public DamageType damageType { get; private set; } //傷害類型分類

    public AttackElement attackElement { get; private set; } //傷害屬性分類

    public float damageMultiplier { get; private set; } //傷害倍率

    public int attackCount { get; private set; } //攻擊次數

    public bool autoAttack { get; private set; } //是否自動施放

    public float autoProbability { get; private set; } //自動施放的機率

    //攻擊對攻擊方附加的效果組合
    public IReadOnlyList<AttackBuffs> OnAttackReturnAttackerAddBuffs()
    {
        return attackSO.attackerAddBuffs;
    }

    //攻擊對攻擊方移除的狀態組合
    public IReadOnlyList<AttackStates> OnAttackReturnAttackerDeleteStates()
    {
        return attackSO.attackerDeleteStates;
    }

    //攻擊對防守方移除的效果組合
    public IReadOnlyList<AttackBuffs> OnAttackReturnDefenserDeleteBuffs()
    {
        return attackSO.defenserDeleteBuffs;
    }

    //攻擊對防守方附加的狀態組合
    public IReadOnlyList<AttackStates> OnAttackReturnDefenserAddStates()
    {
        return attackSO.defenserAddStates;
    }

    public float healMultiplier; //恢復倍率

    public float healScale; //恢復比例

    public float accuracy { get; private set; } //命中率提升

    public int combo { get; private set; } //COMBO數提升

    public float deleteEnemyActionProbability { get; private set; } //移除敵人行動的機率

    //DOWN
    public IReadOnlyList<CardSO> OnAttackReturnDownCards()
    {
        return attackSO.downCards;
    }

    //變身
    public CardSO OnAttackReturnHenshinCard()
    {
        return attackSO.henshin;
    }

    //蘭斯專用移除無敵結界效果
    public AttackBuffs ReturnRanceDeleteBuff()
    {
        return attackSO.ranceDeleteBuff;
    }

    public event Action OnUpdateAttack; //更新攻擊事件
    public event Action OnUseAttack; //攻擊執行事件
    public void UpdateAttack()
    {
        OnUpdateAttack?.Invoke();
    }
    public void UseAttack()
    {
        OnUseAttack?.Invoke();
    }

    //建構子
    public AttackModel(AttackSO so)
    {
        attackSO = so;
        displayName = so.displayName;
        depiction = so.depiction;
        image = so.image;
        source = so.source;
        playTime = so.playTime;
        attackMyself = so.attackMyself;
        onceUse = so.onceUse;
        turnUseSkill = false;
        initUseAP = so.useAP;
        useAP = so.useAP;
        stackAP = so.stackAP;
        recoverAP = so.recoverAP;
        useAttack = so.useAttack;
        attackType = so.attackType;
        damageType = so.damageType;
        attackElement = so.attackElement;
        damageMultiplier = so.damageMultiplier;
        attackCount = so.attackCount;
        autoAttack = so.autoAttack;
        autoProbability = so.autoProbability;
        healMultiplier = so.healMultiplier;
        healScale = so.healScale;
        accuracy = so.accuracy;
        combo = so.combo;
        deleteEnemyActionProbability = so.deleteEnemyActionProbability;
    }
}

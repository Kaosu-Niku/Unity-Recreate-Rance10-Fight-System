using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffModel
{
    BuffSO buffSO; //資源
    //用於移除指定效果時比對參考資源
    public bool CheckBuffSO(BuffSO buffSO)
    {
        return this.buffSO == buffSO;
    }

    public string displayName { get; private set; } //名稱

    public string depiction { get; private set; } //敘述

    public Sprite image { get; private set; } //圖片

    //疊加效果
    public List<BuffSO> coverBuffs;

    public bool canRepeat { get; private set; } //是否可重複添加

    public int turn { get; private set; } //持續回合

    public bool haveTurn { get; private set; } //是否有回合限制

    public bool canDelete { get; private set; } //是否可被正常移除   

    public int nowTurn { get; private set; } //剩餘回合數
    public void SetNowTurn(int turn)
    {
        this.nowTurn = turn;
    }

    public event Action OnBuffTrigger; //效果觸發事件
    public void BuffTrigger()
    {
        OnBuffTrigger?.Invoke();
    }

    //被攻擊時是否無效化修正 (全攻擊)
    public bool OnAttackedInvalidOfAll(bool invalid) 
    {
        bool finalInvalid = buffSO.OnAttackedInvalidOfAll(invalid);
        if (finalInvalid != invalid)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalInvalid;
    }

    //被攻擊時是否無效化修正 (依攻擊類型)
    public bool OnAttackedInvalidOfAttackType(bool invalid, AttackType attackType)
    {
        bool finalInvalid = buffSO.OnAttackedInvalidOfAttackType(invalid, attackType);
        if (finalInvalid != invalid)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalInvalid;
    }

    //被攻擊時是否無效化修正 (依傷害類型)
    public bool OnAttackedInvalidOfDamageType(bool invalid, DamageType damageType)
    {
        bool finalInvalid = buffSO.OnAttackedInvalidOfDamageType(invalid, damageType);
        if (finalInvalid != invalid)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalInvalid;
    }

    //被攻擊時是否無效化修正 (依攻擊屬性)
    public bool OnAttackedInvalidOfAttackElement(bool invalid, AttackElement attackElement)
    {
        bool finalInvalid = buffSO.OnAttackedInvalidOfAttackElement(invalid, attackElement);
        if (finalInvalid != invalid)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalInvalid;
    }

    //被攻擊時是否無效化修正 (依卡片屬性)
    public bool OnAttackedInvalidOfCardElement(bool invalid, CardElement cardElement)
    {
        bool finalInvalid = buffSO.OnAttackedInvalidOfCardElement(invalid, cardElement);
        if (finalInvalid != invalid)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalInvalid;
    }

    //被攻擊時是否無效化修正 (依傷害量)
    public float OnAttackedInvalidOfDamage(float damage)
    {
        float finalDamage = buffSO.OnAttackedInvalidOfDamage(damage);
        if (finalDamage != damage)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalDamage;
    }

    //攻擊時命中率修正
    public float OnAttackAccuracy(float accuracy)
    {
        return buffSO.OnAttackAccuracy(accuracy);
    }

    //被攻擊時命中率修正
    public float OnAttackedAccuracy(float accuracy)
    {
        return buffSO.OnAttackedAccuracy(accuracy);
    }

    //攻擊時傷害修正
    public float OnAttackDamage(float damage, DamageType damageType) 
    {
        return buffSO.OnAttackDamage(damage, damageType); 
    }

    //被攻擊時傷害修正
    public float OnAttackedDamage(float damage, DamageType damageType) 
    {
        return buffSO.OnAttackedDamage(damage, damageType);
    }   

    //被攻擊時是否反擊修正
    public bool OnAttackedUseCounter(bool use, AttackType attackType) 
    {
        bool finalUse = buffSO.OnAttackedUseCounter(use, attackType);
        if (finalUse != use)
        {
            OnBuffTrigger?.Invoke();
        }
        return finalUse;
    }

    //被攻擊時反擊傷害修正
    public float OnAttackedUseCounterDamage(float damage) 
    {
        return buffSO.OnAttackedUseCounterDamage(damage);
    }

    //被攻擊時移除效果
    public BuffSO OnAttackedDeleteBuff(BuffSO deleteBuff, CharacterModel defenser) 
    {
        return buffSO.OnAttackedDeleteBuff(deleteBuff, defenser);
    }

    //自動攻擊的傷害來源 (我方是CardSO，敵方是CharacterSO)
    public CardSO AutoAttackDamageFromCard() 
    {
        return buffSO.AutoDamageFromCard();
    }
    public CharacterSO AutoAttackDamageFromCharacter() 
    {
        return buffSO.AutoDamageFromCharacter();
    }

    //自動攻擊的傷害倍率
    public AttackSO AutoAttackFrom() 
    {
        return buffSO.AutoAttackFrom();
    }

    //自動攻擊的發動機率
    public float AutoAttackProbability()
    {
        return buffSO.AutoAttackProbability();
    }

    //建構子
    public BuffModel(BuffSO so)
    {
        buffSO = so;
        displayName = so.displayName;
        depiction = so.depiction;
        image = so.image;
        coverBuffs = so.coverBuffs;
        canRepeat = so.canRepeat;
        turn = so.turn;
        haveTurn = so.haveTurn;
        canDelete = so.canDelete;        
        nowTurn = turn;
    }
}
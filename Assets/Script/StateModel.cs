using System;
using Unity.VisualScripting;
using UnityEngine;

public class StateModel
{
    StateSO stateSO; //資源
    //用於移除指定狀態時比對參考資源
    public bool CheckStateSO(StateSO stateSO)
    {
        return this.stateSO == stateSO;
    }

    public string displayName { get; private set; } //名稱

    public string depiction { get; private set; } //敘述

    public Sprite image { get; private set; } //圖片

    public int turn { get; private set; } //持續回合

    public int nowTurn { get; private set; } //剩餘回合數
    public void SetNowTurn(int turn)
    {
        this.nowTurn = turn;
    }

    public event Action OnStateTrigger; //狀態觸發事件

    //回合開始時調用
    public void OnTurnStart(CharacterModel character) 
    {
        bool actionCheck = stateSO.OnTurnStart(character, nowTurn);
        if (actionCheck == true)
        {
            OnStateTrigger?.Invoke();
        }
    }

    //回合結束時調用
    public void OnTurnOver(CharacterModel character)
    {
        bool actionCheck = stateSO.OnTurnOver(character, nowTurn);
        if (actionCheck == true)
        {
            OnStateTrigger?.Invoke();
        }
    }

    //睡眠狀態確認
    public bool OnSleep(bool sleep) 
    {
        bool finalSleep = stateSO.OnSleep(sleep);
        if (finalSleep != sleep)
        {
            OnStateTrigger?.Invoke();
        }
        return finalSleep;
    }

    //修改當前AP量時修正
    public int OnAddAP(int ap) 
    {
        int finalAp = stateSO.OnAddAP(ap);
        if (finalAp != ap)
        {
            OnStateTrigger?.Invoke();
        }
        return finalAp;
    }

    //增加當前血量時修正
    public float OnAddNowHp(float hp) 
    {
        float finalNowHp = stateSO.OnAddNowHp(hp);
        if (finalNowHp != hp)
        {
            OnStateTrigger?.Invoke();
        }
        return finalNowHp;
    }

    //攻擊時命中修正
    public float OnAttackAccuracy(float accuracy)
    {
        float finalAccuracy = stateSO.OnAttackAccuracy(accuracy);
        if (finalAccuracy != accuracy)
        {
            OnStateTrigger?.Invoke();
        }
        return finalAccuracy;
    }

    //被攻擊時命中修正
    public float OnAttackedAccuracy(float accuracy)
    {
        float finalAccuracy = stateSO.OnAttackAccuracy(accuracy);
        if (finalAccuracy != accuracy)
        {
            OnStateTrigger?.Invoke();
        }
        return finalAccuracy;
    }

    //攻擊時傷害修正
    public float OnAttackDamage(float damage) 
    {
        float finalDamage = stateSO.OnAttackDamage(damage);
        if (finalDamage != damage)
        {
            OnStateTrigger?.Invoke();
        }
        return finalDamage;
    }

    //被攻擊時傷害修正
    public float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage) 
    {
        float finalDamage = stateSO.OnAttackedDamage(attackElement, cardElement, damage);
        if (finalDamage != damage)
        {
            OnStateTrigger?.Invoke();
        }
        return finalDamage;
    }

    //被攻擊時移除狀態
    public StateSO OnAttackedDeleteState(StateSO deleteState, CharacterModel defenser) 
    {
        return stateSO.OnAttackedDeleteState(deleteState, defenser);
    }

    //建構子
    public StateModel(StateSO so)
    {
        stateSO = so;
        displayName = so.displayName;
        depiction = so.depiction;
        image = so.image;
        turn = so.trun;
        nowTurn = turn;
    }
}

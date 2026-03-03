using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WargSO", menuName = "Scriptable Objects/EnemyActionSO/WargSO")]
public class WargEnemyActionSO : EnemyActionSO
{
    [SerializeField] StateSO sleepState;
    [SerializeField] BuffSO strengthStackBuff;

    //瓦古的條件行動是多回合執行，但原架構只支援一回合的條件行動，為不影響原來的架構邏輯，直接在此改變行動
    public List<AttackSO> triggerAction1;
    public List<AttackSO> triggerAction2;

    //檢查敵人效果
    public override bool OnCheckEnemyBuffs(bool use, IReadOnlyList<BuffModel> buffs) 
    {
        foreach (var buff in buffs)
        {
            //敵人持有蓄力效果就滿足條件
            if (buff.CheckBuffSO(strengthStackBuff) == true)
            {
                return true;
            }
        }
        return use; 
    }


    //檢查玩家狀態
    public override bool OnCheckPlayerStates(bool use, IReadOnlyList<StateModel> states) 
    {        
        bool haveSleep = false;
        
        foreach (var state in states)
        {
            if (state.CheckStateSO(sleepState) == true)
            {
                haveSleep = true;
            }
        }

        //玩家持有睡眠狀態就滿足條件
        if (haveSleep == true)
        {
            //檢查的順序是先敵人效果後玩家狀態，因此此處得到的use值就可以表明敵人是否持有蓄力效果
            //在玩家持有睡眠狀態的前提下，如果敵人未持有蓄力效果就進行第1個行動，反之則是進行第2個行動
            if (use == false)
            {
                triggerAction.action = triggerAction1;
            }
            else
            {
                triggerAction.action = triggerAction2;
            }
            
            return true;
        }

        //如果敵人持有蓄力效果但玩家未持有睡眠狀態，則條件依舊算失敗，不可以執行任何條件行動
        if(use == true)
        {
            return false;
        }

        return use; 
    }
}

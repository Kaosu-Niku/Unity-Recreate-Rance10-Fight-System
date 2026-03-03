using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MediusaSO", menuName = "Scriptable Objects/EnemyActionSO/MediusaSO")]
public class MediusaEnemyActionSO : EnemyActionSO
{
    [SerializeField] StateSO FearState;

    //檢查玩家狀態
    public override bool OnCheckPlayerStates(bool use, IReadOnlyList<StateModel> states) 
    {        
        bool haveFear = false;
        
        foreach (var state in states)
        {
            if (state.CheckStateSO(FearState) == true)
            {
                haveFear = true;
            }
        }

        //玩家未持有恐懼狀態就滿足條件
        if (haveFear == false)
        {
            return true;
        }

        return use; 
    }
}

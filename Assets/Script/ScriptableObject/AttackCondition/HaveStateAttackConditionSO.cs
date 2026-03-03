using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaveStateAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/HaveStateAttackConditionSO")]
public class HaveStateAttackConditionSO : AttackConditionSO
{
    [Tooltip("持有狀態")]
    public StateSO haveState;

    //敵方未持有指定的狀態則條件不符
    public override bool OnHaveStateAttackCondition(bool b, IReadOnlyList<StateModel> states) 
    {
        foreach (StateModel state in states)
        {
            if (state.CheckStateSO(haveState) == true)
            {
                return b;
            }
        }

        return false;
    }
}

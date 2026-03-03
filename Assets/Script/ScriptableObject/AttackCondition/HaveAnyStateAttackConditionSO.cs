using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaveAnyStateAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/HaveAnyStateAttackConditionSO")]
public class HaveAnyStateAttackConditionSO : AttackConditionSO
{
    //敵方未持有任何狀態則條件不符
    public override bool OnHaveAnyStateAttackCondition(bool b, IReadOnlyList<StateModel> states) 
    {
        if (states.Count <= 0)
        {
            return false;
        }
        else
        {
            return b;
        }    
    }
}

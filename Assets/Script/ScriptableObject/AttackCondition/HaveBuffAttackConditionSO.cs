using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaveBuffAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/HaveBuffAttackConditionSO")]
public class HaveBuffAttackConditionSO : AttackConditionSO
{
    [Tooltip("持有效果")]
    public BuffSO haveBuff;

    //我方未持有指定的效果則條件不符
    public override bool OnHaveBuffAttackCondition(bool b, IReadOnlyList<BuffModel> buffs) 
    {
        foreach (BuffModel buff in buffs)
        {
            if(buff.CheckBuffSO(haveBuff) == true)
            {
                return b;
            }
        }

        return false;
    }
}

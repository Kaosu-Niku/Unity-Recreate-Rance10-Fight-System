using UnityEngine;

[CreateAssetMenu(fileName = "AttackConditionSO", menuName = "Scriptable Objects/AttackCondition/AttackConditionSO")]
public class DefaultAttackConditionSO : AttackConditionSO
{
    //攻擊是否能使用
    public override bool OnAttackCondition(bool b) { return b; }
}

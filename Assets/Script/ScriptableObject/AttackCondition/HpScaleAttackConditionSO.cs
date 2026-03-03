using UnityEngine;

[CreateAssetMenu(fileName = "HpScaleAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/HpScaleAttackConditionSO")]
public class HpScaleAttackConditionSO : AttackConditionSO
{
    [Tooltip("血量比例")]
    [Range(0, 100)]
    public float hpScale = 100;

    //我方當前血量比例高於條件設定的血量比例則條件不符
    public override bool OnHpScaleAttackCondition(bool b, float scale) 
    { 
        if(scale > hpScale)
        {
            return false;
        }
        else
        {
            return b;
        }        
    }
}

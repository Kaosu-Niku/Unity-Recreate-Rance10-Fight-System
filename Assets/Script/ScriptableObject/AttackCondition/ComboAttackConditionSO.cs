using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/ComboAttackConditionSO")]
public class ComboAttackConditionSO : AttackConditionSO
{
    [Tooltip("COMBO计")]
    public int combo = 0;

    //讽玡COMBO计ぃ琌﹚COMBO计玥兵ンぃ才
    public override bool OnComboAttackCondition(bool b, int nowCombo) 
    { 
        if(nowCombo != combo)
        {
            return false;
        }
        else
        {
            return b;
        }        
    }
}

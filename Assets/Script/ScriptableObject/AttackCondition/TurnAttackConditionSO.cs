using UnityEngine;

[CreateAssetMenu(fileName = "TurnAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/TurnAttackConditionSO")]
public class TurnAttackConditionSO : AttackConditionSO
{
    [Tooltip("计")]
    public int turn = 0;

    //讽玡计ゼ禬筁﹚计玥兵ンぃ才
    public override bool OnTurnAttackCondition(bool b, int nowTurn) 
    { 
        if(nowTurn < turn)
        {
            return false;
        }
        else
        {
            return b;
        }        
    }
}

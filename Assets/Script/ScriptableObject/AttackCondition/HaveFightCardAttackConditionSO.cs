using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HaveFightCardAttackConditionSO", menuName = "Scriptable Objects/AttackCondition/HaveFightCardAttackConditionSO")]
public class HaveFightCardAttackConditionSO : AttackConditionSO
{
    [Tooltip("出戰卡片")]
    public CardSO haveFightCard;

    //指定的卡片未出戰則條件不符
    public override bool OnHaveFightCardAttackCondition(bool b, IReadOnlyList<CardModel> fightCards) 
    {
        foreach (CardModel fightCard in fightCards)
        {
            if(fightCard.GetCardSO == haveFightCard)
            {
                return b;
            }
        }

        return false;
    }
}

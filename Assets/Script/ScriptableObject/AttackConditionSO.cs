using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackConditionSO : ScriptableObject
{
    //攻擊是否能使用 (無條件)
    public virtual bool OnAttackCondition(bool b) { return b; }

    //攻擊是否能使用 (我方血量低於一定比例)
    public virtual bool OnHpScaleAttackCondition(bool b, float scale) { return b; }

    //攻擊是否能使用 (經過一定回合數)
    public virtual bool OnTurnAttackCondition(bool b, int nowTurn) { return b; }

    //攻擊是否能使用 (指定COMBO數)
    public virtual bool OnComboAttackCondition(bool b, int nowCombo) { return b; }

    //攻擊是否能使用 (我方持有特定效果)
    public virtual bool OnHaveBuffAttackCondition(bool b, IReadOnlyList<BuffModel> buffs) { return b; }

    //攻擊是否能使用 (敵方持有特定狀態)
    public virtual bool OnHaveStateAttackCondition(bool b, IReadOnlyList<StateModel> states) { return b; }

    //攻擊是否能使用 (敵方持有任何狀態)
    public virtual bool OnHaveAnyStateAttackCondition(bool b, IReadOnlyList<StateModel> states) { return b; }

    //攻擊是否能使用 (特定卡片有出戰)
    public virtual bool OnHaveFightCardAttackCondition(bool b, IReadOnlyList<CardModel> fightCards) { return b; }
}

using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "CounterattackBuffSO", menuName = "Scriptable Objects/Buff/CounterattackBuffSO")]
public class CounterattackBuffSO : BuffSO
{
    [Header("===== 戰鬥相關 =====")]

    [Tooltip("是否全攻擊類型都會反擊\n設定為true則表示全攻擊類型都會反擊")]
    public bool allCounter = false;

    [Tooltip("反擊特定攻擊類型的傷害\nclose = 近戰\nFar = 遠程\nMagic = 魔法\nNothing = 無")]
    public AttackType counterAttackType;

    [Tooltip("反擊的傷害倍率\n值為0~無上限，2.5f表示倍率250%")]
    public float counterDamageMultiplier = 1;

    //被攻擊時是否反擊修正
    public override bool OnAttackedUseCounter(bool use, AttackType attackType) 
    {
        if (allCounter == true)
        {
            return true;
        }

        return attackType == counterAttackType;
    }

    //被攻擊時反擊傷害修正
    public override float OnAttackedUseCounterDamage(float damage) 
    {
        return damage * counterDamageMultiplier; 
    }
}

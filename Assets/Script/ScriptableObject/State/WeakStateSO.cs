using UnityEngine;

[CreateAssetMenu(fileName = "WeakStateSO", menuName = "Scriptable Objects/State/WeakStateSO")]
public class WeakStateSO : StateSO
{
    //被攻擊時傷害修正
    public override float OnAttackDamage(float damage)
    {
        //造成傷害-20%
        return damage * 0.8f;
    }

    //被攻擊時傷害修正
    public override float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage)
    {
        //受到傷害+20%
        return damage * 1.2f;
    }
}

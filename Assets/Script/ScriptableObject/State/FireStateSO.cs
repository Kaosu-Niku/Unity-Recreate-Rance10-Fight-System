using UnityEngine;

[CreateAssetMenu(fileName = "FireStateSO", menuName = "Scriptable Objects/State/FireStateSO")]
public class FireStateSO : StateSO
{
    //被攻擊時傷害修正
    public override float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage)
    {
        //受到火屬性傷害則傷害翻倍
        if(attackElement == AttackElement.Fire)
        {
            return damage * 2;
        }
        if (cardElement != null)
        {
            if (cardElement == CardElement.Fire)
            {
                return damage * 2;
            }
        }
        return damage;
    }
}

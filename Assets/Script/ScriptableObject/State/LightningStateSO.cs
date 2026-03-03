using UnityEngine;

[CreateAssetMenu(fileName = "LightningStateSO", menuName = "Scriptable Objects/State/LightningStateSO")]
public class LightningStateSO : StateSO
{
    //被攻擊時傷害修正
    public override float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage)
    {
        //受到雷屬性傷害則傷害翻倍
        if (attackElement == AttackElement.Thunder)
        {
            return damage * 2;
        }
        if(cardElement != null)
        {
            if (cardElement == CardElement.Thunder)
            {
                return damage * 2;
            }
        }
        return damage;
    }
}

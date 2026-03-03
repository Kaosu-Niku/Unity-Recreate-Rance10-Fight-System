using UnityEngine;

[CreateAssetMenu(fileName = "IceStateSO", menuName = "Scriptable Objects/State/IceStateSO")]
public class IceStateSO : StateSO
{
    //被攻擊時傷害修正
    public override float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage) {
        //受到冰屬性傷害則傷害翻倍
        if (attackElement == AttackElement.Ice)
        {
            return damage * 2;
        }
        if (cardElement != null)
        {
            if (cardElement == CardElement.Ice)
            {
                return damage * 2;
            }
        }
        return damage;
    }
}

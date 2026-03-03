using UnityEngine;

[CreateAssetMenu(fileName = "SleepStateSO", menuName = "Scriptable Objects/State/SleepStateSO")]
public class SleepStateSO : StateSO
{
    //睡眠狀態確認
    public override bool OnSleep(bool sleep) 
    {
        return true; 
    }

    //被攻擊時命中修正
    public override float OnAttackedAccuracy(float accuracy) 
    { 
        return accuracy + 1000; //等效必中
    }

    //被攻擊時傷害修正
    public override float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage)
    {
        //受到傷害+100%
        return damage * 2;
    }

    //被攻擊時移除狀態
    public override StateSO OnAttackedDeleteState(StateSO deleteState, CharacterModel defenser) 
    {
        return this;
    }
}

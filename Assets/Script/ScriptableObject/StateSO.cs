using UnityEngine;

public abstract class StateSO : ScriptableObject
{
    [Header("===== 基礎 =====")]

    [Tooltip("遊戲中顯示的狀態名稱")]
    public string displayName;

    [Tooltip("遊戲中顯示的狀態敘述")]
    [TextArea(2, 20)]
    public string depiction;

    [Tooltip("遊戲中顯示的狀態圖片")]
    public Sprite image;

    [Header("===== 戰鬥相關 =====")]

    [Tooltip("持續回合")]
    public int trun = 1;

    //回合開始時調用
    public virtual bool OnTurnStart(CharacterModel character, int stateTurn) { return false; }

    //回合結束時調用
    public virtual bool OnTurnOver(CharacterModel character, int stateTurn) { return false; }

    //睡眠狀態確認
    public virtual bool OnSleep(bool sleep) { return sleep; }

    //修改當前AP量時修正
    public virtual int OnAddAP(int ap) { return ap; }

    //增加當前血量時修正
    public virtual float OnAddNowHp(float hp) { return hp; }

    //攻擊時命中修正
    public virtual float OnAttackAccuracy(float accuracy) { return accuracy; }

    //被攻擊時命中修正
    public virtual float OnAttackedAccuracy(float accuracy) { return accuracy; }

    //攻擊時傷害修正
    public virtual float OnAttackDamage(float damage) { return damage; }

    //被攻擊時傷害修正
    public virtual float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage) { return damage; }

    //被攻擊時移除狀態
    public virtual StateSO OnAttackedDeleteState(StateSO deleteState, CharacterModel defenser) { return deleteState; }
}

//該狀態是屬於什麼類型
//public enum StateType
//{
//    Ice, //冰凍
//    Fire, //燃燒
//    Lightning, //觸電
//    Poison, //中毒
//    Weak, //虛弱
//    Chaos, //混亂
//    Fear, //恐懼
//    Sleep, //睡眠
//    NotHeal, //反再生
//    Dark, //黑暗
//    Curse, //詛咒
//    Death, //即死預告
//}

using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "InvalidBuffSO", menuName = "Scriptable Objects/Buff/InvalidBuffSO")]
public class InvalidBuffSO : BuffSO
{
    [Header("===== 戰鬥相關 =====")]

    [Tooltip("是否被攻擊後移除\n設定為true則表示被攻擊後移除")]
    public bool attackedDelete = false;

    [Tooltip("是否全攻擊無效\n設定為true則表示全攻擊無效")]
    public bool allInvalid = false;

    [Tooltip("是否依攻擊類型判斷攻擊無效\n設定為true則表示會依攻擊類型來判斷攻擊無效")]
    public bool checkAttackType = false;
    [Tooltip("使特定攻擊類型的攻擊無效\nclose = 近戰\nFar = 遠程\nMagic = 魔法\nNothing = 無")]
    public AttackType invalidAttackType;

    [Tooltip("是否依傷害類型判斷攻擊無效\n設定為true則表示會依傷害類型來判斷攻擊無效")]
    public bool checkDamageType = false;
    [Tooltip("使特定傷害類型的攻擊無效\nPhysical = 物理\nMagic = 魔法\nNothing = 無")]
    public DamageType invalidDamageType;

    [Tooltip("是否依攻擊屬性判斷攻擊無效\n設定為true則表示會依攻擊屬性來判斷攻擊無效")]
    public bool checkAttackElement = false;
    [Tooltip("使特定攻擊屬性的攻擊無效\nNothing = 無\nFire = 火\nIce = 冰\nLightning = 雷\nLight = 光\nDark = 闇")]
    public AttackElement invalidAttackElement;
    [Tooltip("使特定卡片屬性的攻擊無效\nNothing = 無\nFire = 火\nIce = 冰\nLightning = 雷\nLight = 光\nDark = 闇")]
    public CardElement invalidCardElement;

    [Tooltip("是否依造成傷害量判斷攻擊無效\n設定為true則表示會依造成傷害量來判斷攻擊無效")]
    public bool checkDamage = false;
    [Tooltip("使傷害量低於設定值的攻擊無效\n若傷害量超出設定值，則改為造成固定100的傷害")]
    public float invalidDamage;

    //被攻擊時是否無效化修正 (全攻擊)
    public override bool OnAttackedInvalidOfAll(bool invalid)
    {
        if (allInvalid == true)
        {
            return true;
        }

        return invalid;
    }

    //被攻擊時是否無效化修正 (依攻擊類型)
    public override bool OnAttackedInvalidOfAttackType(bool invalid, AttackType attackType) 
    {
        if (checkAttackType == true)
        {
            return attackType == invalidAttackType;
        }

        return invalid;
    }

    //被攻擊時是否無效化修正 (依傷害類型)
    public override bool OnAttackedInvalidOfDamageType(bool invalid, DamageType damageType)
    {
        if (checkDamageType == true)
        {
            return damageType == invalidDamageType;
        }

        return invalid;
    }

    //被攻擊時是否無效化修正 (依攻擊屬性)
    public override bool OnAttackedInvalidOfAttackElement(bool invalid, AttackElement attackElement)
    {
        if (checkAttackElement == true)
        {
            return attackElement == invalidAttackElement;
        }

        return invalid;
    }

    //被攻擊時是否無效化修正 (依卡片屬性)
    public override bool OnAttackedInvalidOfCardElement(bool invalid, CardElement cardElement)
    {
        if (checkAttackElement == true)
        {
            return cardElement == invalidCardElement;
        }

        return invalid;
    }

    //被攻擊時是否無效化修正 (依傷害量)
    public override float OnAttackedInvalidOfDamage(float damage)
    {
        if (checkDamage == true)
        {
            if (damage < invalidDamage)
            {
                return 0;
            }
            else
            {
                return 100;
            }
        }

        return damage;
    }

    //被攻擊時移除效果
    public override BuffSO OnAttackedDeleteBuff(BuffSO deleteBuff, CharacterModel defenser) 
    { 
        if(attackedDelete == true)
        {
            if(deleteBuff == null)
            {
                return this;
            }            
        }

        return deleteBuff;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "DefenseBuffSO", menuName = "Scriptable Objects/Buff/DefenseBuffSO")]
public class DefenseBuffSO : BuffSO
{
    [Header("===== 戰鬥相關 =====")]

    [Tooltip("全類型減傷\n值為0~100，75表示減傷75%")]
    [Range(0, 100)]
    public float allDefense = 0;

    [Tooltip("物理減傷\n值為0~100，75表示減傷75%")]
    [Range(0, 100)]
    public float physicalDefense = 0;

    [Tooltip("魔法減傷\n值為0~100，75表示減傷75%")]
    [Range(0, 100)]
    public float magicDefense = 0;

    //被攻擊時傷害修正
    public override float OnAttackedDamage(float damage, DamageType damageType)
    {
        float finalDamage = damage;

        //全類型減傷
        finalDamage = finalDamage * (1 - (allDefense / 100));

        //物理減傷
        if(damageType == DamageType.Physical)
        {
            finalDamage = finalDamage * (1 - (physicalDefense / 100));
        }
        
        //魔法減傷
        if (damageType == DamageType.Magic)
        {
            finalDamage = finalDamage * (1 - (magicDefense / 100));
        }
        
        return finalDamage;
    }
}

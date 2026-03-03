using UnityEngine;

[CreateAssetMenu(fileName = "AttackBuffSO", menuName = "Scriptable Objects/Buff/AttackBuffSO")]
public class AttackBuffSO : BuffSO
{
    [Header("===== 戰鬥相關 =====")]

    [Tooltip("全類型增傷\n值為0~無上限，75表示增傷75%")]
    public float allAttack = 0;

    [Tooltip("物理增傷\n值為0~無上限，75表示增傷75%")]
    public float physicalAttack = 0;

    [Tooltip("魔法增傷\n值為0~無上限，75表示增傷75%")]
    public float magicAttack = 0;

    [Tooltip("命中率\n值為正負無上限，75表示命中率+75%、-25表示命中率-25%")]
    public float Accuracy = 0;

    //攻擊時命中率修正
    public override float OnAttackAccuracy(float accuracy)
    {
        return accuracy + Accuracy;
    }

    //攻擊時傷害修正
    public override float OnAttackDamage(float damage, DamageType damageType) 
    {
        float finalDamage = damage;

        //全類型增傷
        finalDamage = finalDamage * (1 + (allAttack / 100));

        //物理增傷
        if (damageType == DamageType.Physical)
        {
            finalDamage = finalDamage * (1 + (physicalAttack / 100));
        }        

        //魔法增傷
        if (damageType == DamageType.Magic)
        {
            finalDamage = finalDamage * (1 + (magicAttack / 100));
        }       

        return finalDamage; 
    }    
}

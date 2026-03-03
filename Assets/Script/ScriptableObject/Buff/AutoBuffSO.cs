using UnityEngine;

[CreateAssetMenu(fileName = "AutoBuffSO", menuName = "Scriptable Objects/Buff/AutoBuffSO")]
public class AutoBuffSO : BuffSO
{
    [Header("===== 戰鬥相關 =====")]

    [Tooltip("傷害來源 (適用於我方)\n對於我方來說，支援是透過卡片技能添加的，因此傷害來源是參考卡片")]
    public CardSO DamageFromCard;

    [Tooltip("傷害來源 (適用於敵方)\n對於敵方來說，支援是敵方角色的攻擊技能添加的，因此傷害來源是參考敵方角色")]
    public CharacterSO DamageFromCharacter;

    [Tooltip("攻擊參考")]
    public AttackSO attackFrom;

    [Tooltip("發動攻擊的機率")]
    [Range(0, 100)]
    public float attackProbability;

    //自動發動攻擊時的傷害來源 (我方是CardSO，敵方是CharacterSO)
    public override CardSO AutoDamageFromCard() 
    { 
        if(DamageFromCard != null)
        {
            return DamageFromCard;
        }
        
        return null; 
    }
    public override CharacterSO AutoDamageFromCharacter() 
    {
        if (DamageFromCharacter != null)
        {
            return DamageFromCharacter;
        }

        return null; 
    }

    //自動攻擊的傷害倍率
    public override AttackSO AutoAttackFrom() 
    { 
        return attackFrom; 
    }

    //自動攻擊的發動機率
    public override float AutoAttackProbability()
    {
        return attackProbability;
    }
}

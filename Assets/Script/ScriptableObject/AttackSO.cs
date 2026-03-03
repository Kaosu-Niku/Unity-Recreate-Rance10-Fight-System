using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSO : ScriptableObject
{
    [Header("===== 基礎 =====")]

    [Tooltip("遊戲中顯示的技能名稱")]
    public string displayName;

    [Tooltip("遊戲中顯示的技能敘述")]
    [TextArea(3, 20)]
    public string depiction;

    [Tooltip("遊戲中顯示的技能圖片")]
    public Sprite image;

    [Tooltip("遊戲中發動攻擊時播放的音效")]
    public AudioClip source;

    [Tooltip("遊戲中發動攻擊時的演出時間")]
    public float playTime;

    [Tooltip("是否攻擊自己\n(適用於自爆類或治療類的行動)")]
    public bool attackMyself = false;

    [Header("===== 條件相關 =====")]

    [Tooltip("是否限制使用一次")]
    public bool onceUse = false;

    [Tooltip("使用條件")]
    public List<AttackConditionSO> attackConditions;

    [Header("===== AP相關 =====")]

    [Tooltip("消耗的AP量")]
    public int useAP = 0;

    [Tooltip("是否每次使用後都會使AP量消耗量增加")]
    public bool stackAP = false;

    [Tooltip("恢復的AP量")]
    public int recoverAP = 0;

    [Header("===== 傷害相關 =====")]

    [Tooltip("是否會造成傷害\n設定為false則表示此攻擊不會造成傷害，適用於輔助類技能")]
    public bool useAttack = true;

    [Tooltip("攻擊類型分類\nclose = 近戰\nFar = 遠程\nMagic = 魔法\nNothing = 無")]
    public AttackType attackType;

    [Tooltip("傷害類型分類\nPhysical = 物理\nMagic = 魔法\nNothing = 無")]
    public DamageType damageType;

    [Tooltip("傷害屬性分類\nNothing = 無\nFire = 火\nIce = 冰\nLightning = 雷\nLight = 光\nDark = 闇")]
    public AttackElement attackElement;

    [Tooltip("傷害倍率\n值為0~無上限，2.5f表示倍率250%")]
    public float damageMultiplier = 1;

    [Tooltip("攻擊次數")]
    public int attackCount = 1;

    [Header("===== 自動相關 =====")]

    [Tooltip("是否自動施放\n設定為true則表示此攻擊會自動施放")]
    public bool autoAttack = false;

    [Tooltip("自動施放的機率")]
    [Range(0, 100)]
    public float autoProbability = 100;

    [Header("===== 效果和狀態相關 =====")]

    [Tooltip("對自身附加指定效果\n效果對象BuffSO必須指定，如未指定不會附加任何效果")]
    public List<AttackBuffs> attackerAddBuffs;

    [Tooltip("對自身移除指定狀態\n狀態對象StateSO可不指定，如未指定則表示隨機移除一個狀態")]
    public List<AttackStates> attackerDeleteStates;

    [Tooltip("對敵人移除指定效果\n效果對象BuffSO可不指定，如未指定則表示隨機移除一個效果")]
    public List<AttackBuffs> defenserDeleteBuffs;

    [Tooltip("對敵人附加指定狀態\n狀態對象StateSO必須指定，如未指定不會附加任何狀態")]
    public List<AttackStates> defenserAddStates;

    [Header("===== 恢復相關 =====")]

    [Tooltip("恢復倍率\n值為0~無上限，2.5f表示倍率250%\n(攻擊力 * 恢復倍率 = 治療量)")]
    public float healMultiplier = 0;

    [Tooltip("恢復比例\n值為0~100，25表示比例25%\n(總血量 * 恢復比例 = 治療量)")]
    [Range(0, 100)]
    public float healScale = 0;

    [Header("===== 其他特殊效果 =====")]

    [Tooltip("命中率提升\n值為正負無上限，75表示命中率+75%、-25表示命中率-25%")]
    public float accuracy = 0;

    [Tooltip("COMBO數提升\n值為0~無上限，2表示COMBO額外+2")]
    public int combo = 0;

    [Tooltip("移除敵人行動的機率\n(此為針對敵人生效的我方專用效果)\n值為0~100，75表示有75%的機率移除敵人當前行動序列中的一個攻擊")]
    [Range(0, 100)]
    public float deleteEnemyActionProbability = 0;

    [Tooltip("DOWN\n(此為針對我方卡片生效的敵人專用效果)\n如有指定卡片且該卡片在場則優先DOWN該卡片，若無指定卡片或該卡片不在場則隨機DOWN一個在場的卡片")]
    public List<CardSO> downCards;

    [Tooltip("變身\n(此為我方專用效果)\n將使用此攻擊的卡片換成指定卡片")]
    public CardSO henshin;

    [Header("===== 蘭斯專用 =====")]

    [Tooltip("蘭斯專用移除無敵結界效果")]
    public AttackBuffs ranceDeleteBuff;
}

//該攻擊是屬於什麼類型
public enum AttackType
{
    close, //近戰
    Far, //遠程
    Magic, //魔法
    Nothing, //無
}

//該傷害是屬於什麼類型
public enum DamageType
{
    Physical, //物理
    Magic, //魔法
    Nothing, //無
}

//該攻擊是屬於什麼屬性
public enum AttackElement
{
    Nothing, //無
    Fire, //火
    Ice, //冰
    Thunder, //雷
    Light, //光
    Dark, //闇
}

//效果模型
[Serializable]
public class AttackBuffs
{
    [Tooltip("指定效果")]
    public BuffSO attackBuff;

    [Tooltip("成功的機率")]
    [Range(0, 100)]
    public float attackBuffProbability = 100f;
}

//狀態模型
[Serializable]
public class AttackStates
{
    [Tooltip("指定狀態")]
    public StateSO attackState;

    [Tooltip("成功的機率")]
    [Range(0, 200)]
    public float attackStateProbability = 100f;
}

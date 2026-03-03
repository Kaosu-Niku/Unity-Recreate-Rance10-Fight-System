using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSO", menuName = "Scriptable Objects/Buff/BuffSO")]
public abstract class BuffSO : ScriptableObject
{
    [Header("===== 基礎 =====")]

    [Tooltip("遊戲中顯示的效果名稱")]
    public string displayName;

    [Tooltip("遊戲中顯示的效果敘述")]
    [TextArea(3, 20)]
    public string depiction;

    [Tooltip("遊戲中顯示的效果圖片")]
    public Sprite image;

    [Header("===== 設定 =====")]

    [Tooltip("疊加效果\n\n" +
        "註: 重複添加同名的效果時，預設是會將舊的移除後添加上新的，只會保持一個在場，但有部分效果較為特殊可以疊加\n" +
        "疊加效果以層數來對應不同的數據，每次新增一個同名效果等同層數+1\n\n" +
        "(ex: 軍師效果第一次新增是新增軍師效果1，若再次新增軍師效果，則移除軍師效果1，並新增軍師效果2\n" +
        "軍師效果最高是6層，若已經6層的情況下再次新增軍師效果，則移除舊的軍師效果6，並新增新的軍師效果6)")]
    public List<BuffSO> coverBuffs;

    [Tooltip("是否可重複添加\n設定為true則表示此效果可重複添加\n\n" +
        "註: 重複添加同名的效果時，預設是會將舊的移除後添加上新的，只會保持一個在場，但有部分效果較為特殊可以重複添加")]
    public bool canRepeat = false;

    [Tooltip("持續回合")]
    public int turn = 1;

    [Tooltip("是否有回合限制\n設定為true則表示此效果的持續回合結束後會自動移除")]
    public bool haveTurn = true;

    [Tooltip("是否可正常移除\n設定為true則表示此效果可被一般的移除技能給移除\n\n" +
        "註: 一般的移除技能為不指定效果對象而隨機移除的類型，而有少部分的技能具有針對指定效果對象做移除\n" +
        "此類技能視為特殊的移除技能，不受此屬性的設定影響\n\n" +
        "(ex: 無敵結界效果可被蘭斯和健太郎的所有技能做特殊移除、重裝甲效果可被名稱帶有裝甲破壞的類似技能做特殊移除)")]
    public bool canDelete = true;

    //被攻擊時是否無效化修正 (全攻擊)
    public virtual bool OnAttackedInvalidOfAll(bool invalid) { return invalid; }

    //被攻擊時是否無效化修正 (依攻擊類型)
    public virtual bool OnAttackedInvalidOfAttackType(bool invalid, AttackType attackType) { return invalid; }

    //被攻擊時是否無效化修正 (依傷害類型)
    public virtual bool OnAttackedInvalidOfDamageType(bool invalid, DamageType damageType) { return invalid; }

    //被攻擊時是否無效化修正 (依攻擊屬性)
    public virtual bool OnAttackedInvalidOfAttackElement(bool invalid, AttackElement attackElement) { return invalid; }

    //被攻擊時是否無效化修正 (依卡片屬性)
    public virtual bool OnAttackedInvalidOfCardElement(bool invalid, CardElement cardElement) { return invalid; }

    //被攻擊時是否無效化修正 (依傷害量)
    public virtual float OnAttackedInvalidOfDamage(float damage) { return damage; }

    //攻擊時命中率修正
    public virtual float OnAttackAccuracy(float accuracy) { return accuracy; }

    //被攻擊時命中率修正
    public virtual float OnAttackedAccuracy(float accuracy) { return accuracy; }

    //攻擊時傷害修正
    public virtual float OnAttackDamage(float damage, DamageType damageType) { return damage; }

    //被攻擊時傷害修正
    public virtual float OnAttackedDamage(float damage, DamageType damageType) { return damage; }

    //被攻擊時是否反擊修正
    public virtual bool OnAttackedUseCounter(bool use, AttackType attackType) { return use; }

    //被攻擊時反擊傷害修正
    public virtual float OnAttackedUseCounterDamage(float damage) { return damage; }

    //被攻擊時移除效果
    public virtual BuffSO OnAttackedDeleteBuff(BuffSO deleteBuff, CharacterModel defenser) { return deleteBuff; }

    //自動攻擊的傷害來源 (我方是CardSO，敵方是CharacterSO)
    public virtual CardSO AutoDamageFromCard() { return null; }
    public virtual CharacterSO AutoDamageFromCharacter() { return null; }

    //自動攻擊的攻擊參考
    public virtual AttackSO AutoAttackFrom() { return null; }

    //自動攻擊的發動機率
    public virtual float AutoAttackProbability() { return 0; }
}

//該效果是屬於什麼類型
//public enum BuffType
//{
//    //此處需先註明幾項要點
//    //攻擊分成: 近戰、遠程、魔法
//    //傷害分成: 物理、魔法
//    //而最終實際只有三種組合，近戰物理、遠程物理、魔法魔法

//    //防禦
//    Defense, //(1回合) 最多可疊加4層的全類型減傷，1層 = 減傷30%、2層 = 減傷50%、3層 = 減傷70%、4層 = 減傷90%
//    IronWill, //(3回合)(不可移除) 物理減傷70% 魔法減傷70%
//    Wall, //(7回合) 物理減傷20%
//    PhysicalDefense1, //(2回合) 物理減傷20%
//    PhysicalDefense1+, (8回合) 物理減傷20%
//    PhysicalDefense2, //(2回合) 物理減傷40%
//    PhysicalDefense3, //(2回合) 物理減傷80%
//                          //Grandfather
//    MagicResistanceDevice, //(3回合) 魔法減傷40%
//    MagicDefense3, //(2回合) 魔法減傷80%
//    HeavyArmor1, //(永續)(不可移除)(特定技能可移除) 物理減傷10% 魔法減傷10%
//    HeavyArmor2, //(永續)(不可移除)(特定技能可移除) 物理減傷20% 魔法減傷20%
//    HeavyArmor3, //(永續)(不可移除)(特定技能可移除) 物理減傷30% 魔法減傷30%
//    HeavyArmor5, //(永續)(不可移除)(特定技能可移除) 物理減傷50% 魔法減傷50%
//    HeavyArmorPhysical, //(1回合)(不可移除) 物理減傷50%
//    HeavyArmorMagicResistance, //(1回合)(不可移除) 魔法減傷50%
//    BeneathTheSurface, //(1回合)(不可移除) 物理減傷70% 魔法減傷70%

//    //攻擊
//    MilitaryAdvisorEffect, //(?回合) 最多可疊加6層的全類型增傷，每層增傷10%，剩餘回合數根據層數有差，1層 = 9回合、2層 = 9回合、3層 = 4回合、4層 = 4回合、5層 = 2回合、6層 = 2回合
//    Encourage, //(5回合) 物理增傷20%
//    SiegeOperations, //(5回合) 物理增傷20% 命中率提升50%
//    Enthusiasm, //(3回合) 物理增傷50%
//    TheBlessingOfBishamonten, //(5回合) 物理增傷40%
//    MagicCircle, //(5回合) 最多可疊加4層的魔法增傷，1層 = 增傷20%、2層 = 增傷40%、3層 = 增傷80%、4層 = 增傷100%
//    PinkCloud, //(4回合) 魔法增傷40%
//    Combat AI, //(永續) 最多可疊加3層的物理增傷，1層 = 增傷80%、2層 = 增傷80%、3層 = 增傷80%
//    Thunderclouds, //(9回合) 最多可疊加4層的魔法增傷，1層 = 增傷20%、2層 = 增傷50%、3層 = 增傷80%、4層 = 增傷100%
//    Strength Stack, //(2回合) 最多可疊加3層的物理增傷，1層 = 增傷100%、2層 = 增傷100%、3層 = 增傷100%
//    Accelerator, //(3回合) 物理增傷50%
//    Rage, //(3回合)(不可移除) 最多可疊加3層的物理增傷，1層 = 增傷50%、2層 = 增傷100%、3層 = 增傷150%
//                      //Strength Stack (n/3)
//    ManaBattery, //(9回合) 魔法增傷20%

//    //反擊
//    BuyranConstitution, //(3回合) 反擊全類型攻擊
//    RayCounterattack, //(永續)(不可移除) 反擊近戰攻擊
//    AncientSpeciesConstitution, //(永續)(不可移除) 反擊近戰攻擊

//    //無效
//    MagicBarrier, //(1回合) 所有技能無效一次 (僅一次，無效技能後移除此效果)
//    NinpoTransformation, //(1回合) 所有技能無效一次 (僅一次，無效技能後移除此效果)
//    StayAwayFromTheEnemy, //(1回合) 近戰攻擊無效
//    FlightStatus, //(永續) 近戰攻擊無效
//    InvincibleBarrier, //(永續)(不可移除)(特定技能可移除) 攻擊技能無效
//    RestrictedArmor, //(永續)(不可移除) 造成的傷害未達到指定值則無效，達到則造成固定100傷害
//    LittleFullArmor, //(永續)(不可移除) 造成的傷害未達到20萬則無效，達到則造成固定100傷害
//    HoneyConstitution, //(永續)(不可移除) 魔法傷害無效
//    RayConstitution, //(永續)(不可移除) 雷屬性傷害無效
//    HauserConstitution, //(永續)(不可移除) 火屬性傷害無效
//    SiselleConstitution, //(永續)(不可移除) 冰屬性傷害無效
//                      //CandelFlight
//    Evasion, //(3回合) 閃避率提升30%
//    SleepInvalidConstitution, //(永續)(不可移除) 睡眠狀態無效
//    BarrageInAtlanta, //(2回合) 近戰攻擊無效
//    BarrageInJuno, //(2回合) 遠程攻擊無效

//    //地形
//    Sandbags, //(永續)(不可移除)(特定技能可移除) 物理減傷20%
//    Fort, //(永續)(不可移除)(特定技能可移除) 魔法減傷20%
//    NarrowIndoor, //(永續)(不可移除)(特定技能可移除) 遠程攻擊減傷50%
//    Pitfalls, //(永續)(不可移除)(特定技能可移除) 近戰攻擊減傷50%
//              //Flower tones
//    Fog, //(永續)(不可移除) 每回合只能行動2次
//    Desert, //(永續)(不可移除) 每回合5%機率DOWN一個角色
//    Storm, //(永續)(不可移除)(特定技能可移除) 每回合50%機率添加虛弱狀態

//    //回復
//    //Great Spirit
//    //Lorul Horus
//    //Puryo constitution (n/2)
//    //Mannian Temple

//    //其他
//    //Ageha
//    //Decelerator
//    //Decoy
//    //Nina's neck

//    //特殊
//    //Hostage route
//    //Chaliera
//    SensorJizo, //(永續) 無效果
//                //Miki Kurimizu
//                //White Knights
//    IntellectualAwakening, //(9回合) 最多可疊加4層，疊滿4層立即使我方戰鬥失敗
//    InsensitiveMind, //(2回合) 無效果
//    Inspiration, //(2回合) 無效果
//                 //Gate preparation
//                 //Convoy
//    LifeExpectancyIsShort, //(6回合)(不可移除) 剩餘回合數結束後使玩家戰鬥勝利
//                           //Fighting Zetas

//    //支援
//    Shizuka, //(3回合) 每回合80%機率發動魔法攻擊
//    Nagi, //(3回合) 每回合80%機率發動魔法攻擊
//    ZethMagicWeapon, //(3回合) 每回合80%機率發動魔法攻擊
//    TulipSoldier, //(6回合) 每回合50%機率發動遠程攻擊
//    TulipNo2, //(4回合) 每回合25%機率發動遠程攻擊
//    TulipNo3H, //(4回合) 每回合100%機率發動遠程攻擊
//    Darklance, //(4回合) 每回合80%機率發動近戰攻擊
//    Caesar, //(永續) 每回合80%機率發動近戰攻擊
//            //Split Pig (n/4)
//            //Child of Fire
//    Ambush, //(4回合) 每回合80%機率發動近戰攻擊
//    AssaultMonsterSoldier, //(3回合) 每回合80%機率發動近戰攻擊
//    RangedMonsterSoldier, //(3回合) 每回合80%機率發動遠程攻擊
//    MagicMonsterSoldier, //(3回合) 每回合80%機率發動魔法攻擊
//            //WagonMonsterSoldier
//    Uppi, //(永續) 每回合80%機率發動遠程攻擊
//    Swordsman, //(永續) 每回合80%機率發動近戰攻擊
//    Apache, //(永續) 每回合80%機率發動近戰攻擊
//    MariettaCorps, //(永續) 每回合80%機率發動遠程攻擊
//    Vampire, //(4回合) 每回合80%機率發動近戰攻擊
//    HoneyKing, //(永續) 每回合80%機率發動遠程攻擊
//    Arashi, //(3回合)(不可移除) 每回合80%機率發動遠程攻擊
//    MajinSatella, //(永續) 每回合80%機率發動近戰攻擊
//    YukiChan, //(8回合) 每回合80%機率發動魔法攻擊
//    FlameScrivener, //(8回合) 每回合80%機率發動魔法攻擊
//    Ghost, //(8回合) 每回合80%機率發動近戰攻擊
//    PI3, //(永續) 每回合80%機率發動遠程攻擊
//    SatelliteWeapons, //(永續) 每回合80%機率發動遠程攻擊
//    FactoryManagerRobo, //(永續) 每回合80%機率發動近戰攻擊
//    NewWeapons, //(永續) 每回合80%機率發動遠程攻擊
//    PoppinsSoldier, //(3回合) 每回合80%機率發動遠程攻擊
//    ElenaBandits, //(3回合) 每回合80%機率發動近戰攻擊
//    DXMembers, //(6回合) 每回合80%機率發動近戰攻擊

//    //結算
//    //Meal tickets are still in store
//    //Treasure Holding
//    //Enemy capture
//}

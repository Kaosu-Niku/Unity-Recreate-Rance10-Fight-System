using UnityEngine;

public abstract class CardSO : ScriptableObject
{
    [Header("===== 基礎 =====")]

    [Tooltip("遊戲中顯示的卡片名稱")]
    public string displayName;

    [Tooltip("遊戲中顯示的卡片圖片")]
    public Sprite image;

    [Header("===== 戰鬥數據相關 =====")]

    [Tooltip("屬性分類\nNothing = 無\nFire = 火\nIce = 冰\nLightning = 雷\nLight = 光\nDark = 闇")]
    public CardElement element;

    [Tooltip("陣營分類\nRance = 主人公\nRezas = 利薩斯\nHermann = 赫爾曼\nZeth = 賽斯\nFreeCity = 自由都市\n" +
        "Japan = Japan\nOther = 其他\nDemihuman = 亞人\nMonster = 怪物\nGodAndDemon = 神魔")]
    public Camp camp;

    [Tooltip("等級")]
    public int level;

    [Tooltip("血量")]
    public float hp;

    [Tooltip("攻擊力")]
    public float atk;

    [Header("===== 戰鬥技能相關 =====")]

    [Tooltip("一技能")]
    public AttackSO attack1;

    [Tooltip("二技能")]
    public AttackSO attack2;       
}

//屬性
public enum CardElement
{
    Nothing, //無
    Fire, //火
    Ice, //冰
    Thunder, //雷
    Light, //光
    Dark, //闇
}

//陣營
public enum Camp
{
    Rance, //主人公
    Rezas, //利薩斯
    Hermann, //赫爾曼
    Zeth, //賽斯
    FreeCity, //自由都市
    Japan, //Japan
    Other, //其他
    Demihuman, //亞人
    Monster, //怪物
    GodAndDemon, //神魔
}

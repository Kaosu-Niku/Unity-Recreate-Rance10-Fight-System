using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterSO : ScriptableObject
{
    [Header("===== 基礎 =====")]

    [Tooltip("角色Id")]
    public int id;

    [Tooltip("遊戲中顯示的角色名稱")]
    public string displayName;

    [Tooltip("遊戲中顯示的角色圖片")]
    public Sprite image;

    [Header("===== 戰鬥數據相關 =====")]

    [Tooltip("血量")]
    public float hp;

    [Tooltip("攻擊力")]
    public float atk;

    [Tooltip("異常抗性\n值為正負無上限，50表示異常抗性50%\n異常抗性50%表示若受到一個80%機率成功的狀態附加，則會變成只有30%機率成功附加")]
    public float stateResistance;

    [Tooltip("閃避率\n值為正負無上限，50表示閃避率50%\n閃避率50%表示若受到一個80%命中率的攻擊，則會變成只有30%機率成功命中")]
    public float evasion;

    //回合開始時觸發
    public virtual void OnTurnStart() { }

    //回合結束時觸發
    public virtual void OnTurnOver() { }
}

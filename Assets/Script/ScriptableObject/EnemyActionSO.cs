using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyActionSO : ScriptableObject
{
    [Header("===== 固定行動設定 =====")]

    [Tooltip("固定行動回合執行固定行動")]
    public List<FixedAction> FixedActionList;

    [Serializable]
    public class FixedAction
    {
        [Tooltip("指定回合")]
        public int turn;

        [Tooltip("行動")]
        public List<AttackSO> action;
    }

    [Header("===== 隨機行動設定 =====")]

    [Tooltip("非固定行動回合以外的回合\n" +
        "會從所有的隨機行動之中隨機抽取一個做為當回合的行動\n" +
        "已執行過的隨機行動會於此輪移除，確保所有的隨機行動都能輪到\n" +
        "所有的隨機行動都執行過一次後會重置新的一輪後再抽取")]
    public List<RandomAction> RandomActionList;

    [Serializable]
    public class RandomAction
    {
        [Tooltip("行動")]
        public List<AttackSO> action;
    }

    [Header("===== 條件行動設定 =====")]

    [Tooltip("非固定行動回合以外的回合\n" +
        "在滿足特定條件下執行條件行動，優先級高於隨機行動")]
    public TriggerAction triggerAction;

    [Serializable]
    public class TriggerAction
    {
        [Tooltip("行動")]
        public List<AttackSO> action;
    }

    //檢查敵人效果
    public virtual bool OnCheckEnemyBuffs(bool use, IReadOnlyList<BuffModel> buffs) { return use; }

    //檢查玩家效果
    public virtual bool OnCheckPlayerBuffs(bool use, IReadOnlyList<BuffModel> buffs) { return use; }

    //檢查敵人狀態
    public virtual bool OnCheckEnemyStates(bool use, IReadOnlyList<StateModel> states) { return use; }

    //檢查玩家狀態
    public virtual bool OnCheckPlayerStates(bool use, IReadOnlyList<StateModel> states) { return use; }
}

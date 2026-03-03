using System.Collections.Generic;
using UnityEngine;
using static EnemyActionSO;

public class EnemyActionModel
{
    EnemyActionSO enemyActionSO; //資源
    //比對參考資源
    public bool CheckEnemyActionSO(EnemyActionSO enemyActionSO)
    {
        return this.enemyActionSO == enemyActionSO;
    }

    //固定行動
    public IReadOnlyList<FixedAction> ReturnFixedActionList()
    {
        return enemyActionSO.FixedActionList;
    }

    //隨機行動
    public IReadOnlyList<RandomAction> ReturnRandomActionList()
    {
        return enemyActionSO.RandomActionList;
    }

    //條件行動
    public TriggerAction ReturnTriggerAction()
    {
        return enemyActionSO.triggerAction;
    }

    //當輪的剩餘隨機行動
    private List<RandomAction> nowRandomActionList;

    //根據提供的回合數回傳行動
    public IReadOnlyList<AttackModel> TurnReturnAction(int turn)
    {
        //確認提供的回合數是否有對應的固定行動
        for (int i = 0; i < ReturnFixedActionList().Count; i++)
        {
            //回傳對應的固定行動
            if (ReturnFixedActionList()[i].turn == turn)
            {
                return ConvertToAttackModels(ReturnFixedActionList()[i].action);
            }
        }

        //沒有對應的固定行動

        //確認是否有任何特定條件滿足，如有則強制執行條件行動
        bool use = false;
        use = enemyActionSO.OnCheckEnemyBuffs(use, FightManager.Instance.enemy.buffManager.GetBuffs());
        use = enemyActionSO.OnCheckPlayerBuffs(use, FightManager.Instance.player.buffManager.GetBuffs());
        use = enemyActionSO.OnCheckEnemyStates(use, FightManager.Instance.enemy.stateManager.GetStates());
        use = enemyActionSO.OnCheckPlayerStates(use, FightManager.Instance.player.stateManager.GetStates());

        if (use == true)
        {
            return ConvertToAttackModels(ReturnTriggerAction().action);
        }

        // 若本輪隨機行動已全部用完則重置
        if (nowRandomActionList == null || nowRandomActionList.Count == 0)
        {
            nowRandomActionList = new List<RandomAction>(ReturnRandomActionList());
        }

        //隨機抽取一個隨機行動回傳
        int randomIndex = UnityEngine.Random.Range(0, nowRandomActionList.Count);
        RandomAction selectedAction = nowRandomActionList[randomIndex];

        //將此隨機行動從本輪池中移除，直到下次重置
        nowRandomActionList.RemoveAt(randomIndex);

        return ConvertToAttackModels(selectedAction.action);
    }

    private List<AttackModel> ConvertToAttackModels(List<AttackSO> attackSOs)
    {
        List<AttackModel> result = new();

        for (int i = 0; i < attackSOs.Count; i++)
        {
            result.Add(new AttackModel(attackSOs[i]));
        }

        return result;
    }

    //建構子
    public EnemyActionModel(EnemyActionSO so)
    {
        enemyActionSO = so;
    }
}
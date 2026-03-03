using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KaybrisSO", menuName = "Scriptable Objects/CharacterSO/KaybrisSO")]
public class KaybrisCharacterSO : CharacterSO
{
    [SerializeField] List<EnemyAndAction> enemyAndAction;
    [SerializeField] List<BuffSO> addBuffs; //更換時要立即添加上的效果

    [Serializable]
    public class EnemyAndAction
    {
        public CharacterSO newEnemyCharacter; //更換用的敵人角色資源
        public EnemyActionSO newEnemyAction; //更換用的敵人行動資源
    }

    //回合開始時觸發
    public override void OnTurnStart() 
    {
        //每5回合執行一次
        if (FightManager.Instance.nowTurn % 5 != 0)
        {
            return;
        }

        int index = UnityEngine.Random.Range(0, enemyAndAction.Count);
        //重設敵人的角色資源
        FightManager.Instance.enemy.ChangeCharacterSO(enemyAndAction[index].newEnemyCharacter);
        //重設敵人行動數據
        FightManager.Instance.ChangeEnemyAction(enemyAndAction[index].newEnemyAction);
        //添加上指定效果
        foreach (BuffSO buff in addBuffs)
        {
            FightManager.Instance.enemy.AddBuff(buff);
        }
        
    }
}

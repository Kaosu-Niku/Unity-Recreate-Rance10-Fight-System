using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WargSO", menuName = "Scriptable Objects/CharacterSO/WargSO")]
public class WargCharacterSO : CharacterSO
{
    [SerializeField] int changeTurn;
    [SerializeField] CharacterSO newEnemyCharacter; //更換用的敵人角色資源
    [SerializeField] EnemyActionSO newEnemyAction; //更換用的敵人行動資源
    [SerializeField] List<BuffSO> addBuffs; //更換時要立即添加上的效果

    //回合開始時觸發
    public override void OnTurnStart() 
    {
        //在指定的回合數執行
        if (FightManager.Instance.nowTurn == changeTurn)
        {
            //重設敵人的角色資源
            FightManager.Instance.enemy.ChangeCharacterSO(newEnemyCharacter);
            //重設敵人行動數據
            FightManager.Instance.ChangeEnemyAction(newEnemyAction);
            //添加上指定效果
            foreach (BuffSO buff in addBuffs)
            {
                FightManager.Instance.enemy.AddBuff(buff);
            }
        } 
    }
}

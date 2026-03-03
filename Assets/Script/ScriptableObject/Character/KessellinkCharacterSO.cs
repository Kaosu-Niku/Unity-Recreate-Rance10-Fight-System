using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KessellinkSO", menuName = "Scriptable Objects/CharacterSO/KessellinkSO")]
public class KessellinkCharacterSO : CharacterSO
{
    [SerializeField] BuffSO checkBuff;
    [SerializeField] CharacterSO newEnemyCharacter; //更換用的敵人角色資源
    [SerializeField] EnemyActionSO newEnemyAction; //更換用的敵人行動資源
    [SerializeField] List<BuffSO> addBuffs; //更換時要立即添加上的效果

    //回合結束時觸發
    public override void OnTurnOver() 
    {
        bool haveBuff = false;
        foreach (BuffModel buff in FightManager.Instance.enemy.buffManager.GetBuffs())
        {
            //檢查敵人效果中是否持有特定效果
            if(buff.CheckBuffSO(checkBuff) == true)
            {
                haveBuff = true;
            }
        }

        //未持有特定效果
        if (haveBuff == false)
        {
            //重設敵人的角色資源
            FightManager.Instance.enemy.ChangeCharacterSO(newEnemyCharacter);
            //重設敵人行動數據
            FightManager.Instance.ChangeEnemyAction(newEnemyAction);
            //添加上指定效果
            foreach(BuffSO buff in addBuffs)
            {
                FightManager.Instance.enemy.AddBuff(buff);
            }
        }
    }
}

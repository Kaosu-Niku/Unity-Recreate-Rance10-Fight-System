using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaialSO", menuName = "Scriptable Objects/EnemyActionSO/PaialSO")]
public class PaialEnemyActionSO : EnemyActionSO
{
    //檢查敵人效果
    public override bool OnCheckEnemyBuffs(bool use, IReadOnlyList<BuffModel> buffs) 
    {
        //敵人的Buff數量低於3就滿足條件
        if (buffs.Count < 3)
        {
            return true;
        }

        return use; 
    }
}

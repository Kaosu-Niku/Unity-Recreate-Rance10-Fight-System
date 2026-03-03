using UnityEngine;

[CreateAssetMenu(fileName = "NotHealStateSO", menuName = "Scriptable Objects/State/NotHealStateSO")]
public class NotHealStateSO : StateSO
{
    //增加當前血量時修正
    public override float OnAddNowHp(float hp) { 
        //不能增加當前血量
        return 0; 
    }
}

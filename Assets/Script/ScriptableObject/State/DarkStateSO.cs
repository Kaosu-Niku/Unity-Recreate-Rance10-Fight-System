using UnityEngine;

[CreateAssetMenu(fileName = "DarkStateSO", menuName = "Scriptable Objects/State/DarkStateSO")]
public class DarkStateSO : StateSO
{
    //攻擊時命中修正
    public override float OnAttackAccuracy(float accuracy) 
    { 
        return accuracy - 25; 
    }
}

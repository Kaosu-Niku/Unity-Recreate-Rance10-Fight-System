using UnityEngine;

[CreateAssetMenu(fileName = "FearStateSO", menuName = "Scriptable Objects/State/FearStateSO")]
public class FearStateSO : StateSO
{
    //回合開始回復AP量修正
    public override int OnAddAP(int ap) {
        //持有此狀態使AP回復量-1 (增加AP的時候才會-1，如果是減少AP就維持原本的減少量)
        if(ap > 0)
        {
            return ap - 1;
        }
        return ap;
    }
}

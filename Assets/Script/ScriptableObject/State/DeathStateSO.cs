using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "DeathStateSO", menuName = "Scriptable Objects/State/DeathStateSO")]
public class DeathStateSO : StateSO
{
    //回合開始時調用
    public override bool OnTurnStart(CharacterModel character, int stateTurn)
    {
        //檢查此狀態的剩餘回合數，如已經是最後一回合，則減少當前生命比例100%的血量，此傷害為直接減少，不經過戰鬥邏輯判斷
        if (stateTurn <= 1)
        {
            character.ReduceNowHp(character.nowHp + 1);
            FightManager.Instance.PlayAudioForName("StateCurse");
            return true;
        }

        return false;
    } 
}

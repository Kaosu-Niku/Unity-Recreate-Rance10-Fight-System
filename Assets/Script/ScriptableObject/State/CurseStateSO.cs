using UnityEngine;

[CreateAssetMenu(fileName = "CurseStateSO", menuName = "Scriptable Objects/State/CurseStateSO")]
public class CurseStateSO : StateSO
{
    //回合結束時調用
    public override bool OnTurnOver(CharacterModel character, int stateTurn)
    {
        //檢查此狀態的剩餘回合數，如已經是最後一回合，則減少當前生命比例20%的血量，此傷害為直接減少，不經過戰鬥邏輯判斷
        if(stateTurn <= 1)
        {
            float damage = character.nowHp * 0.2f;
            character.ReduceNowHp(damage);
            FightManager.Instance.PlayAudioForName("StateCurse");
            return true;
        }

        return false;
    }
}

using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "PoisonStateSO", menuName = "Scriptable Objects/State/PoisonStateSO")]
public class PoisonStateSO : StateSO
{
    //回合開始時調用
    public override bool OnTurnStart(CharacterModel character, int stateTurn)
    {
        //減少當前生命比例5%的血量，此傷害為直接減少，不經過戰鬥邏輯判斷
        float damage = character.nowHp * 0.05f;
        character.ReduceNowHp(damage);
        FightManager.Instance.PlayAudioForName("StatePoison");

        return true;
    }
}

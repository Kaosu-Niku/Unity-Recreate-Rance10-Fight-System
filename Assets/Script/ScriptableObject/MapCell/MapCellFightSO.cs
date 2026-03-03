using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapCellFightSO", menuName = "Scriptable Objects/MapCell/MapCellFightSO")]
public class MapCellFightSO : MapCellSO
{
    public CharacterSO characterSO; //該場戰鬥的敵人資源
    public EnemyActionSO enemyActionSO; //該場戰鬥的敵人行動序列資源
    public List<BuffSO> enemyBuffSOs; //該場戰鬥初始自動為敵人附加的效果資源
    public List<StateSO> enemyStateSOs; //該場戰鬥初始自動為敵人附加的狀態資源
    public bool haveOver = false; //該場戰鬥是否有回合數限制
    public int overTurn; //該場戰鬥的限制回合數
    public Sprite background; //該場戰鬥的背景圖片
    public AudioClip bgm; //該場戰鬥的BGM
}

using UnityEngine;

public abstract class MapCellSO : ScriptableObject
{
    public string displayName; //名稱
    public Sprite image; //圖片
}

//該地圖格是屬於什麼類型
//public enum MapCellType
//{
//    Nothing, //什麼都沒有
//    Fight, //戰鬥
//    Story, //劇情
//    Award, //獎勵
//    Exp, //經驗
//    Eat, //食券
//    Trap, //陷阱       
//}

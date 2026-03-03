using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int version = 1;

    public List<int> FightCardPoolIndex = new(); //所有出戰卡片對應的卡片池Index

    public List<int> BeatEnemyID = new(); //所有擊敗過的敵人的ID
}

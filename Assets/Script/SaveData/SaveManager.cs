using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    //單例模式
    public static SaveManager Instance { get; private set; }

    public SaveData saveData { get; private set; } //玩家的存檔內容


    void Awake()
    {
        //單例模式建構
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //讀取存檔內容
        saveData = SaveSystem.Load();
    }

    //存檔內容即時修改 (修改後立即存檔)

    //擊敗BOSS時調用
    public void SetBeatEnemyID(int id)
    {
        if(saveData.BeatEnemyID.Contains(id) == false)
        {
            saveData.BeatEnemyID.Add(id);
        }

        SaveSystem.Save(saveData);
    }

    //重設出戰卡片時調用
    public void ResetFightCardPoolIndex(List<int> ids)
    {
        saveData.FightCardPoolIndex = ids;

        SaveSystem.Save(saveData);
    }
}

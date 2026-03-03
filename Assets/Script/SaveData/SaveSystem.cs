using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    //C:\Users\<你的使用者名稱>\AppData\LocalLow\<公司名稱>\<遊戲名稱>\
    static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    //將遊戲數據內容儲存成實體檔案 (c# class 可直接轉換成JSON格式)
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    //讀取實體檔案獲取遊戲數據內容 (JSON格式可直接轉換成 c# class)
    public static SaveData Load()
    {
        //讀取不到實體檔案的情況下默認生成一個全新的空白遊戲數據內容
        if (!File.Exists(SavePath))
        {
            return CreateNewSaveData();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    //生成一個全新的空白遊戲數據內容
    static SaveData CreateNewSaveData()
    {
        return new SaveData
        {
            version = 1,
            FightCardPoolIndex = new List<int>() { 0 },
            BeatEnemyID = new List<int>(),            
        };
    }
}

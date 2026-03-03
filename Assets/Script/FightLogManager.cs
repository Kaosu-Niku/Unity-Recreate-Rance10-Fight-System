using System;
using Unity.VisualScripting;
using UnityEngine;

public class FightLogManager : MonoBehaviour
{
    //單例模式
    public static FightLogManager Instance { get; private set; }

    public event Action<string> OnFightLog; //戰鬥紀錄事件

    void Awake()
    {
        //單例模式建構
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void WriteFightLog(string message)
    {
        OnFightLog?.Invoke(message);
    }
}

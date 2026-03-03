using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPoolSO", menuName = "Scriptable Objects/CardPoolSO")]
public class CardPoolSO : ScriptableObject
{
    [Header("務必確保此卡池資產在遊戲正式發售後不隨意更動卡池中的卡片排列順序\n" +
        "目前在遊戲中會透過指定number的方式向卡池索取指定的卡片資產\n" +
        "而目前卡片的number直接對應List的索引數\n第1張卡片的number就是0，第5張卡片的number就是4...以此類推")]

    [Tooltip("指定出戰的卡片(開發測試用)")]
    public List<CardSO> fightCardPool;

    [Tooltip("存放所有卡片的卡池")]
    public List<CardSO> cardPool;    
}

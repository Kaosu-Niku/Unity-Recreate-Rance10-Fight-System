using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    //單例模式
    public static CardPoolManager Instance { get; private set; }

    [SerializeField] CardPoolSO cardPoolSO; //卡池資源

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

    public int GetPoolCount()
    {
        return cardPoolSO.cardPool.Count;
    }

    public CardModel GetCard(int number)
    {
        if (number < 0 || number >= cardPoolSO.cardPool.Count)
            return null;

        var so = cardPoolSO.cardPool[number];
        if (so == null) return null;

        return new CardModel(so);
    }

    public int GetIndex(CardSO so)
    {
        if (so == null)
            return 0;

        return cardPoolSO.cardPool.FindIndex(x => x == so);
    }
}

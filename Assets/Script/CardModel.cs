using UnityEngine;

public class CardModel
{
    CardSO cardSO; //資源

    public CardSO GetCardSO => cardSO; //只讀

    //用於移除指定卡片時比對參考資源
    public bool CheckCardSO(CardSO cardSO)
    {
        return this.cardSO == cardSO;
    }

    public string displayName { get; private set; } //名稱

    public Sprite image { get; private set; } //圖片

    public CardElement element { get; private set; } //屬性分類

    public Camp camp { get; private set; } //陣營分類  

    public int level { get; private set; } //等級

    public float hp { get; private set; } //血量

    public float atk { get; private set; } //攻擊力

    public bool canFight { get; private set; } //是否能出戰
    public void SetCanFight(bool b)
    {
        canFight = b;
    }

    public AttackModel attack1 { get; private set; } //一技能

    public AttackModel attack2 { get; private set; } //二技能    

    //建構子
    public CardModel(CardSO so)
    {
        cardSO = so;
        displayName = so.displayName;
        image = so.image;
        element = so.element;
        camp = so.camp;
        level = so.level;
        hp = so.hp;
        atk = so.atk;
        canFight = true;
        if (so.attack1 != null)
        {
            attack1 = new AttackModel(so.attack1);
        }
        if (so.attack2 != null)
        {
            attack2 = new AttackModel(so.attack2);
        }   
    }
}
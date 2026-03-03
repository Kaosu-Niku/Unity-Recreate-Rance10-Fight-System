using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Card : MonoBehaviour
{
    [SerializeField] int CardIndex; //此卡片UI屬於第幾順位

    [SerializeField] RectTransform mask; //遮罩層
    [SerializeField] Image image; //圖片層 

    [SerializeField] Image elementImage; //屬性圖片 
    [SerializeField] Text elementText; //屬性文字

    CardModel myCard; //屬於此UI的卡片資料

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);

        PlayerManager.Instance.OnFightCardsChange += UpdateUI_FightCards;
        UpdateUI_FightCards(PlayerManager.Instance.GetFightCards());
    }

    void OnDisable()
    {
        if(PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnFightCardsChange -= UpdateUI_FightCards;
        }
    }

    void UpdateUI_FightCards(IReadOnlyList<CardModel> cards)
    {
        //玩家出戰的卡片改變時觸發

        //根據玩家出戰的卡片顯示卡片圖示、屬性圖示、屬性文字，並自適應化卡片圖示的圖片比例
        if(cards.Count - 1 >= CardIndex)
        {
            if (cards[CardIndex] != null)
            {
                myCard = cards[CardIndex];

                image.gameObject.SetActive(true);
                image.sprite = myCard.image;
                SetSprite(mask, image);
                SetOffsetX(image, 0);

                //暫無屬性圖示，改用相應的純色代替表示
                elementImage.gameObject.SetActive(true);
                switch (myCard.element)
                {
                    case CardElement.Nothing:
                        elementImage.color = new Color32(255, 255, 255, 255);
                        elementText.text = "無";
                        break;
                    case CardElement.Fire:
                        elementImage.color = new Color32(255, 0, 0, 255);
                        elementText.text = "炎";
                        break;
                    case CardElement.Ice:
                        elementImage.color = new Color32(0, 255, 255, 255);
                        elementText.text = "冰";
                        break;
                    case CardElement.Thunder:
                        elementImage.color = new Color32(255, 255, 0, 255);
                        elementText.text = "雷";
                        break;
                    case CardElement.Light:
                        elementImage.color = new Color32(0, 255, 0, 255);
                        elementText.text = "光";
                        break;
                    case CardElement.Dark:
                        elementImage.color = new Color32(125, 0, 255, 255);
                        elementText.text = "闇";
                        break;
                }

                return;
            }
        }

        image.gameObject.SetActive(false);
        elementImage.gameObject.SetActive(false);
    }

    //設置圖片等比縮放，讓圖片高度 = Mask高度
    public void SetSprite(RectTransform mask, Image image)
    {
        if (image == null || image.sprite == null || mask == null)
            return;

        Sprite getSprite = image.sprite;
        float maskHeight = mask.rect.height;
        float spriteHeight = getSprite.rect.height;
        float spriteWidth = getSprite.rect.width;

        float scale = maskHeight / spriteHeight;

        float newWidth = spriteWidth * scale;
        float newHeight = spriteHeight * scale;

        image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }

    // 用這個來調 X 軸（例如 -50, 0, 100）
    public void SetOffsetX(Image image, float x)
    {
        Vector2 pos = image.rectTransform.anchoredPosition;
        pos.x = x;
        image.rectTransform.anchoredPosition = pos;
    }

    //將此卡片移出出戰卡片
    public void OnClickPullFightCard()
    {
        if (myCard != null)
        {
            PlayerManager.Instance.PullFightCard(myCard);
        }
    }
}

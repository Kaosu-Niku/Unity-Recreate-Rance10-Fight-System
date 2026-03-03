using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_TeamCard : MonoBehaviour
{
    [SerializeField] RectTransform mask; //遮罩層
    [SerializeField] Image image; //圖片層 
    [SerializeField] Image elementImage; //屬性圖片 
    [SerializeField] Text elementText; //屬性文字
    [SerializeField] Text nameText; //名稱文字
    [SerializeField] Image canFightImage; //是否出戰提示圖片

    CardModel myCard; //屬於此UI的卡片資料

    public void UpdateUI_TeamCards(CardModel card)
    {
        //外部手動調用觸發

        //根據參數指定的卡片顯示卡片圖示、屬性圖示、屬性文字、名稱文字、是否出戰提示圖片，並自適應化卡片圖示的圖片比例
        if (card != null)
        {
            myCard = card;

            image.gameObject.SetActive(true);
            image.sprite = myCard.image;
            SetSprite(mask, image);
            SetOffsetX(image, 0);

            //暫無屬性圖示，改用相應的純色代替表示
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

            nameText.text = myCard.displayName;

            canFightImage.gameObject.SetActive(!card.canFight);
        }
        else
        {
            image.gameObject.SetActive(false);
        }

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

    //將此卡片放入出戰卡片
    public void OnClickPutFightCard()
    {
        if(myCard != null)
        {
            PlayerManager.Instance.PutFightCard(myCard);
        }
    }
}

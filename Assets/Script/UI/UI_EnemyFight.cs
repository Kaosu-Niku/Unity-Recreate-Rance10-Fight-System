using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyFight : MonoBehaviour
{
    [SerializeField] MapCellFightSO mapCellFightSO; //戰鬥地圖格資源
    [SerializeField] RectTransform mask; //遮罩層
    [SerializeField] Image image; //圖片層 
    [SerializeField] Text nameText; //名稱文字
    [SerializeField] Text beatText; //擊敗文字
    [SerializeField] int enemyID; //敵人ID

    void OnEnable()
    {
        UpdateUI_EnemyFight();
    }

    void UpdateUI_EnemyFight()
    {
        if (mapCellFightSO == null)
            return;

        //根據戰鬥地圖格資源顯示敵人圖示、敵人名稱，並自適應化敵人圖示的圖片比例

        if (mapCellFightSO.image != null)
        {            
            image.sprite = mapCellFightSO.image;
            SetSprite(mask, image);
            SetOffsetX(image, 0);
        }

        if(mapCellFightSO.characterSO != null)
        {
            nameText.text = mapCellFightSO.characterSO.displayName;
        }

        //確認存檔內容來判斷顯示擊敗文字
        beatText.gameObject.SetActive(false);
        if (SaveManager.Instance != null && SaveManager.Instance.saveData != null)
        {
            beatText.gameObject.SetActive(SaveManager.Instance.saveData.BeatEnemyID.Contains(enemyID));
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

    //開啟戰鬥UI並設置該敵人的地圖資源並正式開始一場新戰鬥
    public void OnFightYes()
    {
        if (FightManager.Instance == null)
            return;

        FightManager.Instance.ResetNewFight(mapCellFightSO);
    }
}

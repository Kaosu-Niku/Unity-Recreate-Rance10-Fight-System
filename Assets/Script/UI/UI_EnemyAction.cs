using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyAction : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] Image attackImage;
    [SerializeField] Animator animator;

    AttackModel attack; //屬於此UI的資料

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnEnemyNowActionChange += UpdateUI_EnemyAction;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnEnemyNowActionChange -= UpdateUI_EnemyAction;
        }
    }

    void UpdateUI_EnemyAction(IReadOnlyList<AttackModel> enemyAttack)
    {
        //敵人當前行動改變時觸發
        
        //根據敵人當前行動顯示攻擊圖示
        if (index < 0 || index >= enemyAttack.Count)
        {
            attackImage.gameObject.SetActive(false);
            return;
        }

        //退訂舊攻擊
        if (attack != null)
        {
            attack.OnUpdateAttack -= UpdateAttack;
            attack.OnUseAttack -= UseAttack;
        }

        //更新為新攻擊
        attack = enemyAttack[index];

        //訂閱新攻擊、更新UI
        if (attack != null)
        {
            attack.OnUpdateAttack += UpdateAttack;
            attack.OnUseAttack += UseAttack;
            attackImage.gameObject.SetActive(true);
            attackImage.sprite = attack.image;
        }
        else
        {
            attackImage.gameObject.SetActive(false);
        }
    }

    //更新攻擊時播放動畫
    void UpdateAttack()
    {
        animator.Play("Enemy_Action_In", 0, 0);
    }

    //攻擊執行時播放動畫
    void UseAttack()
    {
        animator.Play("Enemy_Action_Trigger", 0, 0);
    }

    //開啟敵人行動資訊面板
    public void OnOpenHint()
    {
        if (attack != null)
        {
            //由於資訊面板和此UI都是屬於局部座標且不屬於同個父物件，因此在設定座標時要經過以下方式轉換取得

            //此UI的RectTransform
            RectTransform buttonRect = GetComponent<RectTransform>();

            //資訊面板的父物件的RectTransform
            RectTransform hintParentRect = UI_HintPanel.Instance.getEnemyAttackHintPanel.transform.parent.GetComponent<RectTransform>();

            //此UI的世界座標
            Vector3 buttonWorldPos = buttonRect.position;

            //此UI的世界座標轉換成資訊面板的父物件的局部座標
            Vector3 localPos = hintParentRect.InverseTransformPoint(buttonWorldPos);

            //自定局部座標偏移
            localPos.y += 120f;

            //資訊面板的新局部座標
            RectTransform hintRect = UI_HintPanel.Instance.getEnemyAttackHintPanel.GetComponent<RectTransform>();
            hintRect.localPosition = localPos;

            UI_HintPanel.Instance.Open_EnemyAttackHint(attack.displayName, attack.depiction);
        }     
    }
}

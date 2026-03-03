using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Buff_Player : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] GameObject buffBack;
    [SerializeField] Image buffImage;
    [SerializeField] Text buffTurnText;
    [SerializeField] Text buffNameText;
    [SerializeField] GameObject buffLock;
    [SerializeField] Animator animator;

    CharacterModel currentPlayer;
    BuffModel myBuff; //屬於此UI的效果資料

    void Start()
    {
        buffBack.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null);

        FightManager.Instance.OnPlayerChange += ResetPlayer;
    }

    void OnDisable()
    {
        if(FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayerChange -= ResetPlayer;
        }
    }

    void ResetPlayer(CharacterModel player)
    {
        if (currentPlayer != null)
        {
            currentPlayer.buffManager.OnBuffsChange -= UpdateUI_PlayerBuffs;
        }

        currentPlayer = player;

        currentPlayer.buffManager.OnBuffsChange += UpdateUI_PlayerBuffs;
        UpdateUI_PlayerBuffs(currentPlayer.buffManager.GetBuffs());
    }

    void UpdateUI_PlayerBuffs(IReadOnlyList<BuffModel> playerBuffs)
    {
        //我方當前效果改變時觸發
        if (index < 0 || index >= playerBuffs.Count)
        {
            BindBuff(null);
        }
        else
        {
            BindBuff(playerBuffs[index]);
        }        
    }

    void BindBuff(BuffModel newBuff)
    {
        //新舊效果一致
        if (myBuff == newBuff)
        {
            if (myBuff != null)
            {
                if (myBuff.haveTurn)
                {
                    buffTurnText.text = myBuff.nowTurn.ToString();
                }
                else
                {
                    buffTurnText.text = "";
                }
            }
            return;
        }

        //退訂舊效果
        if (myBuff != null)
        {
            myBuff.OnBuffTrigger -= BuffTrigger;
        }

        //更新為新效果
        myBuff = newBuff;

        //訂閱新效果、更新UI、播放動畫
        if (myBuff != null)
        {
            myBuff.OnBuffTrigger += BuffTrigger;
            buffBack.gameObject.SetActive(true);
            buffImage.sprite = myBuff.image;
            buffNameText.text = myBuff.displayName.ToString();
            if (myBuff.haveTurn)
            {
                buffTurnText.text = myBuff.nowTurn.ToString();
            }
            else
            {
                buffTurnText.text = "";
            }
            if (myBuff.canDelete)
            {
                buffLock.gameObject.SetActive(false);
            }
            else
            {
                buffLock.gameObject.SetActive(true);
            }
            animator.Play("Buff_In_Player", 0, 0);
        }
        else
        {
            animator.Play("Buff_Out_Player", 0, 0);
        }
    }

    //效果觸發時播放動畫
    void BuffTrigger()
    {
        animator.Play("Buff_Trigger_Player", 0, 0);
    }

    //開啟效果資訊面板
    public void OnOpenHint()
    {
        if (myBuff != null)
        {
            //由於資訊面板和此UI都是屬於局部座標且不屬於同個父物件，因此在設定座標時要經過以下方式轉換取得

            //此UI的RectTransform
            RectTransform buttonRect = GetComponent<RectTransform>();

            //資訊面板的父物件的RectTransform
            RectTransform hintParentRect = UI_HintPanel.Instance.getBuffHintPanel.transform.parent.GetComponent<RectTransform>();

            //此UI的世界座標
            Vector3 buttonWorldPos = buttonRect.position;

            //此UI的世界座標轉換成資訊面板的父物件的局部座標
            Vector3 localPos = hintParentRect.InverseTransformPoint(buttonWorldPos);

            //自定局部座標偏移
            localPos.x -= 350f;

            //資訊面板的新局部座標
            RectTransform hintRect = UI_HintPanel.Instance.getBuffHintPanel.GetComponent<RectTransform>();
            hintRect.localPosition = localPos;

            UI_HintPanel.Instance.Open_BuffHint(myBuff.displayName, myBuff.depiction);
        }
    }
}

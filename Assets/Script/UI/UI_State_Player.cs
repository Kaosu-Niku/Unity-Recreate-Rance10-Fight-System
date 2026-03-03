using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_State_Player : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] Image stateImage;
    [SerializeField] Text stateTurnText;
    [SerializeField] Animator animator;

    CharacterModel currentPlayer;
    StateModel myState; //屬於此UI的狀態資料

    void Start()
    {
        stateImage.gameObject.SetActive(false);
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
            currentPlayer.stateManager.OnStatesChange -= UpdateUI_PlayerStates;
        }

        currentPlayer = player;

        currentPlayer.stateManager.OnStatesChange += UpdateUI_PlayerStates;
        UpdateUI_PlayerStates(currentPlayer.stateManager.GetStates());
    }

    void UpdateUI_PlayerStates(IReadOnlyList<StateModel> playerStates)
    {
        //我方當前狀態改變時觸發
        if (index < 0 || index >= playerStates.Count)
        {
            BindState(null);
        }
        else
        {
            BindState(playerStates[index]);          
        }
    }

    void BindState(StateModel newState)
    {
        //新舊狀態一致
        if (myState == newState)
        {
            if (myState != null)
            {
                stateImage.sprite = myState.image;
                stateTurnText.text = myState.nowTurn.ToString();
            }
            return;
        }

        //退訂舊狀態
        if (myState != null)
        {
            myState.OnStateTrigger -= StateTrigger;
        }

        //更新為新狀態
        myState = newState;

        //訂閱新狀態、更新UI、播放動畫
        if (myState != null)
        {
            myState.OnStateTrigger += StateTrigger;
            stateImage.gameObject.SetActive(true);
            stateImage.sprite = myState.image;
            stateTurnText.text = myState.nowTurn.ToString();
            animator.Play("State_In", 0, 0);
        }
        else
        {
            animator.Play("State_Out", 0, 0);
        }
    }

    //狀態觸發時播放動畫
    void StateTrigger()
    {
        animator.Play("State_Trigger", 0, 0);
    }

    //開啟狀態資訊面板
    public void OnOpenHint()
    {
        if (myState != null)
        {
            //由於資訊面板和此UI都是屬於局部座標且不屬於同個父物件，因此在設定座標時要經過以下方式轉換取得

            //此UI的RectTransform
            RectTransform buttonRect = GetComponent<RectTransform>();

            //資訊面板的父物件的RectTransform
            RectTransform hintParentRect = UI_HintPanel.Instance.getStateHintPanel.transform.parent.GetComponent<RectTransform>();

            //此UI的世界座標
            Vector3 buttonWorldPos = buttonRect.position;

            //此UI的世界座標轉換成資訊面板的父物件的局部座標
            Vector3 localPos = hintParentRect.InverseTransformPoint(buttonWorldPos);

            //自定局部座標偏移
            localPos.y += 100f;

            //資訊面板的新局部座標
            RectTransform hintRect = UI_HintPanel.Instance.getStateHintPanel.GetComponent<RectTransform>();
            hintRect.localPosition = localPos;

            UI_HintPanel.Instance.Open_StateHint(myState.displayName, myState.depiction);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HintPanel : MonoBehaviour
{
    //單例模式
    public static UI_HintPanel Instance { get; private set; }

    [SerializeField] GameObject hintPanel;
    public GameObject getHintPanel { get => hintPanel; }

    //敵人攻擊資訊面板
    [SerializeField] GameObject enemyAttackHintPanel;
    public GameObject getEnemyAttackHintPanel { get => enemyAttackHintPanel; }
    [SerializeField] Text enemyAttackHintNameText;
    [SerializeField] Text enemyAttackHintDepictionText;

    //效果資訊面板
    [SerializeField] GameObject buffHintPanel;
    public GameObject getBuffHintPanel { get => buffHintPanel; }
    [SerializeField] Text buffHintNameText;
    [SerializeField] Text buffHintDepictionText;

    //狀態資訊面板
    [SerializeField] GameObject stateHintPanel;
    public GameObject getStateHintPanel { get => stateHintPanel; }
    [SerializeField] Text stateHintNameText;
    [SerializeField] Text stateHintDepictionText;

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

    void Start()
    {
        ClosePanel();
    }

    public void ClosePanel()
    {
        hintPanel.SetActive(false);
        enemyAttackHintPanel.SetActive(false);
        buffHintPanel.SetActive(false);
        stateHintPanel.SetActive(false);
    }

    public void Open_EnemyAttackHint(string name, string depiction)
    {
        //顯示敵人攻擊資訊面板
        hintPanel.SetActive(true);
        enemyAttackHintPanel.SetActive(true);

        //修改相應的名稱和敘述文字
        enemyAttackHintNameText.text = name;
        enemyAttackHintDepictionText.text = depiction;
    }

    public void Open_BuffHint(string name, string depiction)
    {
        //顯示效果資訊面板
        hintPanel.SetActive(true);
        buffHintPanel.SetActive(true);

        //修改相應的名稱和敘述文字
        buffHintNameText.text = name;
        buffHintDepictionText.text = depiction;
    }

    public void Open_StateHint(string name, string depiction)
    {
        //顯示狀態資訊面板
        hintPanel.SetActive(true);
        stateHintPanel.SetActive(true);

        //修改相應的名稱和敘述文字
        stateHintNameText.text = name;
        stateHintDepictionText.text = depiction;
    }
}

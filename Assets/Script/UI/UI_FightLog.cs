using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_FightLog : MonoBehaviour
{
    [SerializeField] GameObject logTextTemplate; //Log文字UI模板

    [SerializeField] ScrollRect scrollRect; //ScrollRect的元件
    [SerializeField] RectTransform contentParent; // ScrollRect 的 Content

    // 物件池
    List<GameObject> cardPool = new List<GameObject>();
    int poolSize = 50;

    void Awake()
    {
        //物件池預先生成指定數量的Log文字
        for (int i = 0; i < poolSize; i++)
        {
            GameObject c = Instantiate(logTextTemplate, contentParent, false);
            c.SetActive(false);
            cardPool.Add(c);
        }
    }

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightLogManager.Instance != null);

        FightLogManager.Instance.OnFightLog += UpdateUI_LogText;
    }

    void OnDisable()
    {
        if(FightLogManager.Instance != null)
        {
            FightLogManager.Instance.OnFightLog -= UpdateUI_LogText;
        }
    }

    void UpdateUI_LogText(string log)
    {
        GameObject c;

        //從物件池獲取一個Log文字，如果物件池內的Log文字數量不足，則立刻再新增一個Log文字
        if (cardPool.Count > 0)
        {
            c = cardPool[0];
            cardPool.RemoveAt(0);
        }
        else
        {
            c = Instantiate(logTextTemplate, contentParent, false);
        }
        c.SetActive(true);
        c.transform.SetAsLastSibling();

        //獲取Log文字的指定腳本以設置指定內容
        Text t = c.GetComponent<Text>();
        if (t != null)
        {
            t.text = log;
        }

        //主動調用Layout計算使版面刷新
        Canvas.ForceUpdateCanvases();
    }

    public void ShowLog()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -55);
    }

    public void CloseLog()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -2000);
    }
}

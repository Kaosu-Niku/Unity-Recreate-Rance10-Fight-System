using System;
using UnityEngine;

public class CharacterModel
{
    CharacterSO characterSO; //資源

    public int id { get; private set; }//id
    public string displayName { get; private set; }//名稱

    public Sprite image { get; private set; } //圖片

    public float initHp { get; private set; } //初始血量    

    private float _nowHp; //當前血量
    public float nowHp
    {
        get => _nowHp;
        private set
        {
            if (_nowHp != value)
            {
                _nowHp = value;
                OnNowHpChange?.Invoke(_nowHp, initHp);
            }
        }
    }
    public event Action<float, float> OnNowHpChange; //當前血量改變事件

    public float initAtk { get; private set; } //初始攻擊力

    public float initStateResistance { get; private set; } //初始異常抗性

    public float initEvasion { get; private set; } //初始閃避率

    public int maxAp { get; private set; } //最大AP量

    private int _nowAp; //當前AP量
    public int nowAp
    {
        get => _nowAp;
        private set
        {
            if (_nowAp != value)
            {
                _nowAp = value;
                OnNowApChange?.Invoke(_nowAp);
            }
        }
    }
    public event Action<int> OnNowApChange; //當前AP量改變事件

    public BuffManager buffManager { get; private set; } //效果系統

    public StateManager stateManager { get; private set; } //狀態系統

    //建構子
    public CharacterModel(CharacterSO so)
    {
        characterSO = so;
        id = so.id;
        displayName = so.displayName;
        image = so.image;
        initHp = so.hp;        
        nowHp = initHp;
        initAtk = so.atk;
        initStateResistance = so.stateResistance;
        initEvasion = so.evasion;
        maxAp = 6;
        nowAp = 0;
        buffManager = new BuffManager(this);
        stateManager = new StateManager(this);
    }

    //重設初始血量
    public void SetInitHp(float hp)
    {
        initHp = hp;
        nowHp = hp;
    }

    //重設初始攻擊力
    public void SetInitAtk(float atk)
    {
        initAtk = atk;
    }

    //減少當前血量
    public void ReduceNowHp(float damage)
    {
        nowHp -= damage;
    }

    //增加當前血量
    public void AddNowHp(float hp)
    {
        float finalHp = stateManager.OnAddNowHp(hp);
        nowHp += hp;
    }

    //回合開始時觸發
    public void OnTurnStart()
    {
        AddAP(2);        
        buffManager.OnTurnStart();
        stateManager.OnTurnStart();
        characterSO.OnTurnStart();
    }

    //回合結束時觸發
    public void OnTurnOver()
    {
        characterSO.OnTurnOver();
        stateManager.OnTurnOver();
        characterSO.OnTurnOver();
    }

    //睡眠狀態確認
    public bool OnSleep()
    {
        bool sleep = stateManager.OnSleep();
        return sleep;
    }

    //修改當前AP量時修正
    public void AddAP(int ap)
    {
        int finalAP = stateManager.OnAddAP(ap);
        //當前AP量不得超過最大AP量，也不得低於0
        nowAp = Mathf.Clamp(nowAp + finalAP, 0, maxAp);
    }

    //給自身添加一個指定的效果
    public void AddBuff(BuffSO buffSO)
    {
        buffManager.AddBuff(buffSO);
    }

    //給自身移除一個指定類型的效果
    public void DeleteBuff(BuffSO buffSO)
    {
        buffManager.DeleteBuff(buffSO);
    }

    //給自身隨機移除一個效果
    public void DeleteRandomBuff()
    {
        buffManager.DeleteRandomBuff();
    }

    //給自身添加一個指定的狀態
    public void AddState(StateSO stateSO)
    {
        stateManager.AddState(stateSO);
    }

    //給自身移除一個指定類型的狀態
    public void DeleteState(StateSO stateSO)
    {
        stateManager.DeleteState(stateSO);
    }

    //給自身隨機移除一個狀態
    public void DeleteRandomState()
    {
        stateManager.DeleteRandomState();
    }    

    //重設角色資源 (此方法用於敵人進行更換)
    public void ChangeCharacterSO(CharacterSO newSO)
    {
        characterSO = newSO;
        id = newSO.id;
        displayName = newSO.displayName;
        image = newSO.image;        
        initHp = newSO.hp;
        initAtk = newSO.atk;
        initStateResistance = newSO.stateResistance;
        initEvasion = newSO.evasion;
        FightManager.Instance.OnEnemyChangeInvoke();
    }
}

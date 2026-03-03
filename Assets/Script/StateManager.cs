using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StateManager
{
    //之所以要建立此Manger，是出於架構考慮
    //狀態有許多種類，不適合直接與角色或戰鬥系統耦合
    //應該需要再另建一個Manger來集中管理狀態

    public CharacterModel character { get; private set; } //持有該狀態系統的角色

    List<StateModel> states; //當前持有的所有狀態
    public IReadOnlyList<StateModel> GetStates()
    {
        return states;
    }
    public event Action<IReadOnlyList<StateModel>> OnStatesChange; //當前狀態改變事件
    //(由於集合類型的內部元素變化並不會觸發屬性set，因此集合類型不適合用屬性set觸發event，只能手動觸發event)

    //建構子
    public StateManager(CharacterModel c)
    {
        character = c;
        states = new List<StateModel>();        
    }

    //給自身添加一個指定的狀態
    public void AddState(StateSO stateSO)
    {
        //不可重複添加同類型的狀態，如果已存在同類型的狀態，先將舊的移除，然後再重新添加新的
        for (int i = states.Count - 1; i >= 0; i--)
        {
            if (states[i].CheckStateSO(stateSO))
            {
                states.RemoveAt(i);
            }
        }

        StateModel newState = new StateModel(stateSO);
        states.Add(newState);
        OnStatesChange?.Invoke(GetStates());
        FightLogManager.Instance.WriteFightLog($"{character.displayName} 進入了 {newState.displayName} 狀態 !");
    }

    //給自身移除一個指定類型的狀態
    public void DeleteState(StateSO stateSO)
    {
        foreach (var state in states)
        {
            if (state.CheckStateSO(stateSO))
            {
                FightLogManager.Instance.WriteFightLog($"解除 {state.displayName} 狀態 !");
                states.Remove(state);
                OnStatesChange?.Invoke(GetStates());                
                break;
            }
        }
    }

    //給自身隨機移除一個狀態
    public void DeleteRandomState()
    {
        if (states.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, states.Count);
            StateModel target = states[index];
            FightLogManager.Instance.WriteFightLog($"解除 {target.displayName} 狀態 !");
            states.Remove(target);
        }
        OnStatesChange?.Invoke(GetStates());
        
    }

    //回合開始時觸發
    public void OnTurnStart()
    {       
        foreach (var state in states)
        {
            state.OnTurnStart(character);
        }     
    }

    //回合結束時觸發
    public void OnTurnOver()
    {      
        foreach (var state in states)
        {
            state.OnTurnOver(character);
            state.SetNowTurn(state.nowTurn - 1);
        }
        //所有狀態的剩餘回合數-1，並將剩餘回合數為0的狀態移除
        states.RemoveAll(s => s.nowTurn <= 0);
        OnStatesChange?.Invoke(GetStates());
    }

    //睡眠狀態確認
    public bool OnSleep()
    {
        bool sleep = false;
        foreach (var state in states)
        {
            sleep = state.OnSleep(sleep);
        }

        return sleep;
    }

    //修改當前AP量時修正
    public int OnAddAP(int ap)
    {
        int finalAp = ap;
        foreach (var state in states)
        {
            finalAp = state.OnAddAP(finalAp);
        }

        return finalAp;
    }

    //增加當前血量時修正
    public float OnAddNowHp(float hp)
    {
        float finalHp = hp;
        foreach (var state in states)
        {
            finalHp = state.OnAddNowHp(finalHp);
        }

        return finalHp;
    }

    //攻擊時命中修正
    public float OnAttackAccuracy(float accuracy)
    {
        float finalAccuracy = accuracy;
        foreach (var state in states)
        {
            finalAccuracy = state.OnAttackAccuracy(finalAccuracy);
        }

        return finalAccuracy;
    }

    //被攻擊時命中修正
    public float OnAttackedAccuracy(float accuracy)
    {
        float finalAccuracy = accuracy;
        foreach (var state in states)
        {
            finalAccuracy = state.OnAttackedAccuracy(finalAccuracy);
        }

        return finalAccuracy;
    }

    //攻擊時傷害修正
    public float OnAttackDamage(float damage)
    {
        float finalDamage = damage;
        foreach (var state in states)
        {
            finalDamage = state.OnAttackDamage(finalDamage);
        }

        return finalDamage;
    }

    //被攻擊時傷害修正
    public float OnAttackedDamage(AttackElement attackElement, CardElement? cardElement, float damage)
    {
        float finalDamage = damage;
        foreach (var state in states)
        {
            finalDamage = state.OnAttackedDamage(attackElement, cardElement, finalDamage);
        }

        return finalDamage;
    }

    //被攻擊時移除狀態
    public StateSO OnAttackedDeleteState(CharacterModel defenser)
    {
        StateSO finalState = null;
        foreach (var state in states)
        {
            finalState = state.OnAttackedDeleteState(finalState, defenser);
        }
        return finalState;
    }
}

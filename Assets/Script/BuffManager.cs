using System;
using System.Collections.Generic;

public class BuffManager
{
    //之所以要建立此Manger，是出於架構考慮
    //效果有許多種類，不適合直接與角色或戰鬥系統耦合
    //應該需要再另建一個Manger來集中管理效果

    public CharacterModel character { get; private set; } //持有該效果系統的角色

    List<BuffModel> buffs; //當前持有的所有效果
    public IReadOnlyList<BuffModel> GetBuffs()
    {
        return buffs;
    }
    public event Action<IReadOnlyList<BuffModel>> OnBuffsChange; //當前效果改變事件
    //(由於集合類型的內部元素變化並不會觸發屬性set，因此集合類型不適合用屬性set觸發event，只能手動觸發event)

    //建構子
    public BuffManager(CharacterModel c)
    {
        character = c;
        buffs = new List<BuffModel>();        
    }

    //給自身添加一個指定的效果
    public void AddBuff(BuffSO buffSO)
    {
        //添加效果時，先檢查是否是疊加效果，疊加效果的添加方式較為特殊
        if (buffSO.coverBuffs != null && buffSO.coverBuffs.Count > 0)
        {
            //是疊加效果

            //找出當前持有的疊加效果
            BuffSO findBuffSO = null;
            foreach (BuffSO cb in buffSO.coverBuffs)
            {
                foreach (BuffModel b in buffs)
                {
                    if (b.CheckBuffSO(cb))
                    {
                        findBuffSO = cb;
                    }
                }
            }

            if (findBuffSO != null)
            {
                //有找到，移除當前持有的疊加效果，並添加新一層的疊加效果
                int findIndex = buffSO.coverBuffs.IndexOf(findBuffSO);
                findIndex += 1;
                if(findIndex > buffSO.coverBuffs.Count -1)
                {
                    findIndex = buffSO.coverBuffs.Count - 1;
                }
                BuffSO addBuffSO = buffSO.coverBuffs[findIndex];
                BuffModel addBuff = new BuffModel(addBuffSO);                
                buffs.Add(addBuff);
                FightLogManager.Instance.WriteFightLog($"{addBuff.displayName} 效果出現了 !");
                DeleteBuff(findBuffSO);
            }
            else
            {
                //沒找到，添加第一層的疊加效果
                BuffModel addBuff = new BuffModel(buffSO.coverBuffs[0]);
                buffs.Add(addBuff);
                FightLogManager.Instance.WriteFightLog($"{addBuff.displayName} 效果出現了 !");
            }

            OnBuffsChange?.Invoke(GetBuffs());
            return;
        }

        //不是疊加效果

        //預設不可重複添加同類型的效果，如果已存在同類型的效果，需先將舊的移除，然後再添加新的
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].CheckBuffSO(buffSO))
            {
                //少部分效果可重複添加，不用將舊的移除
                if (buffSO.canRepeat == false)
                {
                    buffs.RemoveAt(i);
                }                
            }
        }

        BuffModel buff = new BuffModel(buffSO);
        buffs.Add(buff);
        FightLogManager.Instance.WriteFightLog($"{buff.displayName} 效果出現了 !");
        OnBuffsChange?.Invoke(GetBuffs());
    }

    //給自身移除一個指定的效果 (可以無視是否可正常移除的設定直接移除)
    public void DeleteBuff(BuffSO buffSO)
    {
        foreach (var buff in buffs)
        {
            if (buff.CheckBuffSO(buffSO))
            {
                FightLogManager.Instance.WriteFightLog($"{buff.displayName} 效果消除了 !");
                buffs.Remove(buff);
                OnBuffsChange?.Invoke(GetBuffs());                
                break;
            }
        }        
    }

    //給自身隨機移除一個效果 (必須是設定可正常移除的效果才能列入被隨機移除的對象)
    public void DeleteRandomBuff()
    {
        //先篩選出所有可被正常移除的效果後才開始進行隨機抽取
        List<BuffModel> canDeleteBuffs = buffs.FindAll(b => b.canDelete);  
        
        if (canDeleteBuffs.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, canDeleteBuffs.Count);
            BuffModel target = canDeleteBuffs[index];
            FightLogManager.Instance.WriteFightLog($"{target.displayName} 效果消除了 !");
            buffs.Remove(target);
            OnBuffsChange?.Invoke(GetBuffs());
            
        }        
    }

    //回合開始時觸發
    public void OnTurnStart()
    {
        //所有效果的剩餘回合數-1，將有回合限制且剩餘回合數為0的效果移除
        foreach (var buff in buffs)
        {
            buff.SetNowTurn(buff.nowTurn - 1);
        }
        buffs.RemoveAll(s => s.haveTurn == true && s.nowTurn <= 0);
        OnBuffsChange?.Invoke(GetBuffs());
    }

    //回合結束時觸發
    public void OnTurnOver()
    {
        
    }

    //被攻擊時是否無效化修正 (全攻擊)
    public bool OnAttackedInvalidOfAll() 
    {
        bool finalInvalid = false;
        foreach (var buff in buffs)
        {
            finalInvalid = buff.OnAttackedInvalidOfAll(finalInvalid);
            if (finalInvalid == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時是否無效化修正 (依攻擊類型)
    public bool OnAttackedInvalidOfAttackType(AttackType attackType)
    {
        bool finalInvalid = false;
        foreach (var buff in buffs)
        {
            finalInvalid = buff.OnAttackedInvalidOfAttackType(finalInvalid, attackType);
            if(finalInvalid == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時是否無效化修正 (依傷害類型)
    public bool OnAttackedInvalidOfDamageType(DamageType damageType)
    {
        bool finalInvalid = false;
        foreach (var buff in buffs)
        {
            finalInvalid = buff.OnAttackedInvalidOfDamageType(finalInvalid, damageType);
            if (finalInvalid == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時是否無效化修正 (依攻擊屬性)
    public bool OnAttackedInvalidOfAttackElement(AttackElement attackElement)
    {
        bool finalInvalid = false;
        foreach (var buff in buffs)
        {
            finalInvalid = buff.OnAttackedInvalidOfAttackElement(finalInvalid, attackElement);
            if (finalInvalid == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時是否無效化修正 (依卡片屬性)
    public bool OnAttackedInvalidOfCardElement(CardElement cardElement)
    {
        bool finalInvalid = false;
        foreach (var buff in buffs)
        {
            finalInvalid = buff.OnAttackedInvalidOfCardElement(finalInvalid, cardElement);
            if (finalInvalid == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時是否無效化修正 (依傷害量)
    public float OnAttackedInvalidOfDamage(float damage)
    {
        float finalDamage = damage;
        foreach (var buff in buffs)
        {
            finalDamage = buff.OnAttackedInvalidOfDamage(finalDamage);
        }
        return finalDamage;
    }

    //攻擊時命中率修正
    public float OnAttackAccuracy(float accuracy)
    {
        float finalAccuracy = accuracy;
        foreach (var buff in buffs)
        {
            finalAccuracy = buff.OnAttackAccuracy(finalAccuracy);
        }
        return finalAccuracy;
    }

    //被攻擊時命中率修正
    public float OnAttackedAccuracy(float accuracy)
    {
        float finalAccuracy = accuracy;
        foreach (var buff in buffs)
        {
            finalAccuracy = buff.OnAttackedAccuracy(finalAccuracy);
        }
        return finalAccuracy;
    }

    //攻擊時傷害修正
    public float OnAttackDamage(float damage, DamageType damageType)
    {
        float finalDamage = damage;
        foreach (var buff in buffs)
        {
            finalDamage = buff.OnAttackDamage(finalDamage, damageType);
        }
        return finalDamage;
    }

    //被攻擊時傷害修正
    public float OnAttackedDamage(float damage, DamageType damageType)
    {
        float finalDamage = damage;
        foreach (var buff in buffs)
        {
            finalDamage = buff.OnAttackedDamage(finalDamage, damageType);
        }
        return finalDamage;
    }

    //被攻擊時是否反擊修正
    public bool OnAttackedUseCounter(AttackType attackType)
    {
        bool finalUse = false;
        foreach (var buff in buffs)
        {
            finalUse = buff.OnAttackedUseCounter(finalUse, attackType);
            if (finalUse == true)
            {
                return true;
            }
        }
        return false;
    }

    //被攻擊時反擊傷害修正
    public float OnAttackedUseCounterDamage(float damage)
    {
        float finalDamage = damage;
        foreach (var buff in buffs)
        {
            finalDamage = buff.OnAttackedUseCounterDamage(finalDamage);
        }
        return finalDamage;
    }

    //被攻擊時移除效果
    public BuffSO OnAttackedDeleteBuff(CharacterModel defenser)
    {
        BuffSO finalBuff = null;
        foreach (var buff in buffs)
        {
            finalBuff = buff.OnAttackedDeleteBuff(finalBuff, defenser);
        }
        return finalBuff;
    }

    //自動攻擊的傷害來源 (我方是CardSO，敵方是CharacterSO)
    public CardSO AutoAttackDamageFromCard(BuffModel b)
    {
        CardSO finalCardSO = null;
        foreach (var buff in buffs)
        {
            if(buff == b)
            {
                finalCardSO = buff.AutoAttackDamageFromCard();
            }                
        }
        return finalCardSO;
    }
    public CharacterSO AutoAttackDamageFromCharacter(BuffModel b)
    {
        CharacterSO finalCharacterSO = null;
        foreach (var buff in buffs)
        {
            if (buff == b)
            {
                finalCharacterSO = buff.AutoAttackDamageFromCharacter();
            }
        }
        return finalCharacterSO;
    }

    //自動攻擊的攻擊參考
    public AttackSO AutoAttackFrom(BuffModel b)
    {
        AttackSO finalAttackSO = null;
        foreach (var buff in buffs)
        {
            if (buff == b)
            {
                finalAttackSO = buff.AutoAttackFrom();
            }
        }
        return finalAttackSO;
    }

    //自動攻擊的發動機率
    public float AutoAttackProbability(BuffModel b)
    {
        float finalAttackProbability = 0;
        foreach (var buff in buffs)
        {
            if (buff == b)
            {
                finalAttackProbability = buff.AutoAttackProbability();
            }
        }
        return finalAttackProbability;
    }
}

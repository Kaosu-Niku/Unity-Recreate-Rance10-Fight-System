using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //單例模式
    public static BattleManager Instance { get; private set; }

    private int _combo; //COMBO數
    public int combo
    {
        get => _combo;
        private set
        {
            if (_combo != value)
            {
                _combo = value;
                OnComboChange?.Invoke(_combo);
            }
        }
    }
    public event Action<int> OnComboChange; //COMBO數改變事件

    //回合開始時重置Combo數
    public void OnTurnStartResetCombo()
    {
        combo = 0;
    }

    public event Action OnAttackOver; //一次攻擊流程結束事件

    bool useHealAction = false;  //確認是否要觸發恢復效果
    bool useDeleteEnemyAction = false;  //確認是否要觸發移除敵人行動效果
    bool useDown = false; //確認是否要觸發DOWN效果    

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

    void OnEnable()
    {
        //使用協程確保等待對象實例化後才執行，防止因腳本執行順序不定，可能在等待對象還未實例化前就先執行而導致出錯
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitUntil(() => FightManager.Instance != null && FightManager.Instance.player != null && FightManager.Instance.enemy != null);
        FightManager.Instance.OnPlayerTurnStart += OnTurnStartResetCombo;
        FightManager.Instance.OnEnemyTurnStart += OnTurnStartResetCombo;
    }

    void OnDisable()
    {
        if (FightManager.Instance != null)
        {
            FightManager.Instance.OnPlayerTurnStart -= OnTurnStartResetCombo;
            FightManager.Instance.OnEnemyTurnStart -= OnTurnStartResetCombo;
        }
    }



    //一次攻擊的流程
    public IEnumerator UseAttack(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        //確認攻擊方的當前AP量是否足夠讓此次攻擊消耗
        if(attacker.nowAp >= attack.useAP)
        {
            attack.UseAttack();

            //無論攻擊結果如何都必須消耗掉AP
            attacker.AddAP(-attack.useAP);

            //根據後續流程來決定這些特殊邏輯是否會執行
            useHealAction = false;
            useDeleteEnemyAction = false;
            useDown = false;

            //!特殊邏輯! 恢復AP
            if (attack.recoverAP > 0)
            {
                attacker.AddAP(attack.recoverAP);
            }            

            if (card != null)
            {
                FightLogManager.Instance.WriteFightLog($"{card.displayName} 使用 {attack.displayName} ! 結果...");
            }
            else
            {
                FightLogManager.Instance.WriteFightLog($"{attacker.displayName} 使用 {attack.displayName} ! 結果...");
            }

            //!特殊邏輯! 蘭斯移除無敵結界效果
            //(因為遊戲中目前實作的具有全攻擊無效的特性的效果之中，無敵結界永久存在同時又只能依靠蘭斯的攻擊消除
            //但是蘭斯的攻擊都是帶有傷害的，而在攻擊流程中對於有傷害的攻擊都會優先判斷是否無效化，
            //因此按照正常流程走，蘭斯的攻擊根本無法消除無敵結界，因為蘭斯的攻擊在發動消除效果之前就先被無敵結界無效化而無法發動
            //因此為了解決此問題需另外添加處理邏輯)
            if (attack.ReturnRanceDeleteBuff() != null)
            {
                RanceDeleteBuff(attacker, defenser, card, attack);
            }           

            //1. 確認攻擊是否被無效化 (全攻擊)
            //攻擊被無效化，跳至1.1.並執行到結束流程
            //攻擊沒有被無效化，跳至2.
            if (AllAttackInvalid(attacker, defenser, card, attack) == false)
            {
                //2. 確認攻擊是否會造成傷害
                //攻擊不會造成傷害，跳至2.1.並依序執行到結束流程
                //攻擊會造成傷害，跳至3.
                if (attack.useAttack == true)
                {                    
                    //3. 確認攻擊是否被無效化 (依攻擊類型) (依傷害類型) (依攻擊屬性)
                    //攻擊被無效化，結束流程
                    //攻擊沒有被無效化，跳至4.
                    if (AttackInvalid(attacker, defenser, card, attack) == false)
                    {
                        //4. 確認攻擊是否可命中 (與命中率和閃避率相關)
                        //攻擊不可命中，結束流程
                        //攻擊可命中，跳至5.並依序執行到結束流程
                        if (AttackSuccessful(attacker, defenser, card, attack) == true)
                        {   
                            //攻擊的連擊次數
                            for(int i = 0; i < attack.attackCount; i++)
                            {
                                FightManager.Instance.PlayAudioForChip(attack.source);

                                //目前以攻擊Model所設定的時間做為流程演出等待時間
                                yield return new WaitForSeconds(attack.playTime);

                                //5. 對防守方造成傷害
                                DefenseGetDamage(attacker, defenser, card, attack);                               
                            }

                            //6. 對攻擊方附加效果 (概率附加)
                            AttackerAddBuffs(attacker, defenser, card, attack);
                            //7. 對攻擊方移除狀態 (必定移除)
                            AttackerDeleteStates(attacker, defenser, card, attack);
                            //8. 對防守方移除效果 (必定移除)
                            DefenserDeleteBuffs(attacker, defenser, card, attack);
                            //9. 對防守方附加狀態 (概率附加)
                            DefenserAddStates(attacker, defenser, card, attack);
                        }
                        else
                        {
                            combo = 0;
                            OnAttackMiss?.Invoke(defenser);
                        }
                    }
                    else
                    {
                        combo = 0;
                        OnAttackMiss?.Invoke(defenser);
                    }
                }
                else
                {
                    FightManager.Instance.PlayAudioForChip(attack.source);

                    //目前以攻擊Model所設定的時間做為流程演出等待時間
                    yield return new WaitForSeconds(attack.playTime);

                    if (attack.useAttack == false)
                    {
                        useHealAction = true;
                        useDown = true;
                    }                    

                    //2.1. 對攻擊方附加效果 (概率附加)
                    AttackerAddBuffs(attacker, defenser, card, attack);
                    //2.2. 對攻擊方移除狀態 (必定移除)
                    AttackerDeleteStates(attacker, defenser, card, attack);
                    //2.3. 對防守方移除效果 (必定移除)
                    DefenserDeleteBuffs(attacker, defenser, card, attack);
                    //2.4. 對防守方附加狀態 (概率附加)
                    DefenserAddStates(attacker, defenser, card, attack);
                }
            }
            else
            {
                FightManager.Instance.PlayAudioForChip(attack.source);

                //目前以攻擊Model所設定的時間做為流程演出等待時間
                yield return new WaitForSeconds(attack.playTime);

                if(attack.useAttack == false)
                {
                    useHealAction = true;
                    useDown = true;
                }               

                //1.1. 對攻擊方附加效果 (概率附加)
                AttackerAddBuffs(attacker, defenser, card, attack);
                //1.2. 對攻擊方移除狀態 (必定移除)
                AttackerDeleteStates(attacker, defenser, card, attack);
                //2.3. 對防守方移除效果 (必定移除)
                DefenserDeleteBuffs(attacker, defenser, card, attack);

                combo = 0;
                OnAttackMiss?.Invoke(defenser);
            }            

            yield return new WaitForSeconds(0.25f);

            //!特殊邏輯! 恢復
            if (useHealAction == true)
            {
                float heal = 0;
                //依攻擊力 * 倍率
                if(attack.healMultiplier > 0)
                {
                    float attackerAttack = attacker.initAtk; //攻擊方的攻擊力
                    //此處根據是否有提供卡片資料來判斷這個攻擊是否發動自卡片的技能
                    //如果是卡片的技能，則攻擊方的攻擊力應改寫為卡片的攻擊力
                    if (card != null)
                    {
                        attackerAttack = card.atk;
                    }
                    heal = attackerAttack * attack.healMultiplier;
                    attacker.AddNowHp(heal);
                    FightLogManager.Instance.WriteFightLog($"{attacker.displayName} 恢復了 {heal} HP !");
                }

                //依最大血量比例
                if(attack.healScale > 0)
                {
                    heal = attacker.initHp * (attack.healScale / 100);
                    attacker.AddNowHp(heal);
                    FightLogManager.Instance.WriteFightLog($"{attacker.displayName} 恢復了 {heal} HP !");
                }                                
            }

            //!特殊邏輯! 移除敵人行動 (我方攻擊敵方專用)
            if (useDeleteEnemyAction == true)
            {
                float r = UnityEngine.Random.Range(0, 100);
                if (r < attack.deleteEnemyActionProbability)
                {
                    FightManager.Instance.RandomDeleteEnemyAction();
                    FightLogManager.Instance.WriteFightLog($"{defenser.displayName} 的行動被阻止了 !");
                }
            }

            yield return null;

            //!特殊邏輯! 變身
            if (attack.OnAttackReturnHenshinCard() != null)
            {
                CardModel henshinCard = new CardModel(attack.OnAttackReturnHenshinCard());
                if (card != null && henshinCard != null)
                {
                    PlayerManager.Instance.PutFightCard(henshinCard);
                }
            }

            yield return null;                       

            //!特殊邏輯! DOWN
            if (useDown == true)
            {
                if (attack.OnAttackReturnDownCards().Count > 0)
                {
                    foreach (CardSO cardSO in attack.OnAttackReturnDownCards())
                    {
                        PlayerManager.Instance.Down(cardSO);
                    }
                }
            }

            yield return null;

            //通知一次攻擊流程結束
            OnAttackOver?.Invoke();
        }
    }

    //攻擊被無效事件
    public event Action<CharacterModel> OnAttackMiss;

    //確認攻擊是否被無效化 (全攻擊)
    bool AllAttackInvalid(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        //防守方的效果確認是否被無效化 (全攻擊)
        if (defenser.buffManager.OnAttackedInvalidOfAll() == true)
        {
            //防守方的效果觸發效果移除 (ex: 魔法屏障、忍法替身術)
            BuffSO dBuff = defenser.buffManager.OnAttackedDeleteBuff(defenser);
            if (dBuff != null)
            {
                FightLogManager.Instance.WriteFightLog($"{defenser.displayName} 透過 {dBuff.displayName} 阻擋了攻擊 !");
                defenser.DeleteBuff(dBuff);
                return true;
            }
            
            FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (魔人的無敵結界)");
            return true;
        }

        return false;
    }

    //確認攻擊是否被無效化 (依攻擊類型) (依傷害類型) (依攻擊屬性)
    bool AttackInvalid(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        //防守方的效果確認是否被無效化 (依攻擊類型)
        if (defenser.buffManager.OnAttackedInvalidOfAttackType(attack.attackType) == true)
        {
            FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (攻擊類型無效)");
            return true;
        }

        //防守方的效果確認是否被無效化 (依傷害類型)
        if (defenser.buffManager.OnAttackedInvalidOfDamageType(attack.damageType) == true)
        {
            FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (傷害類型無效)");
            return true;
        }

        //防守方的效果確認是否被無效化 (依攻擊屬性)
        if (defenser.buffManager.OnAttackedInvalidOfAttackElement(attack.attackElement) == true)
        {
            FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (屬性無效)");
            return true;
        }

        //防守方的效果確認是否被無效化 (依卡片屬性)
        if(card != null)
        {
            if (defenser.buffManager.OnAttackedInvalidOfCardElement(card.element) == true)
            {
                FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (屬性無效)");
                return true;
            }
        }        

        return false;
    }

    //確認攻擊是否可命中
    bool AttackSuccessful(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        float finalAccuracy = 100; //攻擊命中率

        //攻擊可能提升命中率
        finalAccuracy += attack.accuracy;

        //攻擊方的效果修正命中率
        finalAccuracy = attacker.buffManager.OnAttackAccuracy(finalAccuracy);

        //攻擊方的狀態修正命中率
        finalAccuracy = attacker.stateManager.OnAttackAccuracy(finalAccuracy);

        //防守方的效果修正命中率
        finalAccuracy = defenser.buffManager.OnAttackedAccuracy(finalAccuracy);

        //防守方的狀態修正命中率
        finalAccuracy = defenser.stateManager.OnAttackedAccuracy(finalAccuracy);

        //防守方的閃避率修正命中率
        finalAccuracy = finalAccuracy - defenser.initEvasion;

        //計算概率確認攻擊是否可命中
        float r = UnityEngine.Random.Range(0, 100);
        if (r < finalAccuracy)
        {
            return true;
        }

        FightLogManager.Instance.WriteFightLog($"MISS ! (未命中)");
        return false;
    }

    //對防守方造成傷害事件
    public event Action<CharacterModel, float> OnDefenseGetDamage;

    //對防守方造成傷害
    void DefenseGetDamage(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        float finalDamage = 0;

        float attackerAttack = attacker.initAtk; //攻擊方的攻擊力
        //此處根據是否有提供卡片資料來判斷這個攻擊是否發動自卡片的技能
        //如果是卡片的技能，則攻擊方的攻擊力應改寫為卡片的攻擊力
        if (card != null)
        {
            attackerAttack = card.atk;
        }
        AttackType attackAttackType = attack.attackType; //攻擊的攻擊類型
        DamageType attackDamageType = attack.damageType; //攻擊的傷害類型
        AttackElement attackAttackElement = attack.attackElement; //攻擊的傷害屬性
        float attackDamageMultiplier = attack.damageMultiplier; //攻擊的攻擊倍率        

        //確認此次攻擊是否會造成傷害 
        if (attack.useAttack)
        {
            // 基礎公式計算 => 攻擊方的攻擊力 * 攻擊的傷害倍率
            finalDamage = attackerAttack * attackDamageMultiplier;

            //攻擊方的效果修正造成的傷害
            finalDamage = attacker.buffManager.OnAttackDamage(finalDamage, attackDamageType);

            //攻擊方的狀態修正造成的傷害
            finalDamage = attacker.stateManager.OnAttackDamage(finalDamage);

            //防守方的效果修正受到的傷害
            finalDamage = defenser.buffManager.OnAttackedDamage(finalDamage, attackDamageType);

            //防守方的狀態修正受到的傷害
            if(card == null)
            {
                finalDamage = defenser.stateManager.OnAttackedDamage(attack.attackElement, null, finalDamage);
            }
            else
            {
                finalDamage = defenser.stateManager.OnAttackedDamage(attack.attackElement, card.element, finalDamage);
            }

            //依照COMBO數提升傷害
            finalDamage = finalDamage * (1 + (combo * 0.1f));

            //防守方的狀態觸發狀態移除 (ex: 睡眠)
            StateSO dState = defenser.stateManager.OnAttackedDeleteState(defenser);
            if (dState != null)
            {
                defenser.DeleteState(dState);
            }

            float oldFinalDamage = finalDamage;

            //防守方的效果確認是否被無效化 (依傷害量)
            finalDamage = defenser.buffManager.OnAttackedInvalidOfDamage(finalDamage);                      

            if(finalDamage > 0)
            {
                //防守方受到傷害
                defenser.ReduceNowHp(finalDamage);
                OnDefenseGetDamage?.Invoke(defenser, finalDamage);
                if (card != null)
                {
                    FightLogManager.Instance.WriteFightLog($"{defenser.displayName} 受到了 {Mathf.RoundToInt(finalDamage)} 點傷害 !");
                }
                else
                {
                    FightLogManager.Instance.WriteFightLog($"{defenser.displayName} 受到了 {Mathf.RoundToInt(finalDamage)} 點傷害 !");
                }

                //COMBO數提升 (攻擊可能有額外COMBO數提升)
                combo += (1 + attack.combo);
                if (attack.combo != 0)
                {
                    FightLogManager.Instance.WriteFightLog($"COMBO追加效果 !");
                }

                useHealAction = true;
                useDeleteEnemyAction = true;
                useDown = true;

                //!特殊邏輯! 防守方的效果確認是否反擊 (不經過攻擊流程直接造成反擊傷害)
                bool use = defenser.buffManager.OnAttackedUseCounter(attackAttackType);
                if (use == true)
                {
                    //攻擊方受到傷害
                    float counterDamage = defenser.buffManager.OnAttackedUseCounterDamage(finalDamage);
                    attacker.ReduceNowHp(counterDamage);
                    OnDefenseGetDamage?.Invoke(attacker, counterDamage);
                    FightLogManager.Instance.WriteFightLog($"{defenser.displayName} 發起反擊 ! {attacker.displayName} 受到了 {Mathf.RoundToInt(counterDamage)} 點傷害 !");
                }
            }
            else
            {
                if (oldFinalDamage == finalDamage)
                {
                    FightLogManager.Instance.WriteFightLog($"無法造成傷害 !");
                }
                else
                {
                    FightLogManager.Instance.WriteFightLog($"無法造成傷害 ! (極限值突破)");
                }                
            }
        }        
    }

    //對攻擊方附加效果 (概率附加)
    void AttackerAddBuffs(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        IReadOnlyList<AttackBuffs> attackBuffs = attack.OnAttackReturnAttackerAddBuffs(); //效果組合

        foreach (AttackBuffs buff in attackBuffs)
        {
            if(buff.attackBuff != null)
            {
                float r = UnityEngine.Random.Range(0, 100);
                if (r < buff.attackBuffProbability)
                {
                    attacker.AddBuff(buff.attackBuff);
                    FightManager.Instance.PlayAudioForName("AddBuff");
                }
            }            
        }
    }

    //對攻擊方移除狀態 (必定移除)
    void AttackerDeleteStates(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        IReadOnlyList<AttackStates> attackStates = attack.OnAttackReturnAttackerDeleteStates(); //狀態組合

        foreach (AttackStates state in attackStates)
        {
            if(state.attackState != null)
            {
                attacker.DeleteState(state.attackState);
            }
            else
            {
                attacker.DeleteRandomState();                
            }
            FightManager.Instance.PlayAudioForName("DeleteState");
        }
    }

    //對防守方移除效果 (必定移除)
    void DefenserDeleteBuffs(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        IReadOnlyList<AttackBuffs> attackBuffs = attack.OnAttackReturnDefenserDeleteBuffs(); //效果組合

        foreach (AttackBuffs buff in attackBuffs)
        {
            if (buff.attackBuff != null)
            {
                defenser.DeleteBuff(buff.attackBuff);
            }
            else
            {
                defenser.DeleteRandomBuff();               
            }
            FightManager.Instance.PlayAudioForName("DeleteBuff");            
        }
    }

    //對防守方附加狀態 (概率附加)
    void DefenserAddStates(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        IReadOnlyList<AttackStates> attackStates = attack.OnAttackReturnDefenserAddStates(); //狀態組合

        foreach (AttackStates state in attackStates)
        {
            if(state.attackState != null)
            {
                //攻擊方對防守方附加狀態的機率會受到防守方的異常抗性影響機率
                float finalProbability = state.attackStateProbability - defenser.initStateResistance;
                float r = UnityEngine.Random.Range(0, 100);
                if (r < finalProbability)
                {
                    defenser.AddState(state.attackState);
                    FightManager.Instance.PlayAudioForName("AddState");
                }
            }           
        }
    }

    //蘭斯專用移除無敵結界效果
    void RanceDeleteBuff(CharacterModel attacker, CharacterModel defenser, CardModel card, AttackModel attack)
    {
        AttackBuffs deleteBuff = attack.ReturnRanceDeleteBuff();

        if (deleteBuff != null)
        {
            if(deleteBuff.attackBuff != null)
            {
                defenser.DeleteBuff(deleteBuff.attackBuff);
            }           
        }
    }
}

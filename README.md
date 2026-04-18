# Unity Recreate Rance10 Fight System

這是一個 Unity 專案，專案內容在於還原一知名遊戲 Rance 10<br>
此專案用於做為個人的作品集，目的是為展示個人的遊戲開發技術，僅作為學術用途<br>
本人並沒有將圖片、音訊...等等的美術素材提交上此倉庫，避免可能會有侵權問題<br>
此專案重點在於遊戲的程式架構方面設計，而非美術呈現<br>


## 專案簡介

遊戲的初始畫面即可開始編輯戰鬥隊伍以及選擇要戰鬥的敵人來立刻戰鬥<br>
專案中已還原了大部分原遊戲中的主要人物的卡片可供戰鬥使用<br>
可戰鬥的敵人則是還原了原遊戲中的主要BOSS<br>
專案亦有實作簡易的存檔功能<br>
戰勝過的敵人會有存檔紀錄，之後該敵人將會展示出(已擊敗)的文字提示<br>


## 技術分析

使用單例模式製作了大部分系統層面的功能，避免緊耦合的低級架構<br>
ex:<br>
戰鬥系統 = FightManager<br>
攻擊交互系統 = BattleManager<br>
卡片管理系統 = CardPoolManager<br>
音效系統 = AudioManager<br><br>

使用ScriptableObject定義並製作遊戲資源<br>
ex:<br>
角色 = CharacterSO<br>
卡片 = CardSO<br>
技能 = AttackSO<br>
技能釋放條件 = AttackConditionSO<br>
效果 = BuffSO<br>
狀態 = StateSO<br><br>

使用event製作系統與UI之間的交互，達成邏輯和畫面的乾淨分割<br><br>

有實作簡易的客戶端存檔功能，存檔的檔案格式使用主流的JSON格式<br>


## 遊戲DEMO展示

影片連結: https://www.youtube.com/watch?v=vzpf1sbhk_8<br>


## 素材聲明

專案中有使用到原遊戲的美術素材，因為本人沒有美術能力不會自產美術素材，目的只是為避免專案過於簡陋和追求還原性<br>
美術素材的版權皆屬於原著作權人，本人不主張任何權利，亦不將此專案用作於任何商業用途，僅僅是用作個人的學術用途

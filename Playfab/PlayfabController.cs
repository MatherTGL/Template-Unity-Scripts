using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boot;
using Cysharp.Threading.Tasks;
using GameAssets.Meta.Quests;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabController : MonoBehaviour, IBoot
    {
        //TODO использовать
        public static int MaxRetries { get; private set; } = 3;

        public static float RetryDelay { get; private set; } = 2f;

        private static readonly PlayfabLogin _login = new();

        private static readonly PlayfabLeaderboard _leaderboard = new();

        private static readonly PlayfabSaves _saves = new();

        private static readonly PlayfabFriends _friends = new();

        private static readonly PlayfabGetUserData _userData = new();

        private static readonly PlayfabQuests _quests = new();
        
        //TODO после присваивать значение
        private static readonly string _userID = "SORGT3948RKAF";


        void IBoot.InitAwake() { }

        async void IBoot.InitStart()
        {
            TelegramManager.Init();
            _friends.Init();
            var loginName = TelegramManager.webAppService.InitDataUnsafe.User.Id.ToString();
            Debug.Log($"loginName: {loginName}");
            await _login.LoginAsync(_userID);

            //TODO перенести в более нужное место отправку данных в борд
            SendDataToLeaderboard();
        }

        (Bootstrap.TypeLoadObject typeLoad, Bootstrap.TypeSingleOrLotsOf singleOrLotsOf) IBoot.GetTypeLoad()
            => (Bootstrap.TypeLoadObject.SuperImportant, Bootstrap.TypeSingleOrLotsOf.Single);

        public static async UniTask LoginAsync()
            => await _login.LoginAsync(_userID);

        public static void SendDataToLeaderboard()
            => _leaderboard.Send();

        public static async UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardAsync()
            => await _leaderboard.GetAsync();

        public static void SaveData(string otherData, string gameData)
            => _saves.Save(otherData, gameData);

        public static async UniTask<(string, string)> LoadDataAsync()
            => await _saves.LoadAsync();

        public static async UniTask<bool> IsRegistationAsync()
            => await _login.IsRegistationAsync();

        public static async UniTask<List<FriendInfo>> GetFriendsListAsync()
            => await _friends.GetFriendsAsync();

        public static async UniTask<float> GetRefCoinsAsync()
            => await _userData.GetRefCoinsAsync();

        public static async UniTask<List<BaseQuest>> LoadAllServerQuestsAsync()
            => await _quests.LoadAllServerQuestsAsync();

        public static async UniTask<bool> TryStartQuestAsync(IQuest quest)
            => await _quests.TryStartQuestAsync(quest);

        public static async UniTask<bool> TryCompleteQuestAsync(IQuest quest)
            => await _quests.TryCompleteQuestAsync(quest);

        public static async UniTask<bool> TryTakeRewardAsync(IQuest quest)
            => await _quests.TryTakeRewardAsync(quest);
    }
}

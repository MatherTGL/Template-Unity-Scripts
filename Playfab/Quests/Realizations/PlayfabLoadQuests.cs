using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameAssets.Meta.Quests;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabLoadQuests
    {
        private TaskCompletionSource<PlayfabQuestsList> _loadAllQuests = null;
        
        
        public async UniTask<List<BaseQuest>> LoadAllQuestsAsync(PlayfabQuestsList questsList)
        {
            if (_loadAllQuests == null)
            {
                await PlayfabController.LoginAsync();
                LoadQuestsRequestWithRetries(questsList).Subscribe();
            }
            
            PlayfabQuestsList result = await _loadAllQuests.Task;
            _loadAllQuests = null;

            if (result != null && result.quests != null)
                return new List<BaseQuest>(result.quests);
            
            return null;
        }
        
        private IObservable<Unit> LoadQuestsRequestWithRetries(PlayfabQuestsList questsList)
        {
            _loadAllQuests = new();

            return Observable.Defer(() => SendRequest(questsList)).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> SendRequest(PlayfabQuestsList questsList)
        {
            return Observable.Create<Unit>(observer =>
            {
                PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
                {
                    if (result.Data != null && result.Data.ContainsKey("questsData"))
                    {
                        questsList = JsonConvert.DeserializeObject<PlayfabQuestsList>(result.Data["questsData"].Value);
                        Debug.Log($"quests: {questsList} / {questsList.quests} / {questsList.quests?.Count}");
                        _loadAllQuests.SetResult(questsList);
                        Debug.Log($"Tasks for the server have been found! {questsList.quests.Count}");
                    }
                    else
                    {
                        Debug.Log("No quest data found on server.");
                        _loadAllQuests.SetResult(null);
                    }
                }, error =>
                {
                    var ex = new Exception(error.ErrorMessage);
                    _loadAllQuests.SetException(ex);
                });

                return Disposable.Empty;
            });
        }
    }
}
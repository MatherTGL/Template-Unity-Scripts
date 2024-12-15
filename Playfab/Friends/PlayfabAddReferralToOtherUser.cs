using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameAssets.Meta.Referrals;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameAssets.General.Server
{
    public sealed class PlayfabAddReferralToOtherUser
    {
        private TaskCompletionSource<Unit> _taskCompeletionAddReferral = null;


        public async UniTask AddReferralToUserAsync(string targetID)
        {
            if (_taskCompeletionAddReferral == null)
            {
                await PlayfabController.LoginAsync();
                string currentPlayerId = PlayFabSettings.staticPlayer.PlayFabId;

                if (string.IsNullOrEmpty(currentPlayerId))
                    Debug.LogError(new Exception("Current player is not logged in or Title Player Account ID is missing!"));

                GetProfileRequestWithRetries(targetID, currentPlayerId).Subscribe();
            }

            await _taskCompeletionAddReferral.Task;
            _taskCompeletionAddReferral = null;
        }

        private IObservable<Unit> GetProfileRequestWithRetries(string targetID, string accountID)
        {
            _taskCompeletionAddReferral = new TaskCompletionSource<Unit>();

            return Observable.Defer(() => FindPlayerByTitlePlayerAccountID(targetID, accountID)).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                 _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> FindPlayerByTitlePlayerAccountID(string targetID, string accountID)
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new GetUserDataRequest
                {
                    PlayFabId = targetID,
                };

                PlayFabClientAPI.GetUserData(request,
                    result =>
                    {
                        if (result == null) return;
                        Debug.Log("GetUserData result and go AddFriend method");
                        AddFriend(targetID, accountID).Subscribe();
                        observer.OnCompleted();
                    },
                    error =>
                    {
                        var ex = new Exception($"Player not found: {error.ErrorMessage}");
                        _taskCompeletionAddReferral.SetException(ex);
                        observer.OnError(ex);
                    }
                );

                return Disposable.Empty;
            });
        }

        private IObservable<Unit> AddFriend(string targetID, string accountID)
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new ExecuteCloudScriptRequest
                {
                    FunctionName = "AddFriendToPlayer",
                    FunctionParameter = new
                    {
                        targetPlayerId = targetID,
                        friendId = accountID
                    }
                };

                PlayFabClientAPI.ExecuteCloudScript(request,
                    _ =>
                    {
                        Debug.Log("AddFriend method result is success");
                        AccrueMoneyToPlayers(targetID, accountID);
                        observer.OnCompleted();
                    },
                    error =>
                    {
                        var ex = new Exception($"Player not found: {error.ErrorMessage}");
                        _taskCompeletionAddReferral.SetException(ex);
                        observer.OnError(ex);
                    }
                );

                return Disposable.Empty;
            });
        }
        
        private async void AccrueMoneyToPlayers(string target, string account)
        {
            var config = await Addressables.LoadAssetAsync<ReferralsConfig>("ReferralConfig").Task;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "updateRefCoinsForPlayers",
                FunctionParameter = new
                {
                    accountID = account,
                    targetID = target,
                    coinsToAddAccount = config.referralsPlayerBonus,
                    coinsToAddTarget = config.inviterBonus
                }
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    _taskCompeletionAddReferral.SetResult(Unit.Default);
                    Debug.Log($"Cloud Script выполнен успешно. Ответ: {result.FunctionResult}");
                },
                error =>
                {
                    Debug.LogError($"Ошибка вызова Cloud Script: {error.GenerateErrorReport()}");
                    _taskCompeletionAddReferral.SetException(new Exception(error.ErrorMessage));
                }
            );
        }
    }
}

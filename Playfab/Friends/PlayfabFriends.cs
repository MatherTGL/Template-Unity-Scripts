using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabFriends
    {
        private readonly PlayfabAddReferralToOtherUser _playfabFindTitleID = new();

        private TaskCompletionSource<GetFriendsListResult> _taskCompeletionGetFriends = null;


        public async void Init()
        {
            //TODO передавать нужный айди, который получим с ссылки тг
            if (await PlayfabController.IsRegistationAsync() == false)
                AddFriendsToReferral("");
        }

        public async void AddFriendsToReferral(string refID)
        {
            Debug.Log("void AddFriendsToReferral");
            //TODO для теста. Должен браться с ссылки тг
            refID = "717BE9626C32C62";
            await _playfabFindTitleID.AddReferralToUserAsync(refID);
        }

        public async UniTask<List<FriendInfo>> GetFriendsAsync()
        {
            await PlayfabController.LoginAsync();

            if (_taskCompeletionGetFriends == null)
            {
                _taskCompeletionGetFriends = new();
                GetRequestWithRetries().Subscribe();
            }

            var result = await _taskCompeletionGetFriends.Task;
            _taskCompeletionGetFriends = null;
            return result.Friends;
        }

        private IObservable<GetFriendsListResult> GetRequestWithRetries()
        {
            return Observable.Defer(() => SendGetRequest()).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<GetFriendsListResult> SendGetRequest()
        {
            return Observable.Create<GetFriendsListResult>(observable =>
            {
                var request = new GetFriendsListRequest { };

                PlayFabClientAPI.GetFriendsList(request,
                    result =>
                    {
                        _taskCompeletionGetFriends.SetResult(result);
                        observable.OnNext(result);
                        observable.OnCompleted();
                    },
                    error =>
                    {
                        var ex = new Exception(error.ErrorMessage);
                        _taskCompeletionGetFriends.SetException(ex);
                        observable.OnError(ex);
                    }
                );

                return Disposable.Empty;
            });
        }
    }
}

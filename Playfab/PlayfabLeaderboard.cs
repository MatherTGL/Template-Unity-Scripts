using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameAssets.Player.Data;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabLeaderboard
    {
        private TaskCompletionSource<List<PlayerLeaderboardEntry>> _taskCompeletionSource;


        public async void Send()
        {
            await PlayfabController.LoginAsync();
            var currentPlayerCoins = await DataContoller.Imodel.GetCoinsAsync();

            SendRequestWithRetries(currentPlayerCoins).Subscribe();
        }

        private IObservable<Unit> SendRequestWithRetries(float currentCoins)
        {
            return Observable.Defer(() => SendPlayFabRequest(currentCoins)).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> SendPlayFabRequest(float currentCoins)
        {
            return Observable.Create<Unit>(observer =>
            {
                var requestUpdateLeaderboard = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = "Coins",
                            Value = currentCoins
                        }
                    }
                };

                PlayFabClientAPI.UpdatePlayerStatistics(requestUpdateLeaderboard,
                    result =>
                    {
                        observer.OnCompleted();
                    },
                    error =>
                    {
                        observer.OnError(new Exception(error.ErrorMessage));
                    }
                );

                return Disposable.Empty;
            });
        }

        public async UniTask<List<PlayerLeaderboardEntry>> GetAsync()
        {
            await PlayfabController.LoginAsync();

            if (_taskCompeletionSource == null)
            {
                _taskCompeletionSource = new();
                GetLeaderboardRequestWithRetries().Subscribe();
            }

            return await _taskCompeletionSource.Task;
        }

        private IObservable<GetLeaderboardResult> GetLeaderboardRequestWithRetries()
        {
            return Observable.Defer(() => GetPlayFabRequest()).Retry(3).Delay(TimeSpan.FromSeconds(1)).Do(
                 _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<GetLeaderboardResult> GetPlayFabRequest()
        {
            return Observable.Create<GetLeaderboardResult>(observer =>
            {
                var request = new GetLeaderboardRequest
                {
                    StatisticName = "Coins",
                    StartPosition = 0,
                    MaxResultsCount = 10
                };

                PlayFabClientAPI.GetLeaderboard(request,
                    result =>
                    {
                        _taskCompeletionSource.SetResult(result.Leaderboard);
                        observer.OnNext(result);
                        observer.OnCompleted();
                    },
                    error =>
                    {
                        var ex = new Exception($"Get leaderboard request failed: {error.ErrorMessage}");
                        observer.OnError(ex);
                        _taskCompeletionSource.SetException(ex);
                    }
                );

                return Disposable.Empty;
            });
        }
    }
}

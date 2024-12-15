using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameAssets.System.SaveSystem;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabGetUserData
    {
        private TaskCompletionSource<float> _tcsRefCoins = null;


        public async UniTask<float> GetRefCoinsAsync()
        {
            if (_tcsRefCoins == null)
                GetRefCoinsRequestWithRetries().Subscribe();

            return await _tcsRefCoins.Task;
        }

        private IObservable<Unit> GetRefCoinsRequestWithRetries()
        {
            _tcsRefCoins = new();

            return Observable.Defer(() => SendRequest()).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> SendRequest()
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new GetUserDataRequest
                {
                    PlayFabId = PlayFabSettings.staticPlayer.PlayFabId
                };

                PlayFabClientAPI.GetUserData(request,
                    result =>
                    {
                        if (result.Data != null && result.Data.ContainsKey("refCoins"))
                        {
                            var refCoins = result.Data["refCoins"].Value;
                            _tcsRefCoins.SetResult((float)Convert.ToDouble(refCoins));
                        }
                    },
                    error =>
                    {
                        var ex = new Exception(error.ErrorMessage);
                        _tcsRefCoins.SetException(ex);
                        observer.OnError(ex);
                    }
                );

                var requestUpdateData = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "refCoins", "0" }
                    }
                };

                PlayFabClientAPI.UpdateUserData(requestUpdateData,
                    _ =>
                    {
                        Debug.Log("Обновление данных refCoins успешно!");
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
    }
}

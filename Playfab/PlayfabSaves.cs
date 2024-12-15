using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabSaves
    {
        private TaskCompletionSource<UpdateUserDataResult> _taskCompSourceSave = null;

        private TaskCompletionSource<GetUserDataResult> _taskCompSourceLoad = null;


        public async void Save(string otherData, string gameData)
        {
            if (_taskCompSourceSave == null)
                SaveRequestWithRetries(otherData, gameData).Subscribe();

            await _taskCompSourceSave.Task;
        }

        private IObservable<Unit> SaveRequestWithRetries(string otherData, string gameData)
        {
            _taskCompSourceSave = new();

            return Observable.Defer(() => SendSaveRequest(otherData, gameData)).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                 _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> SendSaveRequest(string otherData, string gameData)
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "otherData", otherData },
                        { "gameData", gameData }
                    }
                };

                PlayFabClientAPI.UpdateUserData(request,
                    result =>
                    {
                        _taskCompSourceSave.SetResult(result);
                        observer.OnCompleted();
                        _taskCompSourceSave = null;
                        Debug.Log("Данные сохранены на сервер успешно!");
                    },
                    error =>
                    {
                        var ex = new Exception(error.ErrorMessage);
                        _taskCompSourceSave.SetException(ex);
                        observer.OnError(ex);
                        _taskCompSourceSave = null;
                        Debug.LogError($"Данные не смогли сохраниться на сервер! {error.Error}");
                    }
                );

                return Disposable.Empty;
            });
        }

        public async UniTask<(string gameData, string otherData)> LoadAsync()
        {
            await PlayfabController.LoginAsync();

            if (_taskCompSourceLoad == null)
            {
                _taskCompSourceLoad = new();
                LoadRequestWithRetries().Subscribe();
            }
            GetUserDataResult result = await _taskCompSourceLoad.Task;
            return GetData(result);
        }

        private IObservable<GetUserDataResult> LoadRequestWithRetries()
        {
            return Observable.Defer(() => SendLoadRequest()).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<GetUserDataResult> SendLoadRequest()
        {
            return Observable.Create<GetUserDataResult>(observer =>
            {
                PlayFabClientAPI.GetUserData(new GetUserDataRequest
                {
                    PlayFabId = null
                },
                result =>
                {
                    observer.OnNext(result);
                    _taskCompSourceLoad.SetResult(result);
                    observer.OnCompleted();
                },
                error =>
                {
                    var ex = new Exception($"exception: {error.Error}");
                    _taskCompSourceLoad.SetException(ex);
                    observer.OnError(ex);
                });

                return Disposable.Empty;
            });
        }

        private (string gameData, string otherData) GetData(GetUserDataResult result)
        {
            return (result.Data.Where(config => config.Key == "gameData").Select(config => config.Value.Value).FirstOrDefault(),
                    result.Data.Where(config => config.Key == "otherData").Select(config => config.Value.Value).FirstOrDefault());
        }
    }
}

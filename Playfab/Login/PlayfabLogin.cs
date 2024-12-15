using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;

namespace GameAssets.General.Server
{
    public sealed class PlayfabLogin
    {
        private readonly PlayfabCheckRegistration _playfabCheckRegistration = new();

        private TaskCompletionSource<LoginResult> _taskCompSource;


        public async UniTask LoginAsync(string telegramUserID)
        {
            if (_taskCompSource == null && PlayFabClientAPI.IsClientLoggedIn() == false)
                LoginRequestWithRetries(telegramUserID).Subscribe();

            await _taskCompSource.Task;
        }

        public async UniTask<bool> IsRegistationAsync() => await _playfabCheckRegistration.IsRegistationAsync();

        private IObservable<Unit> LoginRequestWithRetries(string telegramUserID)
        {
            _taskCompSource = new();

            return Observable.Defer(() => SendRequest(telegramUserID)).Retry(3).Delay(TimeSpan.FromSeconds(2)).Do(
                _ => Debug.Log("Request succeeded"),
                ex => Debug.LogError($"Request failed after retries: {ex.Message}")
            );
        }

        private IObservable<Unit> SendRequest(string telegramUserID)
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new LoginWithCustomIDRequest
                {
                    CustomId = telegramUserID,
                    CreateAccount = true
                };

                PlayFabClientAPI.LoginWithCustomID(request,
                    result =>
                    {
                        _taskCompSource.SetResult(result);
                        observer.OnCompleted();
                    },
                    error =>
                    {
                        var ex = new Exception(error.GenerateErrorReport());
                        _taskCompSource.SetException(ex);
                        observer.OnError(ex);
                    }
                );

                return Disposable.Empty;
            });
        }
    }
}

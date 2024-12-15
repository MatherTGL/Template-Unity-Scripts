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
    public sealed class PlayfabCheckRegistration
    {
        private TaskCompletionSource<bool> _taskCompeletionIsRegistation = null;


        public async UniTask<bool> IsRegistationAsync()
        {
            await PlayfabController.LoginAsync();

            if (_taskCompeletionIsRegistation == null)
            {
                _taskCompeletionIsRegistation = new();

                PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                    result =>
                    {
                        if (result.Data != null && result.Data.ContainsKey("IsRegistered"))
                        {
                            Debug.Log("Player is already registered. Skipping logic.");
                            _taskCompeletionIsRegistation.SetResult(true);
                            return;
                        }

                        SetPlayerRegisteredFlag().Subscribe();
                    },
                    error =>
                    {
                        _taskCompeletionIsRegistation.SetException(new Exception(error.ErrorMessage));
                    }
                );
            }

            bool result = await _taskCompeletionIsRegistation.Task;
            _taskCompeletionIsRegistation = null;
            return result;
        }

        private IObservable<Unit> SetPlayerRegisteredFlag()
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string> { { "IsRegistered", "true" } }
                };

                PlayFabClientAPI.UpdateUserData(request,
                    _ =>
                    {
                        Debug.Log("Player registered flag set successfully.");
                        _taskCompeletionIsRegistation.SetResult(false);
                    },
                    error =>
                    {
                        Debug.LogError("Failed to set player registered flag: " + error.GenerateErrorReport());
                        _taskCompeletionIsRegistation.SetException(new Exception(error.ErrorMessage));
                    }
                );

                return Disposable.Empty;
            });
        }
    }
}

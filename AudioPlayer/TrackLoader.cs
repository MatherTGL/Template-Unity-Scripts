using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameAssets.Scripts.Utils.AudioPlayer
{
    public class TrackLoader : MonoBehaviour
    {
        [SerializeField] private MusicController musicController;
        private AsyncOperationHandle<AudioTrackConfig> prevTrackHandle;
        private AsyncOperationHandle<AudioTrackConfig> currentTrackHandle;

        public async void LoadTrack(AssetReferenceAudioTrackConfig trackConfigRef)
        {
            currentTrackHandle = Addressables.LoadAssetAsync<AudioTrackConfig>(trackConfigRef);
            AudioTrackConfig trackConfig = await currentTrackHandle.Task;
            musicController.SwitchToTrackConfig(trackConfig, false, true, UnloadPrevTrack);
            prevTrackHandle = currentTrackHandle;
        }

        public async void LoadTrack(string trackConfigName)
        {
            currentTrackHandle = Addressables.LoadAssetAsync<AudioTrackConfig>(trackConfigName);
            AudioTrackConfig trackConfig = await currentTrackHandle.Task;
            musicController.SwitchToTrackConfig(trackConfig, false, true, UnloadPrevTrack);
            prevTrackHandle = currentTrackHandle;
        }

        private void UnloadPrevTrack()
        {
            if (prevTrackHandle.IsValid()) Addressables.Release(prevTrackHandle);
        }
    }
}
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameAssets.Scripts.Utils.AudioPlayer
{
    public static class TrackLoaderMain
    {
        private static AsyncOperationHandle<AudioTrackConfig> prevTrackHandle;
        private static AsyncOperationHandle<AudioTrackConfig> currentTrackHandle;

        public static async void LoadTrack(AssetReferenceAudioTrackConfig trackConfigRef)
        {
            currentTrackHandle = Addressables.LoadAssetAsync<AudioTrackConfig>(trackConfigRef);
            AudioTrackConfig trackConfig = await currentTrackHandle.Task;
            MusicController.Instance.SwitchToTrackConfig(trackConfig, false, true, UnloadPrevTrack);
            MusicController.Instance.Play();
            prevTrackHandle = currentTrackHandle;
        }
        
        public static async void LoadTrack(AssetReferenceAudioTrackConfig trackConfigName, bool sync)
        {
            currentTrackHandle = Addressables.LoadAssetAsync<AudioTrackConfig>(trackConfigName);
            AudioTrackConfig trackConfig = await currentTrackHandle.Task;
            MusicController.Instance.SwitchToTrackConfig(trackConfig, sync, true, UnloadPrevTrack);
            MusicController.Instance.Play();
            prevTrackHandle = currentTrackHandle;
        }

        public static async void LoadTrack(string trackConfigName)
        {
            currentTrackHandle = Addressables.LoadAssetAsync<AudioTrackConfig>(trackConfigName);
            AudioTrackConfig trackConfig = await currentTrackHandle.Task;
            MusicController.Instance.SwitchToTrackConfig(trackConfig, false, true, UnloadPrevTrack);
            MusicController.Instance.Play();
            prevTrackHandle = currentTrackHandle;
        }

        public static void SetSegment(ushort segment)
        {
            MusicController.Instance.SetSegment(segment);
        }

        public static void Stop()
        {
            MusicController.Instance.Stop();
        }

        private static void UnloadPrevTrack()
        {
            if (prevTrackHandle.IsValid()) Addressables.Release(prevTrackHandle);
        }
    }
}
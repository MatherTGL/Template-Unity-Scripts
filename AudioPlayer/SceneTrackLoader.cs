using UnityEngine;

namespace GameAssets.Scripts.Utils.AudioPlayer
{
    public class SceneTrackLoader : MonoBehaviour
    {
        [SerializeField] private AssetReferenceAudioTrackConfig track;
        [SerializeField] private bool setOnStart = true;

        private void Start()
        {
            if (setOnStart)
                LoadTrack();
        }

        public void LoadTrack()
        {
            TrackLoaderMain.LoadTrack(track);
        }
        
        public void LoadTrack(bool sync)
        {
            TrackLoaderMain.LoadTrack(track, sync);
        }

        public void StopTrack()
        {
            TrackLoaderMain.Stop();
        }
        
        public void SetSegment(int segment)
        {
            TrackLoaderMain.SetSegment((ushort)segment);
        }
    }
}
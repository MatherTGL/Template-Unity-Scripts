using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AudioLayer", menuName = "Audio/AudioTrackConfig")]
public class AudioTrackConfig : ScriptableObject
{
    [field: SerializeField] public AudioClip AudioClip { get; private set; }
    [field: SerializeField] public MusicController.LoopSegment[] LoopSegments { get; private set; }
    [field: SerializeField] public float FadeDuration { get; private set; } = 1f;
    [field: SerializeField] public MusicController.BpmMarker[] BpmMarkers { get; private set; }

    public void SetData(AudioClip audioClip, MusicController.LoopSegment[] loopSegments, float fadeDuration,
        MusicController.BpmMarker[] bpmMarkers)
    {
        AudioClip = audioClip;
        LoopSegments = loopSegments;
        FadeDuration = fadeDuration;
        BpmMarkers = bpmMarkers;
    }
}

[Serializable]
public class AssetReferenceAudioTrackConfig : AssetReferenceT<AudioTrackConfig>
{
    public AssetReferenceAudioTrackConfig(string guid) : base(guid)
    {
    }
}
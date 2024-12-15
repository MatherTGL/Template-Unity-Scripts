using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public bool singleton;

    public AudioClip audioClip;
    [field: SerializeField] public bool PlayOnAwake { get; private set; }
    [field: SerializeField] public AudioMixerGroup TargetMixerGroup { get; private set; }
    public LoopSegment[] loopSegments = Array.Empty<LoopSegment>();
    public float fadeDuration = 1f;
    public BpmMarker[] bpmMarkers = Array.Empty<BpmMarker>();
    public AudioSource AudioSource { get; private set; }
    private Coroutine fadeCoroutine;
    private Coroutine updateLoopCoroutine;
    private Coroutine waitForBeatCoroutine;
    public int currentSegmentIndex { get; private set; }
    private bool paused;

    private static MusicController _instance;

    public static MusicController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("MusicController");
                _instance = gameObject.AddComponent<MusicController>();
                _instance.singleton = true;
                DontDestroyOnLoad(gameObject);
            }

            return _instance;
        }
    }

    [Serializable]
    public struct LoopSegment
    {
        public float startTime;
        public float endTime;

        public LoopSegment(float startTime, float endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }

    [Serializable]
    public struct BpmMarker
    {
        public float Time;
        public float Bpm;
        public int BeatsPerBar;

        public BpmMarker(float time, float bpm, int beatsPerBar)
        {
            Time = time;
            Bpm = bpm;
            BeatsPerBar = beatsPerBar;
        }
    }

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        if (singleton)
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);
        }

        if (AudioSource == null)
        {
            AudioSource = gameObject.GetComponent<AudioSource>();
        }

        AudioSource.playOnAwake = false;
        Init();
        if (PlayOnAwake) Play();
    }

    private void Init()
    {
        if (AudioSource == null)
        {
            AudioSource = gameObject.GetComponent<AudioSource>();
        }

        AudioSource.outputAudioMixerGroup = TargetMixerGroup;
        AudioSource.playOnAwake = false;
        AudioSource.clip = audioClip;
        AudioSource.loop = true;
    }

    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            if (AudioSource.time >= loopSegments[currentSegmentIndex].endTime)
            {
                AudioSource.time = loopSegments[currentSegmentIndex].startTime;
            }

            yield return null;
        }
    }

    public void Play()
    {
        if (updateLoopCoroutine != null) StopCoroutine(updateLoopCoroutine);
        if (AudioSource.isPlaying) return;
        AudioSource.volume = 1;

        if (paused) paused = false;
        else AudioSource.time = 0;

        AudioSource.Play();
        updateLoopCoroutine = StartCoroutine(UpdateLoop());
    }

    public void Renew()
    {
        Stop(true);
        Play();
    }

    public void Pause()
    {
        if (!AudioSource.isPlaying) return;
        if (updateLoopCoroutine != null) StopCoroutine(updateLoopCoroutine);
        FadeToVolume(0f, () =>
        {
            AudioSource.Pause();
            paused = true;
        });
    }

    public void Pause(bool immediately)
    {
        if (!AudioSource.isPlaying) return;
        if (updateLoopCoroutine != null) StopCoroutine(updateLoopCoroutine);

        if (immediately)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            AudioSource.Pause();
            paused = true;
        }
        else
        {
            FadeToVolume(0f, () =>
            {
                AudioSource.Pause();
                paused = true;
            });
        }
    }

    public void Stop()
    {
        paused = false;
        if (!AudioSource.isPlaying) return;
        if (updateLoopCoroutine != null) StopCoroutine(updateLoopCoroutine);
        FadeToVolume(0f, () => { AudioSource.Stop(); });
    }

    public void Stop(bool immediately)
    {
        paused = false;
        if (!AudioSource.isPlaying) return;
        if (updateLoopCoroutine != null) StopCoroutine(updateLoopCoroutine);
        if (immediately)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            AudioSource.Stop();
        }
        else
        {
            FadeToVolume(0f, () => { AudioSource.Stop(); });
        }
    }

    public void SetSegment(ushort segmentIndex)
    {
        if (waitForBeatCoroutine != null) StopCoroutine(waitForBeatCoroutine);
        waitForBeatCoroutine = StartCoroutine(WaitForBeat(AudioSource.time, () => SetSegmentNow(segmentIndex)));
    }

    public void SetSegmentNow(ushort segmentIndex)
    {
        if (waitForBeatCoroutine != null) StopCoroutine(waitForBeatCoroutine);
        currentSegmentIndex = Mathf.Clamp(segmentIndex, 0, loopSegments.Length - 1);
        AudioSource.time = loopSegments[currentSegmentIndex].startTime;
    }

    public void SetTime(float time)
    {
        if (time > GetTotalTime())
            AudioSource.time = 0;
        else
            AudioSource.time = time;
    }

    private IEnumerator WaitForBeat(float startTime, Action onBeat)
    {
        float nextBeatTime = CalculateBeatTime(startTime, 1);

        while (AudioSource.time < nextBeatTime)
        {
            yield return null;
        }

        onBeat.Invoke();
    }

    public float CalculateBeatTime(float time, int beatOffset)
    {
        // Находим маркер BPM, соответствующий времени
        BpmMarker currentBpmMarker = GetBpmMarkerAtTime(time);
        float beatsPerSecond = GetBPSAtMarker(currentBpmMarker);
        float beatDuration = 1f / beatsPerSecond;
        // Вычисляем количество битов от начала сегмента BPM до текущего времени
        int currentBeatNumber = Mathf.FloorToInt((time - currentBpmMarker.Time) / beatDuration);

        // Вычисляем время следующего бита от начала сегмента BPM
        float nextBeatTime = currentBpmMarker.Time + (currentBeatNumber + beatOffset) * beatDuration;

        return nextBeatTime;
    }

    private void FadeToVolume(float targetVolume, Action onEnd = null)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeVolume(targetVolume, onEnd));
    }

    private IEnumerator FadeVolume(float targetVolume, Action onEnd)
    {
        float startVolume = AudioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }

        AudioSource.volume = targetVolume;

        fadeCoroutine = null;
        onEnd?.Invoke();
    }

    public void DebugToEndSegment()
    {
        AudioSource.time = loopSegments[currentSegmentIndex].endTime - 2;
    }

    public float GetCurrentTime()
    {
        if (AudioSource.clip == null) return 0;
        return AudioSource.time;
    }

    public float GetTotalTime()
    {
        if (AudioSource.clip == null) return 0;
        return AudioSource.clip.length;
    }

    public BpmMarker GetBpmMarkerAtTime(float time)
    {
        if (bpmMarkers.Length == 0)
        {
            return new BpmMarker(-1, -1, -1);
        }

        BpmMarker bpmMarker = bpmMarkers[0];
        foreach (var marker in bpmMarkers)
        {
            if (marker.Time <= time)
            {
                bpmMarker = marker;
            }
        }

        return bpmMarker;
    }

    public void SetTrackConfig(AudioTrackConfig trackConfig)
    {
        audioClip = trackConfig.AudioClip;
        loopSegments = trackConfig.LoopSegments;
        fadeDuration = trackConfig.FadeDuration;
        bpmMarkers = trackConfig.BpmMarkers;
        currentSegmentIndex = 0;
        Init();
        Stop(true);
    }

    private Coroutine crossfadeCoroutine;

    public void SwitchToTrackConfig(AudioTrackConfig trackConfig, bool sync = false, bool beatMatch = false,
        Action onEnd = null)
    {
        float currTime = GetCurrentTime();

        if (!AudioSource.isPlaying)
        {
            SetTrackConfig(trackConfig);
            Play();
            return;
        }

        if (beatMatch)
        {
            if (waitForBeatCoroutine != null) StopCoroutine(waitForBeatCoroutine);
            waitForBeatCoroutine = StartCoroutine(WaitForBeat(AudioSource.time,
                () => SwitchToTrackConfig(trackConfig, sync, onEnd: onEnd)));
        }

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(CrossfadeToTrack(trackConfig, onEnd));

        if (sync)
        {
            SetTime(currTime);
        }
    }

    private IEnumerator CrossfadeToTrack(AudioTrackConfig trackConfig, Action onEnd = null)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        AudioSource oldSource = AudioSource;
        AudioSource = newSource;
        SetTrackConfig(trackConfig);
        Play();
        newSource.volume = 0;
        float timer = 0f;
        while (timer < trackConfig.FadeDuration)
        {
            timer += Time.deltaTime;
            oldSource.volume = Mathf.Lerp(1, 0, timer / trackConfig.FadeDuration);
            AudioSource.volume = Mathf.Lerp(0, 1, timer / trackConfig.FadeDuration);
            if (oldSource.time >= loopSegments[currentSegmentIndex].endTime)
            {
                oldSource.time = loopSegments[currentSegmentIndex].startTime;
            }

            yield return null;
        }

        oldSource.Stop();
        Destroy(oldSource);
        onEnd?.Invoke();
    }

    public float GetBPSAtTime(float time)
    {
        BpmMarker bpmMarker = GetBpmMarkerAtTime(time);
        return GetBPSAtMarker(bpmMarker);
    }

    public float GetBPSAtMarker(BpmMarker bpmMarker)
    {
        return bpmMarker.Bpm / 60f / bpmMarker.BeatsPerBar;
    }
}
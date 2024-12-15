#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicController))]
public class BackgroundMusicPlayerEditor : Editor
{
    private const float TIMELINE_HEIGHT = 200f;
    private const float WAVEFORM_HEIGHT = 100f;
    private const int WAVEFORM_RESOLUTION = 4096;

    private MusicController musicPlayer;
    private ushort selectedSegmentIndex = 0;
    private Texture2D waveformTexture;
    private AudioClip lastAudioClip;
    private float zoomLevel = 1f;
    private float timelineOffset = 0f;
    private bool showBeats = true;
    private bool showSegments = true;

    private void OnEnable()
    {
        musicPlayer = (MusicController)target;
    }

    private void OnValidate()
    {
        musicPlayer = (MusicController)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Config"))
            SaveConfig();

        if (GUILayout.Button("Load Config"))
            LoadConfig();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Switch to config sync"))
            LoadConfig(true, true);

        if (GUILayout.Button("Switch to config"))
            LoadConfig(true);

        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();

        // --- Кнопки управления ---
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Play"))
            musicPlayer.Play();

        if (GUILayout.Button("Pause"))
            musicPlayer.Pause();

        if (GUILayout.Button("Stop"))
            musicPlayer.Stop();

        if (GUILayout.Button("Stop Now"))
            musicPlayer.Stop(true);

        if (GUILayout.Button("To End Segment"))
            musicPlayer.DebugToEndSegment();
        EditorGUILayout.EndHorizontal();

        // --- Выбор сегмента ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Segment:");
        if (musicPlayer.loopSegments.Length > 0)
            selectedSegmentIndex =
                (ushort)EditorGUILayout.Popup(selectedSegmentIndex, GetSegmentNames(musicPlayer.loopSegments));

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Set Segment"))
            musicPlayer.SetSegment(selectedSegmentIndex);

        if (GUILayout.Button("Set Segment Now"))
            musicPlayer.SetSegmentNow(selectedSegmentIndex);
        EditorGUILayout.EndHorizontal();

        // --- Таймлайн ---
        DrawTimeline();

        // --- Настройки таймлайна ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Zoom Factor:");
        zoomLevel = EditorGUILayout.Slider(zoomLevel, 1f, 20f);
        EditorGUILayout.LabelField("Timeline Offset:");
        timelineOffset = EditorGUILayout.Slider(timelineOffset, 0f, musicPlayer.GetTotalTime());
        showBeats = EditorGUILayout.Toggle("Show Beats", showBeats);
        showSegments = EditorGUILayout.Toggle("Show Segments", showSegments);
        musicPlayer.singleton = EditorGUILayout.Toggle("Singleton", musicPlayer.singleton);
    }

    private void LoadConfig(bool switchToConfig = false, bool sync = false)
    {
        string path = EditorUtility.OpenFilePanel("Load Audio Config",
            AssetDatabase.GetAssetPath(musicPlayer.audioClip), "asset");

        if (!string.IsNullOrEmpty(path))
        {
            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

            AudioTrackConfig audioConfig = AssetDatabase.LoadAssetAtPath<AudioTrackConfig>(relativePath);
            if (audioConfig != null)
            {
                if (switchToConfig)
                    musicPlayer.SwitchToTrackConfig(audioConfig, sync);
                else
                    musicPlayer.SetTrackConfig(audioConfig);
            }
            else
            {
                Debug.LogError("Failed to load audio config from " + path);
            }
        }
    }

    private void SaveConfig()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Audio Config", musicPlayer.audioClip.name, "asset",
            "Save audio config as");

        if (!string.IsNullOrEmpty(path))
        {
            AudioTrackConfig audioConfig = CreateAudioConfig();
            AssetDatabase.CreateAsset(audioConfig, path);
            AssetDatabase.SaveAssets();
        }
    }

    // --- Отрисовка таймлайна ---
    private void DrawTimeline()
    {
        EditorGUILayout.Space(15);
        Rect timelineRect = GUILayoutUtility.GetRect(100, TIMELINE_HEIGHT);

        // Отрисовка фона
        EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));

        EditorGUI.LabelField(new Rect(timelineRect.width / 2, timelineRect.y + timelineRect.height, 100, 20),
            musicPlayer.GetCurrentTime().ToString("0.00"));

        // Вычисление параметров таймлайна
        float scaledTimelineWidth = timelineRect.width * zoomLevel;
        float timelineX = timelineRect.x + timelineRect.width / 2 -
                          timelineOffset / musicPlayer.GetTotalTime() * scaledTimelineWidth;
        timelineRect.width = scaledTimelineWidth;
        timelineRect.x = timelineX;
        DragTimeline(timelineRect, Event.current);
        ZoomToMousePosition(timelineRect, Event.current);

        HandleTimelineClick(ref timelineRect);

        // Отрисовка элементов таймлайна
        DrawWaveformTexture(timelineRect);
        if (showBeats)
            DrawBeats(timelineRect);
        if (showSegments)
            DrawLoopSegments(timelineRect);
        DrawBpmMarkers(timelineRect);
        DrawPlayhead(timelineRect);
    }

    // --- Отрисовка элементов таймлайна ---

    private void DrawPlayhead(Rect timelineRect)
    {
        float playheadX = timelineRect.x +
                          timelineRect.width * (musicPlayer.GetCurrentTime() / musicPlayer.GetTotalTime());
        float playheadY = timelineRect.y;
        float playheadHeight = timelineRect.height;
        float playheadWidth = 2f;
        Rect playheadRect = new Rect(playheadX, playheadY + 5, playheadWidth, playheadHeight - 10);
        EditorGUI.DrawRect(playheadRect, Color.white);
    }

    private void DrawLoopSegments(Rect timelineRect)
    {
        float visibleTimeRange = musicPlayer.GetTotalTime();
        for (int i = 0; i < musicPlayer.loopSegments.Length; i++)
        {
            float segmentStart = musicPlayer.loopSegments[i].startTime;
            float segmentEnd = musicPlayer.loopSegments[i].endTime;
            float startX = timelineRect.x + timelineRect.width * (segmentStart) / visibleTimeRange;
            float endX = timelineRect.x + timelineRect.width * (segmentEnd) / visibleTimeRange;

            // Отрисовка начала и конца сегмента
            EditorGUI.DrawRect(new Rect(startX, timelineRect.y + 2, 2, timelineRect.height - 4), Color.yellow);
            EditorGUI.DrawRect(new Rect(endX, timelineRect.y + 2, 2, timelineRect.height - 4), Color.yellow);

            // Отображение номера сегмента
            EditorGUI.LabelField(new Rect(startX, timelineRect.y + timelineRect.height - 20, 20, 20),
                (i + 1).ToString());
        }
    }

    private void DrawBeats(Rect timelineRect)
    {
        if (musicPlayer.bpmMarkers.Length == 0)
            return;

        float visibleTimeRange = musicPlayer.GetTotalTime();
        int currentBpmMarkerIndex = 0;
        float currentBpm = musicPlayer.bpmMarkers[currentBpmMarkerIndex].Bpm;
        float currentBeatsPerBar = musicPlayer.bpmMarkers[currentBpmMarkerIndex].BeatsPerBar;
        float startTime = 0;
        float endTime = musicPlayer.bpmMarkers.Length > 1
            ? musicPlayer.bpmMarkers[currentBpmMarkerIndex + 1].Time
            : visibleTimeRange;

        while (startTime < visibleTimeRange)
        {
            float beatsPerSecond = currentBpm / 60f / currentBeatsPerBar;
            float beatDuration = 1f / beatsPerSecond;
            int totalBeats = Mathf.CeilToInt((endTime - startTime) * beatsPerSecond);

            for (int i = 0; i < totalBeats; i++)
            {
                float beatTime = startTime + i * beatDuration;
                float beatX = timelineRect.x + timelineRect.width * (beatTime) / visibleTimeRange;
                EditorGUI.DrawRect(new Rect(beatX, timelineRect.y + 2, 1, timelineRect.height - 4), Color.cyan);
            }

            currentBpmMarkerIndex++;
            if (currentBpmMarkerIndex < musicPlayer.bpmMarkers.Length)
            {
                startTime = endTime;
                currentBpm = musicPlayer.bpmMarkers[currentBpmMarkerIndex].Bpm;
                currentBeatsPerBar = musicPlayer.bpmMarkers[currentBpmMarkerIndex].BeatsPerBar;
                endTime = currentBpmMarkerIndex < musicPlayer.bpmMarkers.Length - 1
                    ? musicPlayer.bpmMarkers[currentBpmMarkerIndex + 1].Time
                    : visibleTimeRange;
            }
            else
            {
                break;
            }
        }
    }

    private void DrawWaveformTexture(Rect timelineRect)
    {
        if (musicPlayer.audioClip == null)
            return;

        if (lastAudioClip != musicPlayer.audioClip || waveformTexture == null)
        {
            lastAudioClip = musicPlayer.audioClip;
            waveformTexture = GetWaveform(musicPlayer.audioClip);
        }

        GUI.DrawTexture(timelineRect, waveformTexture);
    }

    private void DrawBpmMarkers(Rect timelineRect)
    {
        if (musicPlayer.bpmMarkers.Length == 0)
            return;

        float totalTime = musicPlayer.GetTotalTime();
        for (int i = 0; i < musicPlayer.bpmMarkers.Length; i++)
        {
            var bpmMarker = musicPlayer.bpmMarkers[i];
            float markerX = timelineRect.x + timelineRect.width * (bpmMarker.Time / totalTime);

            // Рисование маркера
            Rect markerRect = new Rect(markerX - 2, timelineRect.y, 3, timelineRect.height);
            EditorGUI.DrawRect(markerRect, Color.red);

            // Отображение информации о маркере
            EditorGUI.LabelField(new Rect(markerX, timelineRect.y - 15, 200, 20),
                $"BPM: {bpmMarker.Bpm}, Beats per bar: {bpmMarker.BeatsPerBar}");
        }
    }

    // --- Вспомогательные методы ---

    private AudioTrackConfig CreateAudioConfig()
    {
        var audioTrackConfig = ScriptableObject.CreateInstance<AudioTrackConfig>();
        audioTrackConfig.SetData(musicPlayer.audioClip, musicPlayer.loopSegments, musicPlayer.fadeDuration,
            musicPlayer.bpmMarkers);
        return audioTrackConfig;
    }

    private string[] GetSegmentNames(MusicController.LoopSegment[] loopSegments)
    {
        string[] segmentNames = new string[loopSegments.Length];
        for (int i = 0; i < loopSegments.Length; i++)
        {
            segmentNames[i] = "Segment " + (i + 1);
        }

        return segmentNames;
    }

    private Texture2D GetWaveform(AudioClip clip)
    {
        int height = (int)WAVEFORM_HEIGHT;
        int width = WAVEFORM_RESOLUTION;
        int halfheight = height / 2;
        float heightscale = (float)height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] waveform = new float[width];
        int samplesize = clip.samples * clip.channels;
        float[] samples = new float[samplesize];
        clip.GetData(samples, 0);
        int packsize = (samplesize / width);

        for (int w = 0; w < width; w++)
        {
            waveform[w] = Mathf.Abs(samples[w * packsize]);
        }

        Color background = new Color(0.14f, 0.14f, 0.14f);
        Color foreground = new Color(1f, 0.55f, 0f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, background);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < waveform[x] * heightscale; y++)
            {
                tex.SetPixel(x, halfheight + y, foreground);
                tex.SetPixel(x, halfheight - y, foreground);
            }
        }

        tex.Apply();
        return tex;
    }

    // --- Обработка событий ---

    private bool draggingTimeline;
    private Vector2 dragStartMousePosition;
    private float dragStartTimelineOffset;

    private void HandleTimelineClick(ref Rect timelineRect)
    {
        Event currentEvent = Event.current;
        if (!timelineRect.Contains(currentEvent.mousePosition))
            return;

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            SetTime(timelineRect, currentEvent);
        }

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
        {
            ShowContextMenu(timelineRect, currentEvent);
        }
    }

    private void DragTimeline(Rect timelineRect, Event currentEvent)
    {
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 2)
        {
            draggingTimeline = true;
            dragStartMousePosition = currentEvent.mousePosition;
            dragStartTimelineOffset = timelineOffset;
            currentEvent.Use();
        }

        if (currentEvent.type == EventType.MouseUp && currentEvent.button == 2)
        {
            draggingTimeline = false;
            currentEvent.Use();
        }

        if (draggingTimeline && currentEvent.type == EventType.MouseDrag && currentEvent.button == 2)
        {
            float delta = currentEvent.mousePosition.x - dragStartMousePosition.x;
            timelineOffset = dragStartTimelineOffset - delta * musicPlayer.GetTotalTime() / timelineRect.width;
            currentEvent.Use();
        }
    }

    private void SetTime(Rect timelineRect, Event currentEvent)
    {
        float clickX = currentEvent.mousePosition.x - timelineRect.x;
        float newTime = (clickX / timelineRect.width) * musicPlayer.GetTotalTime();
        musicPlayer.SetTime(newTime);
        currentEvent.Use();
    }

    private void ShowContextMenu(Rect timelineRect, Event currentEvent)
    {
        float clickTime = GetTimeFromClick(timelineRect, currentEvent);

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Set Start Time"), false,
            () => SetSegmentStartTime(clickTime));
        menu.AddItem(new GUIContent("Set End Time"), false,
            () => SetSegmentEndTime(clickTime));
        menu.ShowAsContext();
        currentEvent.Use();
    }

    private float GetTimeFromClick(Rect timelineRect, Event currentEvent)
    {
        float clickX = currentEvent.mousePosition.x - timelineRect.x;
        return (clickX / timelineRect.width) * musicPlayer.GetTotalTime();
    }

    private float CalculateNearestBeatTime(float time)
    {
        MusicController.BpmMarker bpmMarker = musicPlayer.GetBpmMarkerAtTime(time);
        if (bpmMarker.Time < 0) return time;
        float beatsPerSecond = musicPlayer.GetBPSAtMarker(bpmMarker);
        float beatDuration = 1f / beatsPerSecond;
        int nearestBeatIndex = Mathf.RoundToInt(time * beatsPerSecond);
        float startTime = bpmMarker.Time % beatDuration;
        return startTime + nearestBeatIndex * beatDuration;
    }

    private void SetSegmentStartTime(float time)
    {
        float beatTime = CalculateNearestBeatTime(time);
        Undo.RecordObject(musicPlayer, "Set Start Time");
        if (musicPlayer.loopSegments.Length == 0)
        {
            ArrayUtility.Add(ref musicPlayer.loopSegments,
                new MusicController.LoopSegment(beatTime, musicPlayer.audioClip.length - 0.05f));
            return;
        }

        musicPlayer.loopSegments[selectedSegmentIndex] = new MusicController.LoopSegment(beatTime,
            musicPlayer.loopSegments[selectedSegmentIndex].endTime);
        EditorUtility.SetDirty(musicPlayer);
    }

    private void SetSegmentEndTime(float time)
    {
        float beatTime = CalculateNearestBeatTime(time);
        Undo.RecordObject(musicPlayer, "Set End Time");
        if (musicPlayer.loopSegments.Length == 0)
        {
            ArrayUtility.Add(ref musicPlayer.loopSegments,
                new MusicController.LoopSegment(0, beatTime));
            return;
        }

        musicPlayer.loopSegments[selectedSegmentIndex] =
            new MusicController.LoopSegment(musicPlayer.loopSegments[selectedSegmentIndex].startTime, beatTime);
        EditorUtility.SetDirty(musicPlayer);
    }

    private void ZoomToMousePosition(Rect timelineRect, Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel && currentEvent.control)
        {
            float mousePositionX = currentEvent.mousePosition.x - timelineRect.x;
            float mousePositionRatio = mousePositionX / timelineRect.width;
            timelineRect.x += mousePositionRatio;
            float delta = currentEvent.delta.y * 0.3f;
            zoomLevel -= delta;
            currentEvent.Use();
        }
    }

    private void ZoomTimeline(Event currentEvent)
    {
        float delta = currentEvent.delta.y * 0.3f;
        zoomLevel -= delta;
        currentEvent.Use();
    }

    // --- Добавление и удаление BPM маркеров ---

    private void AddBpmMarker(float time)
    {
        Undo.RecordObject(musicPlayer, "Add BPM Marker");
        ArrayUtility.Add(ref musicPlayer.bpmMarkers, new MusicController.BpmMarker(time, 120f, 4));
        EditorUtility.SetDirty(musicPlayer);
    }

    private void RemoveBpmMarker(int index)
    {
        Undo.RecordObject(musicPlayer, "Remove BPM Marker");
        ArrayUtility.RemoveAt(ref musicPlayer.bpmMarkers, index);
        EditorUtility.SetDirty(musicPlayer);
    }

    public override bool RequiresConstantRepaint()
    {
        return musicPlayer.AudioSource.isPlaying;
    }
}

#endif
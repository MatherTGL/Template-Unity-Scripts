using UnityEngine;
using System;
using Sirenix.OdinInspector;

public sealed class AccelerationTime : MonoBehaviour
{
    [ShowInInspector, BoxGroup("ReadOnly"), ReadOnly]
    private static bool _isPause = false;
    public static bool IsPause => _isPause;

    [EnumToggleButtons]
    private enum AvailableAccelerations { Pause = 0, X1 = 1 }

    [SerializeField, BoxGroup("ReadOnly"), ReadOnly, Space(5)]
    private AvailableAccelerations _availableAccelerations;

    public static Action OnPause;


    private void OnEnable() => OnPause += Pause;
    private void OnDisable() => OnPause -= Pause;

    public static void SetPause() => OnPause.Invoke();

    private void Pause() => _isPause = !_isPause;
}
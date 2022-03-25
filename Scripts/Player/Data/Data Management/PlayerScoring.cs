using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Data.Player;

public sealed class PlayerScoring : MonoBehaviour
{
    [ShowInInspector, BoxGroup("Setting's/ReadOnly"), ReadOnly]
    private const byte _scoringTime = 1;

    [SerializeField, BoxGroup("Setting's"), MinValue(1)]
    private byte _pointsGivenPerSecond;


    private void Start() => StartCoroutine(Scoring());

    private IEnumerator Scoring()
    {
        while (true)
        {
            yield return new WaitForSeconds(_scoringTime);
            
            if (AccelerationTime.IsPause != true)
                PlayerData.AddScore(_pointsGivenPerSecond);
        }
    }
}

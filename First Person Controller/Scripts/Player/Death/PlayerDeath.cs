using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data.Player;

public sealed class PlayerDeath : MonoBehaviour
{
    public static Action OnDie;


    private void Awake()
    {
        OnDie += Die;
        OnDie += PlayerData.ClearScore;
    }

    private void Die() => SceneManager.LoadScene(0);
}

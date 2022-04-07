using System;

public sealed class PlayerLosing
{
    public static Action OnLose;


    private static void Lose() => OnLose.Invoke();
}

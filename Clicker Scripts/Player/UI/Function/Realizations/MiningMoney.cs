using System;

public sealed class MiningMoney
{
    public static Action OnMining;

    public static void MoneyMining()
    {
        PlayerData.AddMoney(PlayerData.IncomeMoneyPerClick);
        OnMining.Invoke();
    }
}

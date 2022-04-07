public class PlayerData
{
    private static double playerMoney = 1000d;
    public static double PlayerMoney => playerMoney;

    private static double playerResearchPoint;
    public static double PlayerResearchPoint => playerResearchPoint;

    private static double incomeMoneyPerClick = 1d;
    public static double IncomeMoneyPerClick => incomeMoneyPerClick;


    #region Player Money
    public static void AddMoney(in double amount)
    {
        playerMoney += amount;
    }

    public static void SubtractMoney(in double amount)
    {
        playerMoney -= amount;
    }
    #endregion

    #region Player RP
    public static void AddResearchPoint(in double amount)
    {
        playerResearchPoint += amount;
    }

    public static void SubtractResearchPoint(in double amount)
    {
        playerResearchPoint -= amount;
    }
    #endregion
}

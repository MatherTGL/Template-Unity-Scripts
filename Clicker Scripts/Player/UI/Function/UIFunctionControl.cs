using UnityEngine;

[RequireComponent(typeof(UIAllTextDisplayResources))]
public class UIFunctionControl : MonoBehaviour
{
    public void MoneyMiningButton() => MiningMoney.MoneyMining();

    public void ShowPanel(GameObject desiredObject) => UIAllPanels.ShowPanel(desiredObject);
}

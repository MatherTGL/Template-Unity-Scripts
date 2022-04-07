using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIAllTextDisplayResources : MonoBehaviour
{
    private void OnEnable()
    {
        MiningMoney.OnMining += UpdateMoneyText;
        ShopProduct.OnBuyProduct += UpdateMoneyText;
    }

    private void OnDisable()
    {
        MiningMoney.OnMining -= UpdateMoneyText;
        ShopProduct.OnBuyProduct -= UpdateMoneyText;
    }

    private void UpdateMoneyText()
    {
        for (int i = 0; i < textDisplayMoney.Length; i++)
        {
            textDisplayMoney[i].text = $"$ {PlayerData.PlayerMoney.ToString("#.##")}";
        }
    }


    [SerializeField, BoxGroup("Settings"), Required]
    private Text[] textDisplayMoney;
}

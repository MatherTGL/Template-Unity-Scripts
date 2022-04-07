using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Product", menuName = "Product")]
public sealed class TemplateProduct : ScriptableObject
{
    [SerializeField, BoxGroup("Settings")]
    private double costProduct;

    [SerializeField, BoxGroup("Settings")]
    private string nameProduct;

    private enum PaymentType { Money, RP }
    private PaymentType paymentType;

    [SerializeField, BoxGroup("Settings"), MinValue(0.01f), MaxValue(1.0f)]
    private float percentIncreaseCost;

    private const float maxPercent = 100f;


    private void IncreaseCost()
    {
        costProduct += percentIncreaseCost * maxPercent;
    }

    public void Buy()
    {
        if (paymentType is PaymentType.Money)
            PlayerData.SubtractMoney(costProduct);
        else
            PlayerData.SubtractResearchPoint(costProduct);

        IncreaseCost();
    }
}

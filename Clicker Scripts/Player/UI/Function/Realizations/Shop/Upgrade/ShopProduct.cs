using UnityEngine;
using System;
using Sirenix.OdinInspector;

public sealed class ShopProduct : MonoBehaviour
{
    public static Action OnBuyProduct;


    public void BuyProduct()
    {
        templateProduct.Buy();
        
        OnBuyProduct.Invoke();
    }


    [SerializeField, BoxGroup("Settings"), Required]
    private TemplateProduct templateProduct;
}

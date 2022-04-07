using UnityEngine;

public static class UIAllPanels
{
    public static void ShowPanel(GameObject desiredObject)
    {
        desiredObject.SetActive(!desiredObject.activeSelf);
    }
}

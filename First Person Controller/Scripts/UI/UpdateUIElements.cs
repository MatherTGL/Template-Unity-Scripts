using UnityEngine;

namespace UIGame
{
    public sealed class UpdateUIElements : MonoBehaviour
    {
        private static UIScoreDisplay _uiScoreDisplay;
        public static UIScoreDisplay UIScoreDisplay => _uiScoreDisplay;


        private void Awake()
        {
            _uiScoreDisplay = FindObjectOfType<UIScoreDisplay>();
        }
    }
}
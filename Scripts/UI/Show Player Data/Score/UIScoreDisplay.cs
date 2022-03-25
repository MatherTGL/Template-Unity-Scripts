using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace UIGame
{
    public sealed class UIScoreDisplay : MonoBehaviour
    {
        public void UpdateScoreText(in ushort currentScore)
        {
            _textCurrentScore.text = $"Score {currentScore}";
        }


        [SerializeField, BoxGroup("Setting's"), Required]
        private Text _textCurrentScore;
    }
}
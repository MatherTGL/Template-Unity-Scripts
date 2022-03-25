using UnityEngine;
using Sirenix.OdinInspector;

namespace UIGame.PauseMenu
{
    public sealed class UIPauseMenuDisplay : MonoBehaviour
    {
        private void OnEnable()
        {
            AccelerationTime.OnPause += ActivatePanel;
        }

        private void OnDisable()
        {
            AccelerationTime.OnPause -= ActivatePanel;
        }

        private void ActivatePanel()
        {
            _pausePanel.SetActive(!_pausePanel.activeSelf);
        }

        public void ContinueButton() => AccelerationTime.SetPause();

        public void ExitButton() => Application.Quit();


        [SerializeField, BoxGroup("Setting's"), Required]
        private GameObject _pausePanel;
    }
}
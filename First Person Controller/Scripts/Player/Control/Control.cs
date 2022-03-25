using UnityEngine;
using Sirenix.OdinInspector;
using UIGame;
using UIGame.PauseMenu;
using EventColliders;

namespace ControlPlayer
{
    #region RequireComponent
    [RequireComponent(typeof(EnemyCollisionCheck))]
    [RequireComponent(typeof(PlayerDeath))]
    [RequireComponent(typeof(PlayerScoring))]
    [RequireComponent(typeof(UIScoreDisplay))]
    [RequireComponent(typeof(UpdateUIElements))]
    [RequireComponent(typeof(AccelerationTime))]
    [RequireComponent(typeof(UIPauseMenuDisplay))]
    [RequireComponent(typeof(Rigidbody))]
    #endregion
    
    public sealed class Control : MonoBehaviour
    {
        [EnumToggleButtons]
        private enum TypeControl { Player }

        [SerializeField, BoxGroup("Setting's")]
        private TypeControl _typeControl;


        private void Awake()
        {
            if (_typeControl == TypeControl.Player)
                _IControl = new PlayerControl();
        }

        public void CheckInputMove()
        {
            if (AccelerationTime.IsPause != true)
                _IControl.GravityСhange();
        }

        private IControl _IControl;
    }
}
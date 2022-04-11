using UnityEngine;
using Sirenix.OdinInspector;

namespace Player_Control
{
    [RequireComponent(typeof(InputControl))]
    [RequireComponent(typeof(RaycastZoomPath))]
    public sealed class PlayerControl : MonoBehaviour
    {
        [TitleGroup("General", "Settings", Indent = true)]
        [SerializeField, MinValue(0), LabelText("Move"), Space(2), TitleGroup("General/FirstGroup/Speed"), BoxGroup("General/FirstGroup", false)]
        private float speedMove;

        [SerializeField, MinValue(0), LabelText("Rotate"), Space(2), TitleGroup("General/FirstGroup/Speed"), BoxGroup("General/FirstGroup", false)]
        private float speedRotate;

        [SerializeField, MinValue(0), LabelText("Zoom"), Space(2), TitleGroup("General/FirstGroup/Speed"), BoxGroup("General/FirstGroup", false)]
        private float speedZoom;

        [EnumToggleButtons]
        private enum MovementType { Space, Location }

        [TitleGroup("General/SecondGroup/Bools", Indent = true)]
        [SerializeField, Space(2), HideLabel, TitleGroup("General/SecondGroup/Bools/MovementType"), BoxGroup("General/SecondGroup", false)]
        private MovementType movementType;


        private void Awake()
        {
            playerControlSettings = new PlayerControlSettings();

            if (movementType is MovementType.Location)
                Icontrol = new ControlLocation();
        }

        private void Start()
        {
            playerControlSettings.InitializationParameters(speedMove, speedRotate, gameObject.transform, speedZoom,
                                                           cameraPlayer.gameObject.transform, cameraPlayer);
        }

        private void FixedUpdate()
        {
            Icontrol.Move();
            Icontrol.Zoom();
            Icontrol.Rotate();
        }


#if UNITY_EDITOR
        [SerializeField, TitleGroup("Additional/ThirdGroup/Components"), BoxGroup("Additional/ThirdGroup", false), ToggleLeft]
        private bool isEdit;
#endif

        [TitleGroup("Additional", "Settings", Indent = true), BoxGroup("Additional/ThirdGroup", false), EnableIf("isEdit")]
        [SerializeField, Required, SceneObjectsOnly, TitleGroup("Additional/ThirdGroup/Components"), Space(2)]
        [Title("Camera"), Indent(1), HideLabel]
        private Camera cameraPlayer;

        private IControl Icontrol;

        private static PlayerControlSettings playerControlSettings;
        public static PlayerControlSettings PlayerControlSettings => playerControlSettings;
    }

    public struct PlayerControlSettings
    {
        private float speedMove;
        public float SpeedMove => speedMove;

        private float speedRotate;
        public float SpeedRotate => speedRotate;

        private float speedZoom;
        public float SpeedZoom => speedZoom;


        public void InitializationParameters(in float speedMove, in float speedRotate, in Transform mainPlayerObject,
                                             in float speedZoom, in Transform playerCameraTransform, in Camera camera)
        {
            this.speedMove = speedMove;
            this.speedRotate = speedRotate;
            this.speedZoom = speedZoom;
            this.playerCameraTransform = playerCameraTransform;
            this.mainPlayerObject = mainPlayerObject;
            playerCamera = camera;
        }

        private Transform mainPlayerObject;
        public Transform MainPlayerObject => mainPlayerObject;

        private Transform playerCameraTransform;
        public Transform PlayerCameraTransform => playerCameraTransform;

        private Camera playerCamera;
        public Camera PlayerCamera => playerCamera;
    }
}
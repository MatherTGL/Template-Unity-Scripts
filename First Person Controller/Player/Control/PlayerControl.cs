using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(InputControl))]
public sealed class PlayerControl : MonoBehaviour
{
    [SerializeField, BoxGroup("Setting's"), MinValue(0)]
    private float speedMove;

    [SerializeField, BoxGroup("Setting's"), MinValue(0), MaxValue(100)]
    private byte mouseSensitivity;

    [SerializeField, BoxGroup("Setting's"), MinValue(0)]
    private float forceJump;

    [SerializeField, BoxGroup("Setting's"), MinValue(0), MaxValue(100)]
    private float limitViewVertically;

    [EnumToggleButtons]
    private enum MovementType { FirstPerson, Garage, Space }
    
    [SerializeField, BoxGroup("Setting's"), Space(10)]
    private MovementType movementType;


    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rigidbody = GetComponent<Rigidbody>();

        if (movementType == MovementType.FirstPerson)
            IplayerControl = new PlayerControlFirstPerson();
    }

    private void FixedUpdate() {
        IplayerControl.Move(rigidbody, speedMove);
        IplayerControl.MouseLook(playerCamera.transform, gameObject.transform, mouseSensitivity, limitViewVertically);

        if (Input.GetKeyDown(InputControl.KeycodeSpace))
            IplayerControl.Jump(rigidbody, forceJump);
    }
    

    private IPlayerControl IplayerControl;

    [SerializeField, BoxGroup("Setting's/Component's"), Required, Space(10)]
    private new Rigidbody rigidbody;

    [SerializeField, BoxGroup("Setting's/Component's"), Required]
    private Camera playerCamera;
}

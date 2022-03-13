using Sirenix.OdinInspector;
using UnityEngine;

public sealed class InputControl : MonoBehaviour
{
    private const float maxMouseAcceleration = 25f;

    [ShowInInspector, ReadOnly, BoxGroup("ReadOnly")]
    private static float axisHorizontalMove;
    public static float AxisHorizontalMove => axisHorizontalMove;

    [ShowInInspector, ReadOnly, BoxGroup("ReadOnly")]
    private static float axisVerticalMove;
    public static float AxisVerticalMove => axisVerticalMove;
    
    [ShowInInspector, ReadOnly, BoxGroup("ReadOnly")]
    private static float axisMouseHorizontal;
    public static float AxisMouseHorizontal => axisMouseHorizontal;

    [ShowInInspector, ReadOnly, BoxGroup("ReadOnly")]
    private static float axisMouseVertical;
    public static float AxisMouseVertical => axisMouseVertical;

    private static KeyCode keycodeSpace = KeyCode.Space;
    public static KeyCode KeycodeSpace => keycodeSpace;


    private void Update(){
        axisHorizontalMove = Input.GetAxisRaw("Horizontal");
        axisVerticalMove = Input.GetAxisRaw("Vertical");
        axisMouseHorizontal = Mathf.Clamp(Input.GetAxis("Mouse X"), -maxMouseAcceleration, maxMouseAcceleration); 
        axisMouseVertical = Mathf.Clamp(Input.GetAxis("Mouse Y"), -maxMouseAcceleration, maxMouseAcceleration); 
    }
}

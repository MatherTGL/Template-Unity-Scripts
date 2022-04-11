using Sirenix.OdinInspector;
using UnityEngine;

public sealed class InputControl : MonoBehaviour
{
    [ShowInInspector, TitleGroup("General", "Axis"), ReadOnly, BoxGroup("General/ReadOnly"), Indent(1)]
    [LabelText("Vertical Move")]
    private static float axisVerticalMove;
    public static float AxisVerticalMove => axisVerticalMove;

    [ShowInInspector, TitleGroup("General", "ReadOnly"), ReadOnly, BoxGroup("General/ReadOnly"), Indent(1)]
    [LabelText("Horizontal Move")]
    private static float axisHorizontalMove;
    public static float AxisHorizontalMove => axisHorizontalMove;


    [ShowInInspector, TitleGroup("General"), ReadOnly, BoxGroup("General/ReadOnly"), Indent(1)]
    [LabelText("Mouse Scroll")]
    private static float axisMouseScroll;
    public static float AxisMouseScroll => axisMouseScroll;


    [ShowInInspector, TitleGroup("General", "ReadOnly"), ReadOnly, BoxGroup("General/ReadOnly"), Indent(1)]
    [LabelText("Rotate")]
    private static float axisRotate;
    public static float AxisRotate => axisRotate;


    private void Update()
    {
        axisHorizontalMove = Input.GetAxisRaw("Horizontal");
        axisVerticalMove = Input.GetAxisRaw("Vertical");
        axisMouseScroll = Input.GetAxisRaw("Mouse ScrollWheel");
        axisRotate = Input.GetAxisRaw("Rotate");
    }
}

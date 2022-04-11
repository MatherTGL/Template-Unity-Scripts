using Player_Control;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class RaycastZoomPath : MonoBehaviour
{
    [TitleGroup("General"), SerializeField, MinValue(10), BoxGroup("General/Settings", false), Indent(1)]
    private short distanceRaycast;

    [SerializeField, FoldoutGroup("General/Settings/Zoom"), Indent(1), LabelText("Offset"), MinValue(3)]
    private float minZoomOffset;

    [SerializeField, FoldoutGroup("General/Settings/Zoom"), Indent(1), LabelText("Offset"), MinValue(3)]
    private float maxZoomOffset;

    [SerializeField, FoldoutGroup("General/Settings/Zoom"), Indent(1), LabelText("Min"), ReadOnly]
    private short minDistanceZoom;

    [SerializeField, FoldoutGroup("General/Settings/Zoom"), Indent(1), LabelText("Max"), ReadOnly]
    private short maxDistanceZoom;


#if UNITY_EDITOR
    #region Additional

    [TitleGroup("Additional"), BoxGroup("Additional/Settings")]
    [Title("Sphere Radius", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered, Bold = false)]
    [SerializeField, Range(0.05f, 1f), HideLabel]
    private float sphereRadius;

    [SerializeField, Title("Debug Color", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered, Bold = false)]
    [BoxGroup("Additional/Settings", false), Space(5), ColorPalette("Fall"), HideLabel]
    private Color color;
    #endregion

    #region Debug

    [TitleGroup("Debug"), EnumToggleButtons]
    private enum ActiveDebug { Active, Deactive }

    [SerializeField, BoxGroup("Debug/Group", false), HideLabel]
    private ActiveDebug activeDebug;

    [SerializeField, BoxGroup("Debug/Group", false), ReadOnly, ShowIf("activeDebug", ActiveDebug.Active)]
    private sbyte lenghtRaycast;

    #endregion
#endif


    private void Awake()
    {
        raycastSettings = new RaycastSettings();
    }

    private void FixedUpdate()
    {
        raycast = new Ray(PlayerControl.PlayerControlSettings.PlayerCameraTransform.position, PlayerControl.PlayerControlSettings.PlayerCameraTransform.forward);

        if (Physics.Raycast(raycast, out rayHit, distanceRaycast))
        {
            minDistanceZoom = (short)(rayHit.point.y + minZoomOffset);
            maxDistanceZoom = (short)(rayHit.point.y + maxZoomOffset);

            raycastSettings.UpdateParameters(minDistanceZoom, maxDistanceZoom, rayHit);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (activeDebug == ActiveDebug.Active) Gizmos.DrawRay(raycast);

        Gizmos.DrawSphere(rayHit.point, sphereRadius);
    }
#endif


    private Ray raycast;

    private RaycastHit rayHit;

    private static RaycastSettings raycastSettings;
    public static RaycastSettings RaycastSettings => raycastSettings;
}

public struct RaycastSettings
{
    private float minDistanceZoom;
    public float MinDistanceZoom => minDistanceZoom;

    private float maxDistanceZoom;
    public float MaxDistanceZoom => maxDistanceZoom;


    public void UpdateParameters(in float minDistanceZoom, in float maxDistanceZoom, in RaycastHit hit)
    {
        this.minDistanceZoom = minDistanceZoom;
        this.maxDistanceZoom = maxDistanceZoom;
        rayHit = hit;
    }


    private RaycastHit rayHit;
    public RaycastHit RayHit => rayHit;
}

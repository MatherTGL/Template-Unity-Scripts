using UnityEngine;

namespace Player_Control
{
    public sealed class ControlLocation : IControl
    {
        private float expectedZoom;
        private Vector3 directionMove;


        public void Move()
        {
            directionMove += new Vector3(InputControl.AxisHorizontalMove, 0f, InputControl.AxisVerticalMove) * PlayerControl.PlayerControlSettings.SpeedMove * Time.deltaTime;

            PlayerControl.PlayerControlSettings.PlayerCameraTransform.localPosition = Vector3.Lerp(PlayerControl.PlayerControlSettings.PlayerCameraTransform.localPosition,
                                                                                               directionMove,
                                                                                               0.05f);
        }

        public void Rotate()
        {
            

        }

        public void Zoom()
        {
            expectedZoom -= InputControl.AxisMouseScroll * PlayerControl.PlayerControlSettings.SpeedZoom;

            expectedZoom = Mathf.Clamp(expectedZoom, RaycastZoomPath.RaycastSettings.RayHit.point.y + RaycastZoomPath.RaycastSettings.MinDistanceZoom, RaycastZoomPath.RaycastSettings.RayHit.point.y + RaycastZoomPath.RaycastSettings.MaxDistanceZoom);
            
            PlayerControl.PlayerControlSettings.PlayerCamera.orthographicSize = Mathf.Lerp(PlayerControl.PlayerControlSettings.PlayerCamera.orthographicSize, expectedZoom, 0.02f);
        }
    }
}
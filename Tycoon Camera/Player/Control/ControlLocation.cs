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
            Vector3 directionRotate = new Vector3(0f, InputControl.AxisRotate, 0f) * PlayerControl.PlayerControlSettings.SpeedMove * Time.deltaTime;

            PlayerControl.PlayerControlSettings.PlayerCameraTransform.RotateAround(RaycastZoomPath.RaycastSettings.RayHit.point, directionRotate, 1f);

        }

        public void Zoom()
        {
            expectedZoom -= InputControl.AxisMouseScroll * PlayerControl.PlayerControlSettings.SpeedZoom;

            expectedZoom = Mathf.Clamp(expectedZoom, RaycastZoomPath.RaycastSettings.RayHit.point.y + RaycastZoomPath.RaycastSettings.MinDistanceZoom, RaycastZoomPath.RaycastSettings.RayHit.point.y + RaycastZoomPath.RaycastSettings.MaxDistanceZoom);
            
            PlayerControl.PlayerControlSettings.PlayerCamera.orthographicSize = Mathf.Lerp(PlayerControl.PlayerControlSettings.PlayerCamera.orthographicSize, expectedZoom, 0.02f);
        }
    }
}
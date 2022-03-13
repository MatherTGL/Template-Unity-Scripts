using UnityEngine;

public interface IPlayerControl
{
    void Move(in Rigidbody rigidbody, in float speedMove);
    void MouseLook(in Transform cameraRotation, in Transform player, in byte mouseSensitivity, in float limitViewVertically);
    void Jump(in Rigidbody rigidbody, in float forceJump);
    void Zoom();
    void Rotation();
}

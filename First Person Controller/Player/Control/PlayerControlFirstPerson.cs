using UnityEngine;

public sealed class PlayerControlFirstPerson : IPlayerControl
{
    private float cameraRotationX;


    public void Move(in Rigidbody rigidbody, in float speedMove){
        Vector3 directionMove = new Vector3(InputControl.AxisHorizontalMove, 0f, InputControl.AxisVerticalMove);
        rigidbody.AddRelativeForce(directionMove * speedMove * Time.deltaTime, ForceMode.VelocityChange);
    }

    public void MouseLook(in Transform cameraRotation, in Transform player, in byte mouseSensitivity, in float limitViewVertically){
        cameraRotationX += InputControl.AxisMouseVertical * mouseSensitivity * Time.deltaTime;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -limitViewVertically, limitViewVertically);
        cameraRotation.localEulerAngles = new Vector3(cameraRotationX, 0f, 0f);
        player.eulerAngles += new Vector3(0, InputControl.AxisMouseHorizontal * mouseSensitivity * Time.deltaTime, 0f);
    }

    public void Jump(in Rigidbody rigidbody, in float forceJump){
        rigidbody.AddForce(Vector3.up * forceJump, ForceMode.VelocityChange);
    }

    public void Rotation()
    {
        throw new System.NotImplementedException();
    }

    public void Zoom()
    {
        throw new System.NotImplementedException();
    }
}

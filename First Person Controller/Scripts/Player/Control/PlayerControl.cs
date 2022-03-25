using UnityEngine;
using Data.Physics;

namespace ControlPlayer
{
    public sealed class PlayerControl : IControl
    {
        public void GravityСhange()
        {
            if (PlayerPhysicsData.IsBottomGravity)
                Physics.gravity = new Vector3(0, PlayerPhysicsData.GravityForce, 0);
            else
                Physics.gravity = new Vector3(0, -PlayerPhysicsData.GravityForce, 0);

            PlayerPhysicsData.IsBottomGravity = !PlayerPhysicsData.IsBottomGravity;
        }
    }
}
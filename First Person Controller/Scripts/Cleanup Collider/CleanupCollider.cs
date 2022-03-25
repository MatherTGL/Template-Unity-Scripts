using UnityEngine;

namespace EventColliders
{
    public sealed class CleanupCollider : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            Destroy(other.gameObject);
        }
    }
}
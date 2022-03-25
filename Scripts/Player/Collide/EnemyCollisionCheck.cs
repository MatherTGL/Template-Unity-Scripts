using UnityEngine;

namespace EventColliders
{
    public sealed class EnemyCollisionCheck : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.layer == 6)
                PlayerDeath.OnDie.Invoke();
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyParameters : MonoBehaviour
    {
        [SerializeField, BoxGroup("Setting's"), MinValue(1)]
        private float _speedMove;

        [SerializeField, BoxGroup("Setting's"), Space(5)]
        private Vector3 _directionMove;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (AccelerationTime.IsPause != true)
                _rigidbody.AddForce(_directionMove * _speedMove * Time.deltaTime, ForceMode.VelocityChange);
            else
                _rigidbody.velocity = Vector3.zero;
        }


        [SerializeField, ReadOnly, BoxGroup("Setting's"), Space(5)]
        private Rigidbody _rigidbody;
    }
}
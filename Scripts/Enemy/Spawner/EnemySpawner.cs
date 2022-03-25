using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField, BoxGroup("Setting's")]
        private float _minRandomTimeSpawn;

        [SerializeField, BoxGroup("Setting's")]
        private float _maxRandomTimeSpawn;


        private void Awake() => StartCoroutine(Spawning());

        private IEnumerator Spawning()
        {
            while (true)
            {
                if (AccelerationTime.IsPause != true)
                {
                    Vector3 _spawnPosition = new Vector3(transform.position.x,
                                                                         transform.position.y,
                                                                         0f);

                    Instantiate(_prefabEnemy, _spawnPosition, Quaternion.identity, _parentSpawnedObjects);
                }

                yield return new WaitForSeconds(Random.Range(_minRandomTimeSpawn, _maxRandomTimeSpawn));
            }
        }


        [SerializeField, BoxGroup("Setting's"), Required, Space(10)]
        private EnemyParameters _prefabEnemy;

        [SerializeField, BoxGroup("Setting's"), Required, Space(3)]
        private Transform _parentSpawnedObjects;
    }
}
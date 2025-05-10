using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject _platform;
    public GameObject _spike;
    public GameObject _breakable;
    public GameObject _movingPlatform;

    public float _platformSpawnTimer = 2f;
    private float _currentPlatformSpawnTimer;

    private int _platformSpawnCount;
    private bool _lastWasSpike; // Track if the last platform was a spike

    public float _minX = -2.5f, _maxX = 2.5f;

    private void Start()
    {
        _currentPlatformSpawnTimer = _platformSpawnTimer;
        _lastWasSpike = false;
    }

    private void Update()
    {
        SpawnPlatforms();
    }

    void SpawnPlatforms()
    {
        _currentPlatformSpawnTimer -= Time.deltaTime;

        if (_currentPlatformSpawnTimer <= 0)
        {
            _platformSpawnCount++;

            Vector3 temp = transform.position;
            temp.x = Random.Range(_minX, _maxX);

            GameObject newPlatform = null;

            if (_platformSpawnCount < 2)
            {
                newPlatform = Instantiate(_platform, temp, Quaternion.identity);
                _lastWasSpike = false; // First two platforms are always normal
            }
            else
            {
                int rand;

                // Prevent consecutive spikes
                if (_lastWasSpike)
                {
                    List<int> allowedTypes = new List<int> { 0, 2, 3 }; // 0: normal, 2: breakable, 3: moving
                    rand = allowedTypes[Random.Range(0, allowedTypes.Count)];
                }
                else
                {
                    rand = Random.Range(0, 4); // All types allowed
                }

                switch (rand)
                {
                    case 0:
                        newPlatform = Instantiate(_platform, temp, Quaternion.identity);
                        break;
                    case 1:
                        newPlatform = Instantiate(_spike, temp, Quaternion.identity);
                        break;
                    case 2:
                        newPlatform = Instantiate(_breakable, temp, Quaternion.identity);
                        break;
                    case 3:
                        newPlatform = Instantiate(_movingPlatform, temp, Quaternion.identity);
                        break;
                }

                // Update spike tracking
                if (newPlatform != null)
                {
                    Platform platformScript = newPlatform.GetComponent<Platform>();
                    _lastWasSpike = (platformScript != null && platformScript._isspikes);
                }
            }

            _currentPlatformSpawnTimer = _platformSpawnTimer; // Reset timer
        }
    }
}
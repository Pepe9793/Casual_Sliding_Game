using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject _platform;
    public GameObject _spike;
    public GameObject _breakable;
    public GameObject _movingPlatform;

    public float _platformSpawnTimer = 2f;
    private float _currentPlatformSpawnTimer;

    private int _platformSpawnCount;

    public float _minX = -2f, _maxX = 2f;

    private void Start()
    {
        _currentPlatformSpawnTimer = _platformSpawnTimer;
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
            }
            else
            {
                int rand = Random.Range(0, 4); // 0: normal, 1: spike, 2: breakable, 3: moving

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
            }

            _currentPlatformSpawnTimer = _platformSpawnTimer; // Reset timer
        }
    }
}

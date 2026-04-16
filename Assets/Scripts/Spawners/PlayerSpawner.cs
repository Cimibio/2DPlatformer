using UnityEngine;

namespace Spawners
{
    public class PlayerSpawner : Spawner<Player>
    {
        [Header("Player Settings")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private int _initialLives = 3;
        [SerializeField] private float _respawnDelay = 1f;
        [SerializeField] private bool _debugMode = true;

        [Header("Camera Settings")]
        [SerializeField] private CameraFollower _cameraFollower;

        private Player _currentPlayer;
        private int _currentLives;
        private float _respawnTimer;
        private bool _isRespawning;

        public int CurrentLives => _currentLives;
        public Player CurrentPlayer => _currentPlayer;

        protected override void Start()
        {
            base.Start();
            _currentLives = _initialLives;
            SpawnPlayer();
        }

        private void Update()
        {
            if (_isRespawning)
            {
                _respawnTimer -= Time.deltaTime;

                if (_respawnTimer <= 0)
                {
                    _isRespawning = false;
                    SpawnPlayer();
                }
            }
        }

        private void SpawnPlayer()
        {
            if (_currentLives <= 0)
            {
                Debug.Log($"[{gameObject.name}] GAME OVER! No lives left.");
                return;
            }

            _currentPlayer = GetFromPool();
            _currentPlayer.transform.position = _spawnPoint.position;
            _currentPlayer.transform.rotation = Quaternion.identity;
            _currentPlayer.Reset();
            _currentPlayer.Died += HandlePlayerDeath;

            if (_cameraFollower != null)
            {
                _cameraFollower.SetTarget(_currentPlayer.transform);
            }

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Player spawned. Lives: {_currentLives}");
        }

        private void HandlePlayerDeath()
        {
            if (_currentPlayer == null)
                return;

            _currentPlayer.Died -= HandlePlayerDeath;

            ReleaseToPool(_currentPlayer);
            _currentPlayer = null;

            _currentLives--;

            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Player died. Lives left: {_currentLives}");

            if (_currentLives > 0)
            {
                _isRespawning = true;
                _respawnTimer = _respawnDelay;
            }
            else
            {
                Debug.Log($"[{gameObject.name}] GAME OVER!");
            }
        }
    }
}
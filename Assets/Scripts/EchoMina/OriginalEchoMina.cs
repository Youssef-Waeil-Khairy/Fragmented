using System.Collections.Generic;
using System.Linq;
using Interactables;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace EchoMina.Original
{
    public class OriginalEchoMina : MonoBehaviour
    {
        public delegate void OnStartRecording();
        public delegate void OnStopRecording();
        public delegate void OnStartPlayback();
        public delegate void OnStopPlayback();
        public event OnStartRecording StartRecording;
        public event OnStopRecording StopRecording;
        public event OnStartPlayback StartPlayback;
        public event OnStopPlayback StopPlayback;

        [Header("States")]
        [SerializeField][ReadOnly] private bool _isRecording;
        [SerializeField][ReadOnly] private bool _isPlaying;
        public static OriginalEchoMina Instance;
        public static bool HASSTARTEDUP = false;

        [Header("Movement")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _turnSpeed = 1f;
        [SerializeField, ReadOnly] private float _moveDirection;
        [SerializeField, ReadOnly] private float _turnDirection;

        [Header("Interactions")]
        [SerializeField] private Interactor _interactor;

        [Header("Recordings")]
        [SerializeField][Range(float.MinValue, float.MaxValue)] private float _recordFrequency = 0.1f;
        [SerializeField][ReadOnly] private int _recordIndex;
        [SerializeField][ReadOnly] private int _interactIndex;
        [SerializeField][ReadOnly] private Vector3 _startPosition;
        [SerializeField][ReadOnly] private Quaternion _startRotation;
        [SerializeField][ReadOnly] private List<Vector3> _positions;
        [SerializeField][ReadOnly] private List<Quaternion> _rotations;
        [SerializeField][ReadOnly] private float _interactionTime;
        [SerializeField][ReadOnly] private List<float> _interactions;
        [SerializeField] private CinemachineCamera echoCamera;
        [SerializeField] private CinemachineInputAxisController echoInputAxisController;
        private IRecordable[] recordables;

        // Getters and setters
        public bool IsRecording
        {
            get
            {
                return _isRecording;
            }
        }
        public Interactor EchoInteractor
        {
            get
            {
                return _interactor;
            }
        }

        #region InputMessages
        public void OnMove(Vector3 move)
        {
            _moveDirection = move.x;
            _turnDirection = move.y;
        }
        #endregion

        #region Unity Functions

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            _characterController = GetComponent<CharacterController>();
            _interactor = GetComponent<Interactor>();

            recordables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IRecordable>().ToArray();
            foreach (var recordable in recordables)
            {
                recordable.BindRecordable();
            }

            Despawn();
        }

        private void OnDestroy()
        {
            foreach (IRecordable recordable in recordables)
            {
                recordable.UnbindRecordable();
            }
        }

        private void Update()
        {
            if (_isPlaying && _interactions.Count > 0 && _interactIndex < _interactions.Count)
            {
                _interactionTime += Time.deltaTime;
                if (_interactionTime >= _interactions[_interactIndex])
                {
                    _interactor.InteractCommand();
                    _interactIndex++;
                    _interactionTime = 0f;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_isRecording)
            {
                Vector3 move = transform.forward * (_moveDirection * _speed);
                _characterController.SimpleMove(move);
                transform.Rotate(transform.up, _turnDirection * _turnSpeed * Time.fixedDeltaTime);
            }
            else if (_isPlaying)
            {
                if (_recordIndex >= _positions.Count)
                {
                    CancelInvoke(nameof(LoadSnapshot));
                    TogglePlaying();
                }
            }
        }
        #endregion

        public void SaveInteraction(float interactionTime)
        {
            _interactions.Add(interactionTime);
        }

        private void RecordSnapshot()
        {
            _positions.Add(transform.position);
            _rotations.Add(transform.rotation);
        }

        private void LoadSnapshot()
        {
            if (_recordIndex >= _positions.Count)
            {
                return;
            }

            if (_recordIndex >= _rotations.Count)
            {
                return;
            }

            _characterController.enabled = false;
            transform.position = _positions[_recordIndex];
            transform.rotation = _rotations[_recordIndex];
            _recordIndex++;
            _characterController.enabled = true;
        }

        private void LoadInteraction()
        {
            _interactor.InteractCommand();
            _interactIndex++;
            if (_interactIndex < _interactions.Count)
            {
                Invoke(nameof(LoadInteraction), _interactions[_interactIndex]);
            }
        }

        public void ToggleRecording(Vector3 position, Quaternion rotation)
        {
            _isRecording = !_isRecording;
            if (_isRecording)
            {
                Spawn(position, rotation);
                _positions.Clear();
                _rotations.Clear();
                _interactions.Clear();
                _interactor.StartRecording();
                InvokeRepeating(nameof(RecordSnapshot), _recordFrequency, _recordFrequency);
                if (StartRecording != null) StartRecording();

            }
            else
            {
                CancelInvoke(nameof(RecordSnapshot));
                CancelInvoke(nameof(LoadSnapshot));
                CancelInvoke(nameof(LoadInteraction));
                if (StopRecording != null) StopRecording();
                Despawn();
            }
        }

        private void Despawn()
        {
            Debug.Log("<b><color=red>[ECHOMINA]</color></b> Despawning Echo Mina");

            CancelInvoke(nameof(RecordSnapshot));
            _isRecording = false;
            CancelInvoke(nameof(LoadSnapshot));
            _isPlaying = false;
            echoCamera.enabled = false;
            echoInputAxisController.enabled = false;
            gameObject.SetActive(false);
        }

        private void Spawn(Vector3 position, Quaternion rotation)
        {
            Debug.Log("<b><color=green>[ECHOMINA]</color></b> Spawning Echo Mina");
            gameObject.SetActive(true);
            echoCamera.enabled = true;
            echoInputAxisController.enabled = true;
            _characterController.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _characterController.enabled = true;
            _recordIndex = 0;
            _interactor.StartRecording();
        }

        public void TogglePlaying()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying)
            {
                if (_positions.Count == 0)
                {
                    Debug.Log("<b><color=yellow>[ECHOMINA]</color></b> No data recorded to play Echo Mina");
                    return;
                }
                Debug.Log("<b><color=green>[ECHOMINA]</color></b> Playing recorded playback of Echo Mina");

                gameObject.SetActive(true);
                _characterController.enabled = false;
                transform.position = _startPosition;
                transform.rotation = _startRotation;
                _characterController.enabled = true;
                _recordIndex = 0;
                _interactIndex = 0;
                InvokeRepeating(nameof(LoadSnapshot), _recordFrequency, _recordFrequency);
                if (_interactions.Count > 0)
                {
                    Invoke(nameof(LoadInteraction), _interactions[_interactIndex]);
                }

                if (StartPlayback != null) StartPlayback();
            }
            else
            {
                Debug.Log("<b><color=red>[ECHOMINA]</color></b> Stopping playback of recorded Echo Mina");
                if (StopPlayback != null) StopPlayback();
                Despawn();
            }
        }
    }
}
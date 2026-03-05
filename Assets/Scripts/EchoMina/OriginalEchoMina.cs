using System.Collections.Generic;
using System.Linq;
using Interactables;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using Utility;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace EchoMina.Original
{
    public class OriginalEchoMina : MonoBehaviour
    {
        public enum EchoState
        {
            Recording,
            Playing,
            Inactive
        }
        
        public delegate void OnStartRecording();
        public delegate void OnStopRecording();
        public delegate void OnStartPlayback();
        public delegate void OnStopPlayback();
        public event OnStartRecording StartRecording;
        public event OnStopRecording StopRecording;
        public event OnStartPlayback StartPlayback;
        public event OnStopPlayback StopPlayback;

        [Header("States")]
        [SerializeField][ReadOnly] private EchoState state;
        public static OriginalEchoMina Instance;

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
        private IBindable[] bindables;

        // Getters and setters
        #region Getters And Setters

        public EchoState State
        {
            get
            {
                return state;
            }
        }
        public bool IsRecording
        {
            get
            {
                return state == EchoState.Recording;
            }
        }
        public Interactor EchoInteractor
        {
            get
            {
                return _interactor;
            }
        }

  #endregion

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
            
            bindables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IBindable>().ToArray();
            foreach (var bindable in bindables)
            {
                bindable.BindObject();
            }

            Despawn();
        }

        private void OnDestroy()
        {
            foreach (IRecordable recordable in recordables)
            {
                recordable.UnbindRecordable();
            }

            foreach (var bindable in bindables)
            {
                bindable.UnBindObject();
            }
        }

        private void Update()
        {
            /*
            if (state == EchoState.Playing && _interactions.Count > 0 && _interactIndex < _interactions.Count)
            {
                _interactionTime += Time.deltaTime;
                if (_interactionTime >= _interactions[_interactIndex])
                {
                    _interactor.InteractCommand();
                    _interactIndex++;
                    _interactionTime = 0f;
                }
            }
            */
        }

        private void FixedUpdate()
        {
            switch (state)
            {
                case EchoState.Inactive:
                    if (gameObject.activeSelf)
                    {
                        Despawn();
                    }
                    return;
                case EchoState.Recording:
                    Vector3 move = transform.forward * (_moveDirection * _speed);
                    _characterController.SimpleMove(move);
                    transform.Rotate(transform.up, _turnDirection * _turnSpeed * Time.fixedDeltaTime);
                    break;
                case EchoState.Playing:
                    if (_recordIndex >= _positions.Count)
                    {
                        CancelInvoke(nameof(LoadSnapshot));
                        TogglePlaying();
                    }
                    break;
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
            if (state is EchoState.Inactive or EchoState.Playing)
            {
                Debug.Log("[Echo Mina][Recording] Echo Mina is recording...");
                state = EchoState.Recording;
                
                Spawn(position, rotation);
                _positions.Clear();
                _rotations.Clear();
                _interactions.Clear();
                InvokeRepeating(nameof(RecordSnapshot), _recordFrequency, _recordFrequency);
                if (StartRecording != null) StartRecording();
            }
            else if (state is EchoState.Recording)
            {
                Debug.Log("[Echo Mina][Recording] Echo Mina is no longer recording...");
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

            state = EchoState.Inactive;
            
            CancelInvoke();
            
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
            if (state is EchoState.Recording) // We start playing before stopping recording
            {
                Debug.LogWarning("[Echo Mina][Playback] Echo Mina was still recording when playback started. Recording stopped and playback started");
                CancelInvoke(nameof(RecordSnapshot));
                _recordIndex = 0;
                _interactIndex = 0;
                
                if (StopRecording != null) StopRecording();
                
                _characterController.enabled = false;
                transform.position = _startPosition;
                transform.rotation = _startRotation;
                _characterController.enabled = true;

                if (_interactions.Count > 0)
                {
                    Invoke(nameof(LoadInteraction), _interactions[_interactIndex]);
                }
                
                InvokeRepeating(nameof(LoadSnapshot), _recordFrequency, _recordFrequency);
                
                if (StartPlayback != null) StartPlayback();
                
                state = EchoState.Playing;
            }
            
            else if (state is EchoState.Inactive) // We started or stopped playback
            {
                if (_positions.Count == 0)
                {
                    Debug.Log("<b><color=yellow>[ECHOMINA]</color></b> No data recorded to play Echo Mina");
                    return;
                }
                Debug.Log("<b><color=green>[ECHOMINA]</color></b> Playing recorded playback of Echo Mina");

                gameObject.SetActive(true);
                echoCamera.enabled = false;
                state = EchoState.Playing; 
                   
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
                Debug.Log("<b><color=red>[ECHOMINA]</color></b> Stopping playback of recorded Echo Mina because we reached the end of the recording");
                if (StopPlayback != null) StopPlayback();
                Despawn();
            }
        }
    }
}
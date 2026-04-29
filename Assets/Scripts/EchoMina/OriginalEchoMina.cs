using System;
using System.Collections.Generic;
using System.Linq;
using Interactables;
using JetBrains.Annotations;
using NaughtyAttributes;
using PlayerControls;
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
        [UsedImplicitly] public event OnStartPlayback StartPlayback;
        public event OnStopPlayback StopPlayback;

        [Header("States")] [SerializeField] [ReadOnly]
        private EchoState state;

        public static OriginalEchoMina Instance;

        //trail
        public static event Action OnInstanceReady;
        //trail

        [Foldout("Movement")] Rigidbody rb;
        [Foldout("Movement")][SerializeField] private float _speed = 1f;
        [Foldout("Movement")][SerializeField] private float _turnSpeed = 1f;
        [Foldout("Movement")][SerializeField, ReadOnly] private float _moveDirection;
        [Foldout("Movement")][SerializeField, ReadOnly] private float _turnDirection;
        [Foldout("Movement")][SerializeField] private float maxSpeed = 5f;
        [Foldout("Movement")][SerializeField] private float maxTurnSpeed = 5f;

        [Foldout("Interactions")] [SerializeField] private Interactor _interactor;

        [Header("Recordings")] [SerializeField] [Range(float.MinValue, float.MaxValue)]
        private float _recordFrequency = 0.1f;

        [Foldout("Recording")][SerializeField] [ReadOnly] private int _recordIndex;
        [Foldout("Recording")][SerializeField] [ReadOnly] private int _interactIndex;
        [Foldout("Recording")][SerializeField] private float _recordingTimer = 0;
        [Foldout("Recording")][SerializeField] private float _recordingTimeoutWarning = 15f;
        [Foldout("Recording")][SerializeField] private float _recordingTimeout = 18f;
        [Foldout("Recording")][SerializeField] private GameObject _RecordingWarning;
        [Foldout("Recording")][SerializeField] [ReadOnly] private Vector3 _startPosition;
        [Foldout("Recording")][SerializeField] [ReadOnly] private Quaternion _startRotation;
        [Foldout("Recording")][SerializeField] [ReadOnly] private List<Vector3> _positions;
        [Foldout("Recording")][SerializeField] [ReadOnly] private List<Quaternion> _rotations;
        [Foldout("Recording")][SerializeField] [ReadOnly] private float _interactionTime;
        [Foldout("Recording")][SerializeField] [ReadOnly] private List<float> _interactions;
        [Foldout("Recording")][SerializeField] private CinemachineCamera echoCamera;
        [Foldout("Recording")][SerializeField] private CinemachineInputAxisController echoInputAxisController;
        [Foldout("Recording")]private IRecordable[] recordables;
        [Foldout("Recording")]private IBindable[] bindables;
        
        [Foldout("Animation")][SerializeField] private Animator animator;
        [Foldout("Animation")][SerializeField] private string parameterNameWalking = "IsWalking";
        [Foldout("Animation")][SerializeField] private string parameterNameWalkingSpeed = "WalkingSpeed";
        [Foldout("Animation")][SerializeField] private bool isWalking = false;

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

        public CinemachineCamera EchoCamera  => echoCamera;
        public float MoveDirection
        {
            get { return _moveDirection; }
            set
            {
                _moveDirection = value;
                if (_moveDirection != 0)
                {
                    IsWalking = true;
                    if (!Mathf.Approximately(_moveDirection, animator.GetFloat(parameterNameWalkingSpeed)))
                    {
                        animator.SetFloat(parameterNameWalkingSpeed, _moveDirection);
                    }
                }
                else
                {
                    IsWalking = false;
                }
            }
        }
        public bool IsWalking
        {
            get {return isWalking;}
            set
            {
                isWalking = value;
                if (isWalking != animator.GetBool(parameterNameWalking))
                {
                    animator.SetBool(parameterNameWalking, isWalking);
                }
            }
        }
        #endregion

        #region InputMessages

        public void OnMove(Vector3 move)
        {
            MoveDirection = move.x;
            _turnDirection = move.y;
        }

        #endregion

        #region Unity Functions

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

                //trail
                OnInstanceReady?.Invoke();
                //trail
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            state = EchoState.Inactive;

            rb = GetComponent<Rigidbody>();
            SetMaxSpeed();
            _interactor = GetComponent<Interactor>();
            animator = GetComponentInChildren<Animator>();
            animator.SetBool(parameterNameWalking, false);

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

        private void OnEnable()
        {
            if (PlayerController.Instance != null)
            {
                transform.position = PlayerController.Instance.transform.position;
            }
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
                    _recordingTimer += Time.fixedDeltaTime;
                    if (_recordingTimer >= _recordingTimeoutWarning)
                    {
                        _RecordingWarning.SetActive(true);

                        if (_recordingTimer >= _recordingTimeout)
                        {
                            //ToggleRecording(transform.position, transform.rotation);
                            PlayerController.Instance.ToggleRecording();
                        }

                    }

                    Vector3 move = transform.forward * (_moveDirection * _speed);
                    rb.AddForce(move, ForceMode.Force);
                    rb.AddTorque(transform.up * (_turnDirection * _turnSpeed * Time.fixedDeltaTime));

                    //Debug.Log($"{rb.linearVelocity.magnitude} | {rb.angularVelocity.magnitude}");

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

            transform.position = _positions[_recordIndex];
            transform.rotation = _rotations[_recordIndex];
            _recordIndex++;
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

        public void ToggleRecording()
        {
            if (state is EchoState.Inactive or EchoState.Playing)
            {
                Debug.Log("[Echo Mina][Recording] Echo Mina is recording...");
                Spawn();

                state = EchoState.Recording;

                _recordingTimer = 0f;
                _positions.Clear();
                _rotations.Clear();
                _interactions.Clear();
                InvokeRepeating(nameof(RecordSnapshot), _recordFrequency, _recordFrequency);

                if (StartRecording != null) StartRecording();
            }
            else if (state is EchoState.Recording)
            {
                Debug.Log("[Echo Mina][Recording] Echo Mina is no longer recording...");

                Despawn();
            }
        }

        public void Despawn()
        {
            Debug.Log("<b><color=red>[ECHOMINA]</color></b> Despawning Echo Mina");

            if (state is EchoState.Recording)
            {
                CancelInvoke(nameof(RecordSnapshot));
                CancelInvoke(nameof(LoadSnapshot));
                CancelInvoke(nameof(LoadInteraction));
                if (StopRecording != null) StopRecording();
            }

            state = EchoState.Inactive;

            transform.position = PlayerController.Instance.transform.position;
            transform.rotation = PlayerController.Instance.transform.rotation;

            CancelInvoke();

            echoCamera.enabled = false;
            echoInputAxisController.enabled = false;
            _RecordingWarning.SetActive(false);
            gameObject.SetActive(false);
        }

        [Button]
        public void Spawn()
        {
            Debug.Log($"<b><color=green>[ECHOMINA]</color></b> Spawning Echo Mina");
            gameObject.SetActive(true);
            echoCamera.enabled = true;
            echoInputAxisController.enabled = true;
            transform.position = PlayerController.Instance.transform.position;
            transform.rotation = PlayerController.Instance.transform.rotation;
            _recordIndex = 0;
            _interactor.StartRecording();
        }

        public void TogglePlaying(int value = -1)
        {
            if (value == 0)
            {
                // Inactive -> return
                if (state is EchoState.Inactive) return;
                // Recording -> do nothing
                if (state is EchoState.Recording)
                {
                    Debug.LogError("You are trying to stop a playback while recording. If you see this, you messed something up");
                    CancelInvoke(nameof(RecordSnapshot));
                    if (StopRecording != null) StopRecording();
                    return;
                }
                // Playing ->
                if (state is EchoState.Playing)
                {
                    Debug.Log("<b><color=red>[ECHOMINA]</color></b> Stopping playback of recorded Echo Mina because we reached the end of the recording");
                    if (StopPlayback != null) StopPlayback();
                    Despawn();
                    return;
                }
            }
            else if (value == 1)
            {
                // Inactive -> do nothing
                if (state is EchoState.Playing) return;
            }

            if (state is EchoState.Recording) // We start playing before stopping recording
            {
                Debug.LogWarning("[Echo Mina][Playback] Echo Mina was still recording when playback started. Recording stopped and playback started");
                CancelInvoke(nameof(RecordSnapshot));
                _recordIndex = 0;
                _interactIndex = 0;

                if (StopRecording != null) StopRecording();

                transform.position = _positions[0];
                transform.rotation = _rotations[0];

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

                transform.position = _positions[0];
                transform.rotation = _rotations[0];

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

        [Button]
        public void SetMaxSpeed()
        {
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxTurnSpeed;
        }
    }
}
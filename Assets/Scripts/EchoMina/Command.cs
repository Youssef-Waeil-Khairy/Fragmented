using System;
using System.Collections;
using Interactables;
using UnityEngine;
using UnityEngine.AI;

namespace EchoMina
{
    [Serializable]
    public class Command : MonoBehaviour
    {
        public enum CommandType
        {
            None,
            Move,
            Rotate,
            Wait,
            Interact
        }

        public CommandType Type;
        public GameObject Mina;
        public NavMeshAgent Agent;
        public CommandManager Manager;
        public bool IsExecuting;
        public bool IsTimingOut = false;
        [SerializeField] private float _timeoutTimer;

        // Move command type
        [SerializeField] private LineRenderer _path;
        [SerializeField] private float _errorMargin;
        private Transform _currentTransform;
        [SerializeField] private int _pathIndex;
        [SerializeField] private float _speed;
        public bool IsGrounded;

        // Rotate command type
        private Quaternion _startRotation;
        [SerializeField] private Quaternion _endRotation;
        [SerializeField] private float _rotateSpeed;

        // Wait command type
        private float _currentTime;
        [SerializeField] private float _duration;

        // Interact command type
        private Interactor _interactor;

        public void BeginExecute()
        {
            IsExecuting = true;
            IsTimingOut = false;
            _currentTransform = Mina.transform;
            Agent = _currentTransform.GetComponent<NavMeshAgent>();
            _interactor = Mina.GetComponent<Interactor>();

            if (Type == CommandType.Move)
            {
                Debug.Log("Move command started");
                // Reset all the path indexes and get the starting point
                _pathIndex = -1;
                Agent.stoppingDistance = _errorMargin;
                Agent.speed = _speed;
                //Agent.angularSpeed = _rotateSpeed;

                MoveToNextPoint();

            }
            else if (Type == CommandType.Rotate)
            {
                Debug.Log("Rotate command started");
                _currentTime = 0f;
                _startRotation = Mina.transform.rotation;
            }
            else if (Type == CommandType.Wait)
            {
                Debug.Log("Wait command started");
                _currentTime = 0f;
            }
            else if (Type == CommandType.Interact)
            {
                Debug.Log("Interact command started");
            }
        }

        public void UpdateExecute(float deltaTime)
        {
            if (Type == CommandType.Move)
            {
                // Does the agent have a path and is close to the destination
                if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    MoveToNextPoint();
                }
                else if (Agent.pathPending) // If the agent has a pending path, return and wait
                {
                    return;
                }

                // Vector3 direction = _path.GetPosition(_pathIndex) - _currentTransform.position;
                // direction.Normalize();
                //
                //
                // _currentTransform.rotation = Quaternion.Lerp(_currentTransform.rotation, Quaternion.LookRotation(direction), _rotateSpeed * deltaTime);
            }
            else if (Type == CommandType.Rotate)
            {
                if (_currentTime < _duration)
                {
                    _currentTransform.rotation = Quaternion.Lerp(_startRotation, _endRotation, _currentTime / _duration);
                    _currentTime += deltaTime;
                }
                else
                {
                    _currentTransform.rotation = _endRotation;
                    IsExecuting = false;
                }
            }
            else if (Type == CommandType.Wait)
            {
                _currentTime += deltaTime;
                if (_currentTime >= _duration)
                {
                    IsExecuting = false;
                }
            }
            else if (Type == CommandType.Interact)
            {
                _interactor.InteractCommand();
                IsExecuting = false;
            }
        }

        public void EndExecute()
        {
            Debug.Log($"Finished executing command {Type}");
        }

        void Update()
        {
            if (IsTimingOut)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= _timeoutTimer)
                {
                    Manager.TimedOut(gameObject);
                }
            }
        }

        private void MoveToNextPoint()
        {
            // Increment the path index
            _pathIndex++;

            // Check if we have reached the end of the path
            if (_pathIndex >= _path.positionCount)
            {
                IsExecuting = false;
                return;
            }

            // Test to see if the next point is reachable
            if (!NavMesh.SamplePosition(Mina.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                // It is not reachable
                IsExecuting = false;
                Manager.TimedOut(gameObject);
                return;
            }

            // Set the agent's new destination
            Agent.SetDestination(_path.GetPosition(_pathIndex));
        }

        // private void OnDrawGizmosSelected()
        // {
        //     if (Mina == null) return;
        //     if (Type == CommandType.Rotate)
        //     {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawLine(Mina.transform.position, Mina.transform.position + (Mina.transform.forward + _endRotation));
        //     }
        // }
    }
}
using System;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

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
        public bool IsExecuting;

        // Move command type
        [SerializeField] private LineRenderer _path;
        [SerializeField] private float _errorMargin;
        private Transform _currentTransform;
        [SerializeField] private int _pathIndex;
        [SerializeField] private float _speed;

        // Wait command type
        private float _currentTime = 0f;
        [SerializeField] private float _duration;

        public void BeginExecute()
        {
            IsExecuting = true;

            Debug.Log($"Begin executing command {Type}");

            if (Type == CommandType.Move)
            {
                _pathIndex = 0;
                _currentTransform = Mina.transform;
                _currentTransform.position = _path.GetPosition(_pathIndex);
            }
            else if (Type == CommandType.Wait)
            {
                _currentTime = 0f;
            }
        }

        public void UpdateExecute(float deltaTime)
        {
            if (Type == CommandType.Move)
            {
                Vector3 direction = _path.GetPosition(_pathIndex) - _currentTransform.position;
                direction.Normalize();
                _currentTransform.position += (direction *  _speed * deltaTime);
                float distance = Vector3.Distance(_currentTransform.position, _path.GetPosition(_pathIndex));
                Debug.Log($"Distance: {distance}");
                if (distance <= _errorMargin)
                {
                    Debug.Log("Moving to next path point");
                    _pathIndex++;
                }

                if (_pathIndex >= _path.positionCount)
                {
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
        }

        public void EndExecute()
        {
            Debug.Log($"Finished executing command {Type}");
        }
    }
}
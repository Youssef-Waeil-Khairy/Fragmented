using System;
using NaughtyAttributes;
using UnityEngine;

public class PressurePlateMover : MonoBehaviour
{
    [SerializeField] private Transform _objectToMove;
    [SerializeField] private float _moveDistance = 2f;      // How far it moves (Y axis)
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private bool _moveUpOnPress = true;    // false = moves DOWN on press

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    void Start()
    {
        _startPosition = _objectToMove.position;
        _targetPosition = _startPosition;
    }

    void Update()
    {
        _objectToMove.position = Vector3.MoveTowards(
            _objectToMove.position,
            _targetPosition,
            _moveSpeed * Time.deltaTime
        );
    }

    // Call this from OnBeginInteraction or OnUnInteractOn
    [Button]
    public void MoveToPressed()
    {
        float direction = _moveUpOnPress ? 1f : -1f;
        _targetPosition = _startPosition + Vector3.up * _moveDistance * direction;
    }

    // Call this from OnEndInteraction or OnUnInteractOff
    [Button]
    public void MoveToResting()
    {
        _targetPosition = _startPosition;
    }

    private void OnDrawGizmos()
    {
        if (_objectToMove != null)
        {
            Debug.DrawLine(_objectToMove.position, _objectToMove.position + (_objectToMove.up * (_moveUpOnPress ? 1f : -1f)) * _moveDistance, Color.blueViolet);
        }
    }
}
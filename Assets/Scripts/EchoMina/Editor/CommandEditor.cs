using UnityEditor;

namespace EchoMina.Editor
{
    [CustomEditor(typeof(Command))]
    public class CommandEditor : UnityEditor.Editor
    {
        private SerializedProperty _commandType;
        private SerializedProperty _mina;
        private SerializedProperty _manager;
        private SerializedProperty _timeOutDelay;
        private SerializedProperty _path;
        private SerializedProperty _delayDuration;
        private SerializedProperty _margin;
        private SerializedProperty _speed;
        private SerializedProperty _rotateSpeed;
        private SerializedProperty _rotateTarget;

        void OnEnable()
        {
            _commandType = serializedObject.FindProperty("Type");
            _mina = serializedObject.FindProperty("Mina");
            _manager = serializedObject.FindProperty("Manager");
            _timeOutDelay = serializedObject.FindProperty("_timeoutTimer");
            _path = serializedObject.FindProperty("_path");
            _delayDuration = serializedObject.FindProperty("_duration");
            _margin = serializedObject.FindProperty("_errorMargin");
            _speed = serializedObject.FindProperty("_speed");
            _rotateTarget = serializedObject.FindProperty("_endRotation");
            _rotateSpeed = serializedObject.FindProperty("_rotateSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_commandType);
            EditorGUILayout.PropertyField(_mina);
            EditorGUILayout.PropertyField(_manager);
            EditorGUILayout.PropertyField(_timeOutDelay);

            Command.CommandType t = (Command.CommandType)_commandType.enumValueIndex;

            switch (t)
            {
                case Command.CommandType.None:
                    break;
                case Command.CommandType.Move:
                    EditorGUILayout.PropertyField(_path);
                    EditorGUILayout.PropertyField(_margin);
                    EditorGUILayout.PropertyField(_speed);
                    break;
                case Command.CommandType.Rotate:
                    EditorGUILayout.PropertyField(_rotateTarget);
                    EditorGUILayout.PropertyField(_delayDuration);
                    break;
                case Command.CommandType.Wait:
                    EditorGUILayout.PropertyField(_delayDuration);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
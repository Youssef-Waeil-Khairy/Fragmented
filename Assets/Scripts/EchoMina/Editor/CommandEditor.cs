using UnityEditor;

namespace EchoMina.Editor
{
    [CustomEditor(typeof(Command))]
    public class CommandEditor : UnityEditor.Editor
    {
        private SerializedProperty _commandType;
        private SerializedProperty _mina;
        private SerializedProperty _path;
        private SerializedProperty _delayDuration;
        private SerializedProperty _margin;
        private SerializedProperty _speed;

        void OnEnable()
        {
            _commandType = serializedObject.FindProperty("Type");
            _mina = serializedObject.FindProperty("Mina");
            _path = serializedObject.FindProperty("_path");
            _delayDuration = serializedObject.FindProperty("_duration");
            _margin = serializedObject.FindProperty("_errorMargin");
            _speed = serializedObject.FindProperty("_speed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_commandType);
            EditorGUILayout.PropertyField(_mina);

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
                case Command.CommandType.Wait:
                    EditorGUILayout.PropertyField(_delayDuration);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
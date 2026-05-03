using UnityEditor;
using UnityEngine;

namespace EchoMina.Editor
{
    [CustomEditor(typeof(CommandManager))]
    public class CommandManagerEditor : UnityEditor.Editor
    {
        private Command.CommandType  _commandType;
        private SerializedProperty _echo;

        void OnEnable()
        {
            _echo = serializedObject.FindProperty("_echo");
        }
        public override void OnInspectorGUI()
        {
            CommandManager commandManager = (CommandManager)target;

            EditorGUILayout.PropertyField(_echo);
            serializedObject.ApplyModifiedProperties();

            if (commandManager.Commands.Count == 0)
            {
                commandManager.GetCommands();
            }

            _commandType = (Command.CommandType)EditorGUILayout.EnumPopup("Command Type", _commandType);
            if (GUILayout.Button("Add Command") && _commandType != Command.CommandType.None)
            {
                GameObject go = new GameObject($"{_commandType}", typeof(Command));
                go.transform.SetParent(commandManager.transform);
                Command command = go.GetComponent<Command>();
                command.Type = _commandType;
                command.Manager = commandManager;
                command.Mina = commandManager.Echo;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Current Command Index: {commandManager.CommandIndex}");
            EditorGUILayout.LabelField($"Current Command: {commandManager.Commands[commandManager.CommandIndex].Type}");
        }
    }
}
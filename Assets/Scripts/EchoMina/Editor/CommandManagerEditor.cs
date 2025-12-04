using UnityEditor;

namespace EchoMina.Editor
{
    [CustomEditor(typeof(CommandManager))]
    public class CommandManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CommandManager commandManager = (CommandManager)target;

            if (commandManager.Commands.Count == 0)
            {
                commandManager.GetCommands();
            }

            EditorGUILayout.LabelField($"Current Command Index: {commandManager.CommandIndex}");
            EditorGUILayout.LabelField($"Current Command: {commandManager.Commands[commandManager.CommandIndex].Type}");
            
        }
    }
}
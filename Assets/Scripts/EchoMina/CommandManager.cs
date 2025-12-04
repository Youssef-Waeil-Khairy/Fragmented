using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoMina
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeReference]
        public List<Command> Commands;
        [SerializeField] private int _commandIndex = 0;
        public int  CommandIndex { get; }

        private void OnEnable()
        {
            // Get a list of all the commands for this Echo Mina
            var gos = GetComponentsInChildren<Command>();
            Commands = new List<Command>();
            Commands.AddRange(gos);
            _commandIndex = 0;

            Commands[_commandIndex].BeginExecute();
        }

        private void Update()
        {
            if (_commandIndex >= Commands.Count)
            {
                return;
            }

            if (Commands[_commandIndex].IsExecuting)
            {
                Commands[_commandIndex].UpdateExecute(Time.deltaTime);
            }
            else
            {
                NextCommand();
            }
        }
        private void NextCommand()
        {
            Commands[_commandIndex].EndExecute();
            _commandIndex++;

            if (_commandIndex >= Commands.Count) return;
            
            Commands[_commandIndex].BeginExecute();
        }

        public void GetCommands()
        {
            Commands.Clear();
            Commands.AddRange(GetComponentsInChildren<Command>());
        }
    }
}
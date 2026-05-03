using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoMina
{
    public class CommandManager : MonoBehaviour
    {
        public List<Command> Commands;
        [SerializeField] private int _commandIndex = 0;
        [SerializeField] private GameObject _echo;
        public int  CommandIndex { get { return _commandIndex; } }
        public GameObject Echo { get { return _echo; } }

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

        /// <summary>
        /// Called if a given command timed out. Commands can time out if they try to move along a path but there is no ground under them
        /// </summary>
        /// <param name="command">GameObject of the command that timed out</param>
        public void TimedOut(GameObject command)
        {
            // Disable Echo Mina
            command.GetComponent<Command>().Mina.SetActive(false);
            // Disable CommandManager
            gameObject.SetActive(false);
        }
    }
}
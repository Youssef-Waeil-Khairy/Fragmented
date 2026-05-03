using System;
using UnityEngine;

namespace Utility
{
    public class StartupLogger : MonoBehaviour
    {
        #region Startup

        /// <summary>
        /// Printed formatted start up message for things in Awake
        /// </summary>
        /// <param name="message">what happened</param>
        /// <param name="scriptName">Who is calling this. use this.name when calling it</param>
        public static void LogAwake(string message, string scriptName)
        {
            Debug.Log($"<color=teal><b>[Start Up][Awake][{scriptName}]</color></b> {message}");
        }

        /// <summary>
        /// Printed formatted start up message for things in Awake
        /// </summary>
        /// <param name="message">what happened</param>
        /// <param name="scriptName">Who is calling this. use this.name when calling it</param>
        public static void LogEnable(string message, string scriptName)
        {
            Debug.Log($"<color=teal><b>[Start Up][On Enable][{scriptName}]</color></b> {message}");
        }
        
        /// <summary>
        /// Printed formatted start up message for things in Awake
        /// </summary>
        /// <param name="message">what happened</param>
        /// <param name="scriptName">Who is calling this. use this.name when calling it</param>
        public static void LogStart(string message, string scriptName)
        {
            Debug.Log($"<color=teal><b>[Start Up][Start][{scriptName}]</color></b> {message}");
        }

  #endregion

        #region Shutdown
        
        /// <summary>
        /// Formatted message for shutting down in OnDisable
        /// </summary>
        /// <param name="message"></param>
        /// <param name="scriptName"></param>
        public static void LogDisable(string message, string scriptName)
        {
            Debug.Log($"<color=teal><b>[Shutdown][On Disable][{scriptName}]</color></b> {message}");
        }
        
        /// <summary>
        /// Formatted message for shutting down in OnDisable
        /// </summary>
        /// <param name="message"></param>
        /// <param name="scriptName"></param>
        public static void LogDestroy(string message, string scriptName)
        {
            Debug.Log($"<color=teal><b>[Shutdown][On Destroy][{scriptName}]</color></b> {message}");
        }

  #endregion
        
    }
}

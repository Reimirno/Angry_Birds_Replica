/***
DebugExt.cs

Description: A better debug for Unity
Author: Yu Long
Created: Monday, November 22 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reimirno
{
    public static class DebugExt
    {
        /// <summary>
        /// Call this method from any object, instead of calling the native Debug.Log().
        /// </summary>
        /// <param name="myObj">The caller object</param>
        /// <param name="msg">Messages to log. You can pass in more than one.</param>
        public static void Log(this Object myObj, params object[] msg)
        {
            DoLog(Debug.Log, "", myObj, msg);
        }

        /// <summary>
        /// Call this method from any object, instead of calling the native Debug.LogError().
        /// </summary>
        /// <param name="myObj">The caller object</param>
        /// <param name="msg">Messages to log. You can pass in more than one.</param>
        public static void LogError(this Object myObj, params object[] msg)
        {
            DoLog(Debug.LogError, "<!>".ApplyColor("red"), myObj, msg);
        }

        /// <summary>
        /// Call this method from any object, instead of calling the native Debug.LogWarning().
        /// </summary>
        /// <param name="myObj">The caller object</param>
        /// <param name="msg">Message to log. You can pass in more than one.</param>
        public static void LogWarning(this Object myObj, params object[] msg)
        {
            DoLog(Debug.LogWarning, "<?>".ApplyColor("yellow"), myObj, msg);
        }

        /// <summary>
        /// An alternative to Object.Log(). They are the same except for the color of message printed. Meant to display success message.
        /// </summary>
        /// <param name="myObj">The caller object</param>
        /// <param name="msg">Messages to log. You can pass in more than one.</param>
        public static void LogSuccess(this Object myObj, params object[] msg)
        {
            DoLog(Debug.Log, "<O>".ApplyColor("green"), myObj, msg);
        }

        public static void LogToDo(this Object myObj, params object[] msg)
        {
            DoLog(Debug.LogWarning, "<?>".ApplyColor("yellow"), myObj, "Remember to implement this:", msg);
        }

        /// <summary>
        /// Private helper methods that formats color into strings.
        /// </summary>
        /// <param name="myStr">The original string</param>
        /// <param name="color">Color name</param>
        /// <returns>The formated string with color.</returns>
        private static string ApplyColor(this string myStr, string color)
        {
            return $"<color={color}>{myStr}</color>";
        }

        /// <summary>
        /// Helper function to handle all actual logging actions.
        /// </summary>
        /// <param name="LogFunction">The native debug method to be used</param>
        /// <param name="prefix">Prefix to the actual log</param>
        /// <param name="myObj">Caller object name</param>
        /// <param name="msg">Actual log message</param>
        private static void DoLog(Action<string, Object> LogFunction, string prefix, Object myObj, params object[] msg)
        {
#if UNITY_EDITOR
            var name = (myObj ? myObj.name : "NullObject").ApplyColor("lightblue");
            LogFunction($"{prefix}[{name}]: {string.Join("; ", msg)}\n ", myObj);
#endif
        }

    }
}

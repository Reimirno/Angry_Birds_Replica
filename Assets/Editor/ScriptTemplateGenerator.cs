/***
ScriptTemplateGenerator.cs

Description: Manage and create C# script from templates instead of using the original Unity Template.
It also prepends a header to each created C# file. Just like this.
Make sure to update your info (your name, contact, project namespace) at the bottom of this file so the header reflects your info instead of mine.
Author: Yu Long
Created: Monday, November 22 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Reimirno
{
    /// <summary>
    /// Creates scripts from txt template files.
    /// </summary>
    public class ScriptTemplateGenerator
    {

        /// <summary>
        /// Script types
        /// </summary>
        public enum ScriptType
        {
            CSharpScript,
            MonobehaviorScript,
            ScriptableObjectScript,
            SingletonScript
        }

        /// <summary>
        /// Abstraction of template objects. Contains infomation about the template.
        /// </summary>
        public struct ScriptTemplate
        {
            public ScriptType type;
            public string defaultName;
            public string path;
        }

        /// <summary>
        /// Keep track of available templates.
        /// </summary>
        private static Dictionary<ScriptType, ScriptTemplate> Templates;

        /// <summary>
        /// Static initializer. Populate the template dictionary.
        /// If more templates are added, make sure to add them into the dictionary here with the correct name and path.
        /// </summary>
        static ScriptTemplateGenerator(){
            
            Templates = new Dictionary<ScriptType, ScriptTemplate>();
            Templates.Add(ScriptType.CSharpScript, new ScriptTemplate
            {
                type = ScriptType.CSharpScript,
                defaultName = "NewCSharpScript.cs",
                path = "/Editor/CSharpScriptTemplate.txt",
            });
            Templates.Add(ScriptType.MonobehaviorScript, new ScriptTemplate
            {
                type = ScriptType.MonobehaviorScript,
                defaultName = "NewMonoScript.cs",
                path = "/Editor/MonoScriptTemplate.txt",
            });
            Templates.Add(ScriptType.ScriptableObjectScript, new ScriptTemplate
            {
                type = ScriptType.ScriptableObjectScript,
                defaultName = "NewScriptableObject.cs",
                path = "/Editor/ScriptableObjectTemplate.txt",
            });
            Templates.Add(ScriptType.SingletonScript, new ScriptTemplate
            {
                type = ScriptType.SingletonScript,
                defaultName = "NewSingletonScript.cs",
                path = "/Editor/SingletonTemplate.txt",
            });
        }

        /// <summary>
        /// A wrapper that creates Unity asset menu for creating our templated scripts.
        /// </summary>
        [MenuItem("Assets/Create/Create Script/My C# Script", false, 0)]
        private static void CreateMyCSScript()
        {
            GenerateScript(ScriptType.CSharpScript);
        }

        /// <summary>
        /// A wrapper that creates Unity asset menu for creating our templated scripts.
        /// </summary>
        [MenuItem("Assets/Create/Create Script/My Mono Script", false, 1)]
        private static void CreateMyMonoScript()
        {
            GenerateScript(ScriptType.MonobehaviorScript);
        }

        /// <summary>
        /// A wrapper that creates Unity asset menu for creating our templated scripts.
        /// </summary>
        [MenuItem("Assets/Create/Create Script/My Scriptable Object", false, 2)]
        private static void CreateMySOScript()
        {
            GenerateScript(ScriptType.ScriptableObjectScript);
        }

        /// <summary>
        /// A wrapper that creates Unity asset menu for creating our templated scripts.
        /// </summary>
        [MenuItem("Assets/Create/Create Script/My Singleton Script", false, 3)]
        private static void CreateMySingletonScript()
        {
            GenerateScript(ScriptType.SingletonScript);
        }

        /// <summary>
        /// Core method to generate the script. First read the corresponding txt file and then create an cs file.
        /// </summary>
        /// <param name="type"></param>
        private static void GenerateScript(ScriptType type)
        {
            try
            {
                var templateObj = Templates[type];
                ProjectWindowUtil.CreateAssetWithContent(
                    templateObj.defaultName,
                    File.ReadAllText(Application.dataPath + templateObj.path),
                    EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                Debug.LogError("Something went wrong. Perhaps check the template path?");
           }
        }

        /// <summary>
        /// Add a header to each script created (not just the templated script).
        /// </summary>
        private class AddHeader : UnityEditor.AssetModificationProcessor
        {
            /// <summary>
            /// The header to be added to each script created.
            /// </summary>
            private static string Header =
@"/*
#SCRIPTNAME#.cs

Description: To be filled in.
Author: #AUTHORNAME#
Created: #CREATIONTIME#
Unity Version: #UNITYVERSION#
Contact: #AUTHOREMAIL#
*/
";
            /// <summary>
            /// Add a header to each script created (not just the templated script).
            /// </summary>
            private static void OnWillCreateAsset(string path)
            {
                path = path.Replace(".meta", "");
                string fileExt = Path.GetExtension(path);

                if (fileExt != ".cs") return;

                string realPath = Application.dataPath.Replace("Assets", "") + path;
                string scriptContent = File.ReadAllText(realPath);

                var ns = typeof(ScriptTemplateGenerator).Namespace;                 //Change this to your own project's namespace
                scriptContent = scriptContent.Replace("#NAMESPACE#", ns);
                var sn = Path.GetFileName(Path.GetFileNameWithoutExtension(path));
                scriptContent = scriptContent.Replace("#SCRIPTNAME#", sn);
                var head = Header;
                head = head.Replace("#SCRIPTNAME#", sn);
                head = head.Replace("#UNITYVERSION#", Application.unityVersion);
                head = head.Replace("#CREATIONTIME#", DateTime.Now.ToString("dddd, MMMM dd yyyy"));
                head = head.Replace("#AUTHORNAME#", "Yu Long");                     //Change this to your own name
                head = head.Replace("#AUTHOREMAIL#", "long_yu@berkeley.edu");       //Change this to your own email address
                
                scriptContent = scriptContent.Replace("#HEADER#", head);

                File.WriteAllText(path, scriptContent);
                //AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            }
        }
    }
}


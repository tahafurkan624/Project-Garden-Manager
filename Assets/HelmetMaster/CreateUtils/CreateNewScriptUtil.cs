#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;
#endif

namespace zz_HelmetMaster.CreateUtils
{
    public static class CreateNewScriptUtil
    {
#if UNITY_EDITOR
        public static void CreateScript(string pathName)
        {
            string resourceFile = Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates"), "81-C# Script-NewBehaviourScript.cs.txt");
            CreateScriptAssetFromTemplate(pathName, resourceFile);
            AssetDatabase.Refresh();
        }

        private static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string resourceContent = File.ReadAllText(resourceFile);
            return CreateScriptAssetWithContent(pathName, PreprocessScriptAssetTemplate(pathName, resourceContent));
        }
        private static Object CreateScriptAssetWithContent(string pathName, string templateContent)
        {
            templateContent = SetLineEndings(templateContent, EditorSettings.lineEndingsForNewScripts);
            File.WriteAllText(Path.GetFullPath(pathName), templateContent);
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof (UnityEngine.Object));
        }
        private static string SetLineEndings(string content, LineEndingsMode lineEndingsMode)
        {
            string replacement;
            switch (lineEndingsMode)
            {
                case LineEndingsMode.OSNative:
                    replacement = Application.platform != RuntimePlatform.WindowsEditor ? "\n" : "\r\n";
                    break;
                case LineEndingsMode.Unix:
                    replacement = "\n";
                    break;
                case LineEndingsMode.Windows:
                    replacement = "\r\n";
                    break;
                default:
                    replacement = "\n";
                    break;
            }
            content = Regex.Replace(content, "\\r\\n?|\\n", replacement);
            return content;
        }
        
        private static string PreprocessScriptAssetTemplate(string pathName, string resourceContent)
        {
            string rootNamespace = (string) null;
            if (Path.GetExtension(pathName) == ".cs")
                rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            string str1 = resourceContent.Replace("#NOTRIM#", "");
            string withoutExtension = Path.GetFileNameWithoutExtension(pathName);
            string str2 = str1.Replace("#NAME#", withoutExtension);
            string str3 = withoutExtension.Replace(" ", "");
            string str4 = RemoveOrInsertNamespace(str2.Replace("#SCRIPTNAME#", str3), rootNamespace);
            string str5;
            if (char.IsUpper(str3, 0))
            {
                string newValue = char.ToLower(str3[0]).ToString() + str3.Substring(1);
                str5 = str4.Replace("#SCRIPTNAME_LOWER#", newValue);
            }
            else
            {
                string newValue = "my" + char.ToUpper(str3[0]).ToString() + str3.Substring(1);
                str5 = str4.Replace("#SCRIPTNAME_LOWER#", newValue);
            }
            return str5;
        }
        
        private static string RemoveOrInsertNamespace(string content, string rootNamespace)
        {
            string str1 = "#ROOTNAMESPACEBEGIN#";
            string str2 = "#ROOTNAMESPACEEND#";
            if (!content.Contains(str1) || !content.Contains(str2))
                return content;
            if (string.IsNullOrEmpty(rootNamespace))
            {
                content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + str1 + "[ \\t]*", string.Empty);
                content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + str2 + "[ \\t]*", string.Empty);
                return content;
            }
            string separator = content.Contains("\r\n") ? "\r\n" : "\n";
            List<string> stringList = new List<string>((IEnumerable<string>) content.Split(new string[3]
            {
                "\r\n",
                "\r",
                "\n"
            }, StringSplitOptions.None));
            int index1 = 0;
            while (index1 < stringList.Count && !stringList[index1].Contains(str1))
                ++index1;
            string str3 = stringList[index1];
            string str4 = str3.Substring(0, str3.IndexOf("#"));
            stringList[index1] = "namespace " + rootNamespace;
            stringList.Insert(index1 + 1, "{");
            for (int index2 = index1 + 2; index2 < stringList.Count; ++index2)
            {
                string str5 = stringList[index2];
                if (!string.IsNullOrEmpty(str5) && str5.Trim().Length != 0)
                {
                    if (str5.Contains(str2))
                    {
                        stringList[index2] = "}";
                        break;
                    }
                    stringList[index2] = str4 + str5;
                }
            }
            return string.Join(separator, stringList.ToArray());
        }
#endif
    }
}
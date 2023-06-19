using System.IO;
using UnityEditor;

namespace TGameEditor
{
    public class ScriptKeywordReplace : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".meta")
            {
                path = path.Remove(path.LastIndexOf('.'));
                extension = Path.GetExtension(path);
            }
            if (extension != ".cs" && extension != ".js")
                return;

            string file = File.ReadAllText(path);

            string shortPath = path.Remove(0, "Assets/".Length);
            if (shortPath.StartsWith("Scripts/"))
            {
                shortPath = shortPath.Remove(0, "Scripts/".Length);
            }
            int index = shortPath.LastIndexOf('/');
            if (index > -1)
            {
                shortPath = shortPath.Remove(index);
            }
            else
            {
                shortPath = "Script";
            }
            shortPath = shortPath.Replace('/', '.');
            shortPath = shortPath.Replace(" ", string.Empty);

            file = file.Replace(KeywordReplaceWindow.CREATIONDATE, System.DateTime.Now.ToString("yyyy/MM/dd"));
            file = file.Replace(KeywordReplaceWindow.FILEPATH, shortPath);
            file = file.Replace(KeywordReplaceWindow.PROJECTNAME, PlayerSettings.productName);
            file = file.Replace(KeywordReplaceWindow.AUTHOR, EditorPrefs.GetString(KeywordReplaceWindow.AUTHOR));

            File.WriteAllText(path, file);
            AssetDatabase.Refresh();
        }
    }

    public class KeywordReplaceWindow : EditorWindow
    {
        [MenuItem("Tools/脚本关键字替换")]
        public static void ShowWindow()
        {
            GetWindow<KeywordReplaceWindow>("脚本关键字替换");
        }

        public const string CREATIONDATE = "#CREATIONDATE#";
        public const string FILEPATH = "#FILEPATH#";
        public const string PROJECTNAME = "#PROJECTNAME#";
        public const string AUTHOR = "#AUTHOR#";

        private void OnGUI()
        {
            string author = EditorGUILayout.TextField(AUTHOR, EditorPrefs.GetString(AUTHOR));
            EditorPrefs.SetString(AUTHOR, author);

            EditorGUILayout.LabelField(PROJECTNAME, PlayerSettings.productName);
            EditorGUILayout.LabelField(CREATIONDATE, System.DateTime.Now.ToString("yyyy/MM/dd"));
            EditorGUILayout.LabelField(FILEPATH, "文件路径");
        }
    }
}
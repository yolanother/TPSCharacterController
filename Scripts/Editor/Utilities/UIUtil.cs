using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace DoubTech.TPSCharacterController
{
    public class UIUtil
    {
        private static Dictionary<string, GUIContent> iconPool = new Dictionary<string, GUIContent>();
        private static Dictionary<Color, GUIStyle> backgroundStylePool = new Dictionary<Color, GUIStyle>();

        private static GUIStyle boxStyle;
        public static GUIStyle BoxStyle {
            get
            {
                if (null == boxStyle)
                {
                    boxStyle = new GUIStyle(GUI.skin.box);
                    boxStyle.alignment = TextAnchor.MiddleCenter;
                    boxStyle.fontStyle = FontStyle.Italic;
                    boxStyle.fontSize = 12;
                    GUI.skin.box = boxStyle;
                }

                return boxStyle;
            }
        }

        public static T AssetFileBrowser<T>(string title, string[] extensions, string defaultPath = null) where T : Object {
            string key = "TPSUTIL::AFB::" + title;
            string lastPath = EditorPrefs.GetString(key);
            if (null == lastPath) lastPath = null != defaultPath ? defaultPath : Application.dataPath;
            string path = EditorUtility.OpenFilePanelWithFilters(title, lastPath, extensions);
            if (null != path) {
                EditorPrefs.SetString(key, path);
                path = "Assets/" + path.Substring(Application.dataPath.Length);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return null;
        }
        public static string FileSaveBrowser(string title, string defaultName, string extension, string defaultPath = null) {
            string key = "TPSUTIL::FSB::" + title;
            string lastPath = EditorPrefs.GetString(key);
            if (null == lastPath) lastPath = null != defaultPath ? defaultPath : Application.dataPath;
            string path = EditorUtility.SaveFilePanel(title, lastPath, defaultName, extension);
            
            if (null != path) {
                EditorPrefs.SetString(key, new FileInfo(path).DirectoryName);
            }

            return path;
        }
        public static string DirectoryBrowser(string title, string defaultName, string defaultPath = null) {
            string key = "TPSUTIL::DB::" + title;
            string lastPath = EditorPrefs.GetString(key);
            if (string.IsNullOrEmpty(lastPath)) lastPath = null != defaultPath ? defaultPath : Application.dataPath;
            string path = EditorUtility.OpenFolderPanel(title, lastPath, defaultName);
            
            if (null != path) {
                EditorPrefs.SetString(key, path);
            }

            return path;
        }

        public static string DirectoryBrowseField(string title, string currentPath)
        {
            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(currentPath))
            {
                string key = "TPSUTIL::DB::" + title;
                currentPath = EditorPrefs.GetString(key);
            }
            currentPath = EditorGUILayout.TextField(title, currentPath);
            if (GUILayout.Button("Browse", GUILayout.Width(75)))
            {
                currentPath = UIUtil.DirectoryBrowser(title, "Pickups", currentPath);
            }
            EditorGUILayout.EndHorizontal();
            return currentPath;
        }

        public static GUIContent GetGuiIcon(string iconName, string tooltip = null)
        {
            GUIContent content;
            if (!iconPool.TryGetValue(iconName + tooltip, out content))
            {
                content = EditorGUIUtility.IconContent(iconName, tooltip);
                iconPool[iconName + tooltip] = content;
            }

            return content;
        }

        public static bool ButtonIconBordered(string iconName, string tooltip = null)
        {
            return GUILayout.Button(GetGuiIcon(iconName, tooltip), GUILayout.Width(24), GUILayout.Height(24));
        }

        public static bool ButtonIconBorderless(string iconName, string tooltip = null)
        {
            return GUILayout.Button(GetGuiIcon(iconName, tooltip), GUI.skin.label, GUILayout.Width(24), GUILayout.Height(24));
        }

        public static GUIStyle BackgroundColor(float r, float g, float b, float a = 1)
        {
            return BackgroundColor(new Color(r, g, b, a));
        }

        public static GUIStyle BackgroundColor(Color color)
        {
            GUIStyle style;

            if (!backgroundStylePool.TryGetValue(color, out style))
            {
                backgroundStylePool[color] = style = new GUIStyle();
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
            }

            return style;
        }

        public static void BoldLabel(string label)
        {
            GUILayout.Label(label, EditorStyles.boldLabel);
        }
    }
}

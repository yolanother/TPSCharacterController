using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController
{
    public class UIUtil
    {
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
    }
}

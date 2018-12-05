using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace GameKit
{
    public class SceneCheck
    {
        public static string Execute(string projectCode, string folder)
        {
            string root = Path.GetFullPath(".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

            string searchPath = root + folder + (Path.DirectorySeparatorChar + projectCode);
            if (!Directory.Exists(searchPath))
            {
                return "";
            }
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0) return "";

            int counter = -1;

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                EditorUtility.DisplayProgressBar("Search File", scene.path, counter / (float) scenes.Length);
                counter++;
                if (Path.GetFileName(scene.path).IndexOf(projectCode + "_") != 0)
                {
                    EditorUtility.ClearProgressBar();
                    return "场景命名不是以'" + projectCode + "_'开头: " + scene.path;
                }
            }

            EditorUtility.ClearProgressBar();
            return "";
        }

        public static string Execute(string[] projectCodes, string folder)
        {
            string root = Path.GetFullPath(".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

            string searchPath = root + folder;
            if (!Directory.Exists(searchPath))
            {
                return "";
            }
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0) return "";

            int counter = -1;

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                EditorUtility.DisplayProgressBar("Search File", scene.path, counter / (float) scenes.Length);
                counter++;

                bool isMatch = false;
                foreach (var projectCode in projectCodes)
                {
                    if (Path.GetFileName(scene.path).IndexOf(projectCode + "_") == 0)
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (!isMatch)
                {
                    EditorUtility.ClearProgressBar();
                    return "场景命名不是以任何一个子项目编号开头: " + scene.path;
                }
            }

            EditorUtility.ClearProgressBar();
            return "";
        }
    }
}
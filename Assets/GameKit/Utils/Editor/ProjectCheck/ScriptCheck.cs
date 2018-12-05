using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GameKit
{
	/// <summary>
	/// 代码检测，检测namespace是否为ProjectCode ,检测是否使用了自己写的WWW来加载资源
	/// </summary>
	public class ScriptCheck {

		public static string Execute(string projectCode , string folder , bool isCombProj = false){
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			string searchPath = root + folder + (isCombProj ? Path.DirectorySeparatorChar + projectCode:"");
			if(!Directory.Exists(searchPath)){
				return "";
			}

			List<string> filePath = new List<string>();  
			filePath.AddRange(
				Directory.GetFiles(searchPath, "*.cs", SearchOption.AllDirectories)  
			);

			int counter = -1;  
			foreach (string file in filePath) {
				string tempFolder = file.Substring (root.Length);
				EditorUtility.DisplayProgressBar ("Search file", tempFolder, counter / (float)filePath.Count);  
				counter++; 

				string contents = File.ReadAllText(file);
				var regexStr = @"namespace\s+"+projectCode+"";
				Match mt = Regex.Match(contents, regexStr);
				if(!mt.Success){
					EditorUtility.ClearProgressBar ();  
					return "命名空间不是"+projectCode+": " + tempFolder;
				}

				if (contents.Contains("WWW ")) {
					EditorUtility.ClearProgressBar ();  
					return "用了自己写的WWW来加载资源: " + tempFolder;
				}
			}

			EditorUtility.ClearProgressBar();  
			return "";
		}

	}
}

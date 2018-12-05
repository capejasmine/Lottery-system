using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// 判断Resources和StreamingAssets文件夹目录是否正确
	/// </summary>
	public class FolderCheck {

		public static string Execute(string projectCode , string folder , bool isCombProj = false)
		{
			
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			//判断StreamingAssets文件夹
			string streamingFolder = root+"StreamingAssets"+ Path.DirectorySeparatorChar;
			if(isCombProj){
				streamingFolder += projectCode + Path.DirectorySeparatorChar; 
			}
			if(Directory.Exists(streamingFolder))
			{
				string[] streamingChildren = Directory.GetDirectories (streamingFolder);
				string[] streamingFiles = Directory.GetFiles (streamingFolder);
				if (!isCombProj &&streamingChildren!=null && streamingChildren.Length > 1) {
					return "StreamingAssets内容没有按规则:" + streamingChildren [0].Substring (root.Length);
				}
				if (streamingFiles!=null && streamingFiles.Length>0) {
					foreach (string file in streamingFiles) {
						if (file.IndexOf(".DS_Store") == -1 && file.IndexOf (".meta") == -1) {
							EditorUtility.ClearProgressBar();  
							return "StreamingAssets内容没有按规则: "+file.Substring(root.Length);
						}
					}
				}
			}

			string searchPath = root + folder + (isCombProj ? Path.DirectorySeparatorChar + projectCode:"");
			if(!Directory.Exists(searchPath)){
				return "";
			}
			//判断Resources文件夹
			List<string> folderPath = new List<string>();
			folderPath.AddRange(
				Directory.GetDirectories(searchPath, "Resources", SearchOption.AllDirectories)  
			);
			int counter = -1;  
			foreach (string resources in folderPath) {
				EditorUtility.DisplayProgressBar ("Search Resources", resources.Substring(root.Length), counter / (float)folderPath.Count);  
				counter++; 
				string[] children = Directory.GetDirectories (resources);
				string[] files = Directory.GetFiles (resources);

				if (children!=null && children.Length > 1) {
					EditorUtility.ClearProgressBar ();  
					return "Resources内容没有按规则: " + children [0].Substring (root.Length);
				}
				//!Path.GetFileName(children[0]).Equals(projectCode)
				if (files!=null && files.Length>0) {
					foreach (string file in files) {
						if (file.IndexOf(".DS_Store") == -1 && file.IndexOf (".meta") == -1) {
							EditorUtility.ClearProgressBar();
							return "Resources内容没有按规则: "+file.Substring(root.Length);
						}
					}
				}
			}

			EditorUtility.ClearProgressBar();  
			return "";
		}
	}

}

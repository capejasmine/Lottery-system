using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// StreamingAssets下的文件大小检查
	/// </summary>
	public class FileSizeCheck {
		
		const float IMG_MAX_FILE_SIZE = 2f*1024*1024;//图片文件大小上限
		const float AUDIO_MAX_FILE_SIZE = 3f*1024*1024;//声音文件大小上限

		public static string Execute(string projectCode , bool isCombProj = false){
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			string streamingFolder = root+"StreamingAssets"+ Path.DirectorySeparatorChar;
			if(!Directory.Exists(streamingFolder)) return "";

			if(isCombProj){
				//只检测某一个文件夹下面的文件
				streamingFolder += projectCode + Path.DirectorySeparatorChar;
				if(!Directory.Exists(streamingFolder)) return "";
			}
			List<string> filePath = new List<string>();  
			filePath.AddRange(
				Directory.GetFiles(streamingFolder,"*.*", SearchOption.AllDirectories)
			); 

			int counter = -1;  
			foreach (string file in filePath) {  
				string tempFolder = file.Substring (root.Length);
				EditorUtility.DisplayProgressBar ("Search File", tempFolder, counter / (float)filePath.Count);  
				counter++; 

				var fileInfo = new FileInfo(file);
				string ext = fileInfo.Extension.ToLower ();
				if (ext.Equals (".mp3") || ext.Equals (".wav") || ext.Equals (".ogg")) {
					if (fileInfo.Length > AUDIO_MAX_FILE_SIZE) {
						EditorUtility.ClearProgressBar ();  
						return "文件大小超过了"+AUDIO_MAX_FILE_SIZE+"bytes: " + tempFolder;
					}
				}
				else if(ext.Equals (".jpg") || ext.Equals (".png") || ext.Equals (".jpeg"))
				{
					if (fileInfo.Length > IMG_MAX_FILE_SIZE) {
						EditorUtility.ClearProgressBar ();  
						return "文件大小超过了"+IMG_MAX_FILE_SIZE+"bytes: " + tempFolder;
					}
				}
			}

			EditorUtility.ClearProgressBar();  
			return "";
		}

	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// 图片尺寸大小检测
	/// </summary>
	public class ImgSizeCheck{

		public const int MAX_SIZE = 2048;
			
		public static string Execute(string projectCode , string folder, bool isCombProj = false){
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			List<string> filePath = new List<string>();  
			if(isCombProj){
				string streamingFolder = root + "StreamingAssets"+ Path.DirectorySeparatorChar;
				if(Directory.Exists(streamingFolder))
				{
					string checkFolder = streamingFolder + projectCode + Path.DirectorySeparatorChar;
					if(Directory.Exists(checkFolder))
					{
						filePath.AddRange(
							Directory.GetFiles(checkFolder,"*.*", SearchOption.AllDirectories)
						); 
					}
				}

				string assetFolder = root + folder + Path.DirectorySeparatorChar + projectCode + Path.DirectorySeparatorChar; 
				if(Directory.Exists(assetFolder))
				{
					filePath.AddRange(
						Directory.GetFiles(assetFolder,"*.*", SearchOption.AllDirectories)
					); 
				}
			}else{
				filePath.AddRange(
					Directory.GetFiles(root,"*.*", SearchOption.AllDirectories)
				); 
			}

			int counter = -1;  
			foreach (string file in filePath) {  
				string tempFolder =  file.Substring (root.Length);
				EditorUtility.DisplayProgressBar ("Search File", tempFolder, counter / (float)filePath.Count);  
				counter++; 

				string ext = Path.GetExtension(file).ToLower ();
				if (ext.Equals (".jpg") || ext.Equals (".png") || ext.Equals (".jpeg")) {

					Texture2D texture = GameCheckerEditor.LoadPNG (file);
					if(CheckTransparent(texture)){
						EditorUtility.ClearProgressBar ();  
						return "图片透明部分太多: " + tempFolder;
					}

					if (texture.width>MAX_SIZE || texture.height> MAX_SIZE) {
						EditorUtility.ClearProgressBar ();  
						return "图片尺寸超过了"+MAX_SIZE+"px: " + tempFolder;
					}
				}
			}

			EditorUtility.ClearProgressBar();  
			return "";
		}

		static bool CheckTransparent(Texture2D texture){
			return false;
		}
	}
}
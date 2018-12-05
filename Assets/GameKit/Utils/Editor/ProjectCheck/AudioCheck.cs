using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace GameKit
{
	public class AudioCheck
	{
		public const int MAX_LENGTH = 10 ;//最大声音时间 ，单位秒

		public static string Execute(string projectCode , string folder , bool isCombProj = false){
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			string searchPath = root+folder+(isCombProj ? Path.DirectorySeparatorChar + projectCode:"");
			if(!Directory.Exists(searchPath)){
				return "";
			}

			List<string> filePath = new List<string>();  
			filePath.AddRange(
				Directory.GetFiles(searchPath,"*.*", SearchOption.AllDirectories)
			); 
			int counter = -1;

			foreach (string file in filePath) {
				string tempFolder = file.Substring (root.Length);
				EditorUtility.DisplayProgressBar ("Search File", tempFolder, counter / (float)filePath.Count);  
				counter++; 

				string ext = Path.GetExtension(file).ToLower ();
				if (ext.Equals (".mp3") || ext.Equals (".wav") || ext.Equals (".ogg") || ext.Equals(".m4a")) {
					string path = file.Substring (file.IndexOf("Assets/"));
					AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip> (path);
					var audioImporter = (AudioImporter) AudioImporter.GetAtPath(path);
					if (audio && audio.length > MAX_LENGTH && audioImporter.defaultSampleSettings.loadType!= AudioClipLoadType.Streaming)
					{
						EditorUtility.ClearProgressBar ();  
						return "此声音文件没有设置成Streaming: " + tempFolder;
					}
					if (audio && audioImporter.forceToMono == false) {
						EditorUtility.ClearProgressBar ();  
						return "此声音文件没有设置成Mono(单声道): " + tempFolder;
					}
				}
			}

			EditorUtility.ClearProgressBar();  
			return "";
		}


		public static void FixByOneKey(string projectCode, string folder, bool isCombProj = false)
		{
			string root = Path.GetFullPath(".") + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

			string searchPath = root + folder + (isCombProj ? Path.DirectorySeparatorChar + projectCode : "");
			if (!Directory.Exists(searchPath))
			{
				return;
			}

			List<string> filePath = new List<string>();
			filePath.AddRange(
				Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories)
			);
			int counter = -1;

			foreach (string file in filePath)
			{
				string tempFolder = file.Substring(root.Length);
				EditorUtility.DisplayProgressBar("Search File", tempFolder, counter / (float) filePath.Count);
				counter++;

				string ext = Path.GetExtension(file).ToLower();
				if (ext.Equals(".mp3") || ext.Equals(".wav") || ext.Equals(".ogg") || ext.Equals(".m4a"))
				{
					string path = file.Substring(file.IndexOf("Assets/"));
					AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
					var audioImporter = (AudioImporter) AudioImporter.GetAtPath(path);
					var isDirty = false;
					if (audio && audio.length > MAX_LENGTH &&
						audioImporter.defaultSampleSettings.loadType != AudioClipLoadType.Streaming)
					{
						var setting = audioImporter.defaultSampleSettings;
						setting.loadType = AudioClipLoadType.Streaming;
						audioImporter.defaultSampleSettings = setting;
						isDirty = true;
					}
					if (audio && audioImporter.forceToMono == false)
					{
						audioImporter.forceToMono = true;
						isDirty = true;

						SerializedObject serializedObject = new UnityEditor.SerializedObject (audioImporter);
						SerializedProperty normalize = serializedObject.FindProperty ("m_Normalize");
						normalize.boolValue = false;
						serializedObject.ApplyModifiedProperties();

					}
					if(isDirty){
						audioImporter.SaveAndReimport();
						EditorUtility.SetDirty(audio);
					}
				}
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
		}
	}
}
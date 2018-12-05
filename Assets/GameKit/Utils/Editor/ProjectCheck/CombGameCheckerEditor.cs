using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// 合集游戏检查
	/// </summary>
	public class CombGameCheckerEditor : EditorWindow  {

		string checkFolder = "";
		string projectCodes = "";
		string log = "";

		[MenuItem("Window/合集项目检测",false,10001)]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			CombGameCheckerEditor window = (CombGameCheckerEditor)EditorWindow.GetWindow(typeof(CombGameCheckerEditor));
			window.minSize = new Vector2(220,400);
			window.titleContent = new GUIContent ("合集项目检测");
			window.Show();
		}

		void OnGUI()
		{
			GUILayout.Label("设置", EditorStyles.boldLabel);

			checkFolder = EditorGUILayout.TextField("Root文件夹(可以为空)", checkFolder.Trim());
			projectCodes = EditorGUILayout.TextField("项目代号(用,分隔多个)", projectCodes.ToUpper().Trim());

			string[] pcodes = projectCodes.Split(',');
			if(pcodes==null || pcodes.Length==0 || projectCodes.Length==0) return;
			for(int i = 0;i<pcodes.Length;++i){
				pcodes[i] = pcodes[i].Trim();
			}

			GUILayout.Space(10);
			if(GUILayout.Button("检查文件大小",GUILayout.Height(25))){
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						log = FileSizeCheck.Execute (projectCode,true);
						if(log.Length>0) break;
					}
				}
			}
			if(GUILayout.Button("检查图片尺寸",GUILayout.Height(25))){
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						log = ImgSizeCheck.Execute (projectCode,checkFolder,true);
						if(log.Length>0) break;
					}
				}
			}

			if(GUILayout.Button("检查目录结构",GUILayout.Height(25))){
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						log = FolderCheck.Execute (projectCode,checkFolder,true);
						if(log.Length>0) break;
					}
				}
			}
			if(GUILayout.Button("检查代码",GUILayout.Height(25))){
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						log = ScriptCheck.Execute (projectCode,checkFolder,true);
						if(log.Length>0) break;
					}
				}
			}
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("检查声音文件",GUILayout.Height(25))){
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						log = AudioCheck.Execute (projectCode,checkFolder,true);
						if(log.Length>0) break;
					}
				}
			}
			if(GUILayout.Button("一键修复声音",GUILayout.Height(25))){
				log = "";
				foreach(string projectCode in pcodes)
				{
					if(!string.IsNullOrEmpty(projectCode))
					{
						AudioCheck.FixByOneKey (projectCode,checkFolder,true);
					}
				}

			}
			GUILayout.EndHorizontal();

			if(GUILayout.Button("检查场景名称",GUILayout.Height(25))){
				log = SceneCheck.Execute (pcodes,checkFolder);
			}



			//日志输出
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("错误输出");
			GUILayout.EndHorizontal();
			Color bgColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			GUIStyle guiStyle = EditorStyles.textArea;
			guiStyle.wordWrap = true;
			EditorGUILayout.LabelField (log,guiStyle,GUILayout.MaxHeight(100));
			GUI.backgroundColor = bgColor;
		}


		public static Texture2D LoadPNG(string filePath) {
			Texture2D tex = null;
			byte[] fileData;

			if (File.Exists(filePath))     {
				fileData = File.ReadAllBytes(filePath);
				tex = new Texture2D(2, 2);
				tex.LoadImage(fileData);
			}
			return tex;
		}

	}
}
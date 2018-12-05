using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// 游戏检查
	/// </summary>
	public class GameCheckerEditor : EditorWindow  {

		internal string projectCode = "";
		string checkFolder = "_Game";
		string log = "";

		[MenuItem("Window/普通项目检测",false,10000)]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			GameCheckerEditor window = (GameCheckerEditor)EditorWindow.GetWindow(typeof(GameCheckerEditor));
			window.minSize = new Vector2(220,430);
			window.titleContent = new GUIContent ("普通项目检测");
			window.projectCode = EditorSettings.projectGenerationRootNamespace;
			window.Show();
		}

		void OnGUI()
		{
			GUILayout.Label("设置", EditorStyles.boldLabel);
			projectCode = EditorGUILayout.TextField("项目代号", projectCode.ToUpper().Trim());

			if(string.IsNullOrEmpty(projectCode)) return;

			GUILayout.BeginHorizontal();
			Color bgColor = GUI.backgroundColor;
			Color contentColor = GUI.contentColor;
			GUI.backgroundColor = Color.yellow;
			GUI.contentColor = Color.white;
			if(GUILayout.Button("初始化项目设置",GUILayout.Height(30)) && EditorUtility.DisplayDialog("是否确认用"+projectCode+"来初始化项目？","","是","否")){
				EditorSettings.serializationMode = SerializationMode.ForceText;
				EditorSettings.projectGenerationRootNamespace = projectCode;
	//			EditorSettings.spritePackerMode = SpritePackerMode.Disabled;
				EditorSettings.externalVersionControl = "Visible Meta Files";

				this.ShowNotification(new GUIContent("项目设置成功"));
			}
			GUI.backgroundColor = bgColor;
			GUI.contentColor = contentColor;
			GUILayout.EndHorizontal();

			GUILayout.Space(15);
			if(GUILayout.Button("检查文件大小",GUILayout.Height(25))){
				log = FileSizeCheck.Execute (projectCode);
			}
			if(GUILayout.Button("检查图片尺寸",GUILayout.Height(25))){
				log = ImgSizeCheck.Execute (projectCode,checkFolder);
			}
			if(GUILayout.Button("检查ios和android资源大小",GUILayout.Height(25))){
				log = ExportCheck.Execute (projectCode);
			}


			GUILayout.Space(10);
			checkFolder = EditorGUILayout.TextField("要检测的文件夹", checkFolder.Trim());
			if (string.IsNullOrEmpty (checkFolder)) {
				return;
			}

			if(GUILayout.Button("检查目录结构",GUILayout.Height(25))){
				log = FolderCheck.Execute (projectCode,checkFolder);
			}
			if(GUILayout.Button("检查代码",GUILayout.Height(25))){
				log = ScriptCheck.Execute (projectCode,checkFolder);
			}
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("检查声音文件",GUILayout.Height(25))){
				log = AudioCheck.Execute (projectCode,checkFolder);
			}
			if(GUILayout.Button("一键修复声音",GUILayout.Height(25))){
				log = "";
				AudioCheck.FixByOneKey (projectCode,checkFolder);
			}
			GUILayout.EndHorizontal();
			if(GUILayout.Button("检查场景名称",GUILayout.Height(25))){
				log = SceneCheck.Execute (projectCode,checkFolder);
			}

			//日志输出
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("错误输出");
			GUILayout.EndHorizontal();
			bgColor = GUI.backgroundColor;
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

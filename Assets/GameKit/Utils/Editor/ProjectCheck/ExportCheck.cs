using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace GameKit
{
	/// <summary>
	/// 判断ios和android资源大小大小
	/// </summary>
	public class ExportCheck  {

		const int MAX_SIZE = 100;//单位M

		public static string Execute(string projectCode , bool isCombProj = false){
			string root = Path.GetFullPath (".") + Path.DirectorySeparatorChar;

			string iosPath = root + "proj.ios_mac"+ Path.DirectorySeparatorChar;
			if(Directory.Exists(iosPath))
			{
				string dataPath = iosPath + "Data"+ Path.DirectorySeparatorChar;
				float size = GetDirectorySize(dataPath)/1024f/1024f;
				if( size > MAX_SIZE ){
					return "IOS资源超出"+MAX_SIZE+"M大小: proj.ios_mac/Data";
				}
			}
			else
			{
				return "IOS工程目录不存在：proj.ios_mac";
			}

			string androidPath = root + "proj.android-studio"+ Path.DirectorySeparatorChar;
			if(Directory.Exists(androidPath))
			{
				string dataPath = androidPath + "app/src/main/assets"+ Path.DirectorySeparatorChar;
				float size = GetDirectorySize(dataPath)/1024f/1024f;
				if( size > MAX_SIZE ){
					return "Android资源超出"+MAX_SIZE+"M大小: proj.android-studio/app/src/main/assets";
				}
			}
			else
			{
				return "Android工程目录不存在：proj.android-studio";
			}
			return "";
		}

		/// <summary>
		/// 获取文件夹大小
		/// </summary>
		/// <returns>The directory size.</returns>
		/// <param name="dirPath">Directory Path.</param>
		static long GetDirectorySize(string dirPath)
		{
			if (!Directory.Exists(dirPath))
				return 0;
			long len=0;
			DirectoryInfo di = new DirectoryInfo(dirPath);
			//通过GetFiles方法,获取di目录中的所有文件的大小
			foreach (FileInfo fi in di.GetFiles())
			{
				len += fi.Length;
			}

			//获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
			DirectoryInfo[] dis = di.GetDirectories();
			if (dis.Length > 0)
			{
				for (int i = 0; i < dis.Length; i++)
				{
					len += GetDirectorySize(dis[i].FullName);
				}
			}

			return len;
		}
	}

}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// png处理
/// Author:zhouzhanglin
/// </summary>
public class PngProcess : Editor {

	[MenuItem("Tools/Premulity Alpha(文件夹)",false,64)]
	static void PremulityAlphaFolder () {

		if(Selection.activeObject is DefaultAsset)
		{
			Dictionary<string,string> texturePathKV = new Dictionary<string, string>();
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(Directory.Exists(dirPath)){
				foreach (string path in Directory.GetFiles(dirPath))
				{  
					if(path.LastIndexOf(".meta")==-1){
						if( System.IO.Path.GetExtension(path) == ".png"){
							int start = path.LastIndexOf("/")+1;
							int end = path.LastIndexOf(".png");
							texturePathKV[path.Substring(start,end-start)] = path.Substring(6);
							continue;
						}
					}
				} 


				if( texturePathKV.Count>0){
					foreach(string fileName in texturePathKV.Keys){
						string path = texturePathKV[fileName];
						Texture2D t = LoadPNG(Application.dataPath+"/"+path);
						Color32[] colors =  t.GetPixels32();
						for(int i=0;i<colors.Length;++i){
							Color32 c = colors[i];
							c.r =(byte)( (c.a*c.r)/255);
							c.g =(byte)( (c.a*c.g)/255);
							c.b =(byte)( (c.a*c.b)/255);
							colors[i] = c;
						}
						t.SetPixels32(colors);
						byte[] bytes = t.EncodeToPNG();
						SavePNG(Application.dataPath+path,bytes);
					}

				}
			}
		}
		AssetDatabase.Refresh();
	}

	[MenuItem("Tools/Premulity Alpha(图片)",false,65)]
	static void PremulityAlphaFile(){
		if(Selection.activeObject is DefaultAsset || Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(File.Exists(dirPath) && dirPath.LastIndexOf(".meta")==-1 && System.IO.Path.GetExtension(dirPath) == ".png"){
				string path = dirPath.Substring(6);
				Texture2D t = LoadPNG(Application.dataPath+path);
				Color32[] colors =  t.GetPixels32();
				for(int i=0;i<colors.Length;++i){
					Color32 c = colors[i];
					c.r =(byte)( (c.a*c.r)/255);
					c.g =(byte)( (c.a*c.g)/255);
					c.b =(byte)( (c.a*c.b)/255);
					colors[i] = c;
				}
				t.SetPixels32(colors);
				byte[] bytes = t.EncodeToPNG();
				SavePNG(Application.dataPath+path,bytes);
				AssetDatabase.Refresh();
			}
		}
	}


	[MenuItem("Tools/Remove Premulity Alpha(文件夹)",false,66)]
	static void RemovePremulityAlphaFolder () {

		if(Selection.activeObject is DefaultAsset)
		{
			Dictionary<string,string> texturePathKV = new Dictionary<string, string>();
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(Directory.Exists(dirPath)){
				foreach (string path in Directory.GetFiles(dirPath))
				{  
					if(path.LastIndexOf(".meta")==-1){
						if( System.IO.Path.GetExtension(path) == ".png"){
							int start = path.LastIndexOf("/")+1;
							int end = path.LastIndexOf(".png");
							texturePathKV[path.Substring(start,end-start)] = path.Substring(6);
							continue;
						}
					}
				} 


				if( texturePathKV.Count>0){
					foreach(string fileName in texturePathKV.Keys){
						string path = texturePathKV[fileName];
						Texture2D t = LoadPNG(Application.dataPath+"/"+path);
						Color32[] colors =  t.GetPixels32();
						for(int i=0;i<colors.Length;++i){
							Color32 c = colors[i];
							if(c.a>0){
								c.r =(byte)( ((float)c.r/c.a)*255);
								c.g =(byte)( ((float)c.g/c.a)*255);
								c.b =(byte)( ((float)c.b/c.a)*255);
							}
							colors[i] = c;
						}
						t.SetPixels32(colors);
						byte[] bytes = t.EncodeToPNG();
						SavePNG(Application.dataPath+path,bytes);
					}

				}
			}
		}
		AssetDatabase.Refresh();
	}

	[MenuItem("Tools/Remove Premulity Alpha(图片)",false,67)]
	static void RemovePremulityAlphaFile(){
		if(Selection.activeObject is DefaultAsset || Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(File.Exists(dirPath) && dirPath.LastIndexOf(".meta")==-1 && System.IO.Path.GetExtension(dirPath) == ".png"){
				string path = dirPath.Substring(6);
				Texture2D t = LoadPNG(Application.dataPath+path);
				Color32[] colors =  t.GetPixels32();
				for(int i=0;i<colors.Length;++i){
					Color32 c = colors[i];
					if(c.a>0){
						c.r =(byte)( ((float)c.r/c.a)*255);
						c.g =(byte)( ((float)c.g/c.a)*255);
						c.b =(byte)( ((float)c.b/c.a)*255);
					}
					colors[i] = c;
				}
				t.SetPixels32(colors);
				byte[] bytes = t.EncodeToPNG();
				SavePNG(Application.dataPath+path,bytes);
				AssetDatabase.Refresh();
			}
		}
	}

	//所有StreamingAssets下面的图片
	[MenuItem("Tools/Encrypt StreamingAssets(所有)",false,128)]
	static void ReverseBitmapBytesStreamingAssets(){
		List<string> paths = new List<string>();
		string dirPath = Application.streamingAssetsPath;
		GetAllImgs(paths,dirPath);

		if(paths.Count>0){
			int i = 0;
			EditorApplication.update = delegate() {
				if(i<paths.Count){
					string p = paths[i];
					ReverseBitmapBytes(p);
					EditorUtility.DisplayProgressBar("Reverse Bitmap Bytes", "", (float)i / paths.Count);
					++i;
				}
				else
				{
					EditorUtility.ClearProgressBar();
					EditorApplication.update = null;
				}
			};
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}
	}
	static void GetAllImgs( List<string> paths , string dirPath){
		if(Directory.Exists(dirPath)){
			foreach(string file in Directory.GetFiles(dirPath)){
				if( file.IndexOf(".png")>0 || file.IndexOf(".PNG")>0 || 
					file.IndexOf(".jpg")>0 || file.IndexOf(".JPG")>0
				){
					paths.Add(file);
				}
			}
			foreach (string directory in Directory.GetDirectories(dirPath))
			{  
				if(dirPath.LastIndexOf(".meta")==-1 ){
					GetAllImgs(paths,directory);
				}
			}
		}
	}

	//某一个文件夹
	[MenuItem("Tools/Encrypt One Folder(文件夹)",false,129)]
	static void ReverseBitmapBytesFolder(){
		if(Selection.activeObject is DefaultAsset)
		{
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(Directory.Exists(dirPath)){
				foreach (string path in Directory.GetFiles(dirPath))
				{  
					if(path.LastIndexOf(".meta")==-1 &&
						(path.IndexOf(".png")>0 || path.IndexOf(".PNG")>0 || 
							path.IndexOf(".jpg")>0 || path.IndexOf(".JPG")>0)
					){
						ReverseBitmapBytes(Application.dataPath+path.Substring(6));
					}
				}
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}
		}
	}
	[MenuItem("Tools/Encrypt a Bitmap(图片)",false,130)]
	static void ReverseBitmapBytesFile(){
		if(Selection.activeObject is DefaultAsset || Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			if(File.Exists(dirPath) && dirPath.LastIndexOf(".meta")==-1  &&
				(dirPath.IndexOf(".png")>0 || dirPath.IndexOf(".PNG")>0 || 
					dirPath.IndexOf(".jpg")>0 || dirPath.IndexOf(".JPG")>0)

			){
				ReverseBitmapBytes(Application.dataPath+dirPath.Substring(6));
				AssetDatabase.Refresh();
			}
		}
	}

	[MenuItem("Tools/Sprites导出png",false,3)]
	static void SpritesToPng () {
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
			if(objs!=null)
			{
				Texture2D source = LoadPNG(Application.dataPath+path.Substring(6));
				if(source==null) return;

				string folder = Application.dataPath+"_"+Selection.activeObject.name;
				if(Directory.Exists(folder)){
					Directory.Delete(folder,true);
				}
				Directory.CreateDirectory(folder);

				foreach(Object obj in objs)
				{
					if(obj is Sprite)
					{
						Sprite s = obj as Sprite;

						Texture2D texture = new Texture2D((int)s.rect.width,(int)s.rect.height,TextureFormat.ARGB32,false);
						for(int i=0;i<texture.width;++i){
							for(int j=0;j<texture.height;++j){
								Color c = source.GetPixel((int)s.textureRect.x+i,(int)s.textureRect.y+j);
								texture.SetPixel(i,j,c);
							}
						}
						texture.Apply();
						File.WriteAllBytes(folder+"/"+s.name+".png",texture.EncodeToPNG());
					}
				}
				Debug.Log("======>导出位置:"+folder);
			}
		}
	}



	[MenuItem("Tools/Resize Bitmap Size/最接近2^n(长宽不等)",false,3)]
	static void ResizeBitmapToNearPOT(){
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			string path = Application.dataPath+dirPath.Substring(6);
			Texture2D t = LoadPNG(path);
			int width = GetNearPOT(t.width);
			int height = GetNearPOT(t.height);
			if(width>0 && height>0){
				Texture2D newTexture = new Texture2D(width,height);
				ClearTexture2D(newTexture);
				newTexture.wrapMode = t.wrapMode;
				newTexture.filterMode = t.filterMode;
				newTexture.anisoLevel = t.anisoLevel;
				newTexture.alphaIsTransparency = t.alphaIsTransparency;

				for(int i = 0 ;i<t.width;++i){
					for(int j=0;j<t.height; ++j){
						Color c = t.GetPixel(i,j);
						newTexture.SetPixel(i,height-t.height+j,c);
					}
				}
				SavePNG(path,newTexture);
				AssetDatabase.Refresh();
			}
		}
	}

	[MenuItem("Tools/Resize Bitmap Size/最接近2^n(长宽相同)",false,3)]
	static void ResizeBitmapToNearPOT2(){
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			string path = Application.dataPath+dirPath.Substring(6);
			Texture2D t = LoadPNG(path);
			int width = GetNearPOT(t.width);
			int height = GetNearPOT(t.height);
			if(width>0 && height>0){
				if(width>height) height = width;
				else if(height>width) width = height;

				Texture2D newTexture = new Texture2D(width,height);
				ClearTexture2D(newTexture);
				newTexture.wrapMode = t.wrapMode;
				newTexture.filterMode = t.filterMode;
				newTexture.anisoLevel = t.anisoLevel;
				newTexture.alphaIsTransparency = t.alphaIsTransparency;

				for(int i = 0 ;i<t.width;++i){
					for(int j=0;j<t.height; ++j){
						Color c = t.GetPixel(i,j);
						newTexture.SetPixel(i,height-t.height+j,c);
					}
				}
				SavePNG(path,newTexture);
				AssetDatabase.Refresh();
			}
		}
	}


	[MenuItem("Tools/Resize Bitmap Size/宽裁剪一半",false,3)]
	static void ResizeBitmapCutWidth(){
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			string path = Application.dataPath+dirPath.Substring(6);
			Texture2D t = LoadPNG(path);
			int width = t.width/2;
			int height = t.height;

			Texture2D newTexture = new Texture2D(width,height);
			ClearTexture2D(newTexture);
			newTexture.wrapMode = t.wrapMode;
			newTexture.filterMode = t.filterMode;
			newTexture.anisoLevel = t.anisoLevel;
			newTexture.alphaIsTransparency = t.alphaIsTransparency;

			for(int i = 0 ;i<width;++i){
				for(int j=0;j<height; ++j){
					Color c = t.GetPixel(i,j);
					newTexture.SetPixel(i,j,c);
				}
			}
			SavePNG(path,newTexture);
			AssetDatabase.Refresh();
		}
	}

	[MenuItem("Tools/Resize Bitmap Size/高裁剪一半",false,3)]
	static void ResizeBitmapCutHeight(){
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string dirPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			string path = Application.dataPath+dirPath.Substring(6);
			Texture2D t = LoadPNG(path);
			int width = t.width;
			int height = t.height/2;

			Texture2D newTexture = new Texture2D(width,height);
			ClearTexture2D(newTexture);
			newTexture.wrapMode = t.wrapMode;
			newTexture.filterMode = t.filterMode;
			newTexture.anisoLevel = t.anisoLevel;
			newTexture.alphaIsTransparency = t.alphaIsTransparency;

			for(int i = 0 ;i<width;++i){
				for(int j=0;j<height; ++j){
					Color c = t.GetPixel(i,j);
					newTexture.SetPixel(i,height+j,c);
				}
			}
			SavePNG(path,newTexture);
			AssetDatabase.Refresh();
		}
	}


	static int GetNearPOT(int size){
		if(size<=2) return 2;
		if(size<=4) return 4;
		if(size<=8) return 8;
		if(size<=16) return 16;
		if(size<=32) return 32;
		if(size<=64) return 64;
		if(size<=128) return 128;
		if(size<=256) return 256;
		if(size<=512) return 512;
		if(size<=1024) return 1024;
		if(size<=2048) return 2048;
		return -1;//not support
	}

	static void ReverseBitmapBytes(string filePath)
	{
		if (File.Exists(filePath))     {
			byte[] fileData = File.ReadAllBytes(filePath);
			System.Array.Reverse(fileData);

			if(File.Exists(filePath+".meta")){
				File.Delete(filePath+".meta");
			}
			File.Delete(filePath);

			if(filePath.LastIndexOf(".txt")==-1){
				filePath += ".txt";
			}else{
				filePath = filePath.Substring(0,filePath.LastIndexOf(".txt"));
			}
			SavePNG(filePath,fileData);
		}
	}

	static Texture2D LoadPNG(string filePath) {
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath))     {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData);
		}
		return tex;
	}

	static void SavePNG(string filePath,Texture2D t) {
		File.WriteAllBytes(filePath,t.EncodeToPNG());
	}
	static void SavePNG(string filePath,byte[] fileData) {
		File.WriteAllBytes(filePath,fileData);
	}

	static void ClearTexture2D(Texture2D t){
		Color32[] colors =  t.GetPixels32();
		for(int i=0;i<colors.Length;++i){
			Color32 c = colors[i];
			c.a = 0;
			colors[i] = c;
		}
		t.SetPixels32(colors);
	}
}

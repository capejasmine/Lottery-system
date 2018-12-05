using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameKit
{
    public class AssetImportedProcess : AssetPostprocessor
    {
        void OnPostprocessAudio(AudioClip clip)
        {
            AudioImporter model = (AudioImporter) assetImporter;
            if (model != null)
            {
                if (!model.forceToMono)
                    Debug.LogError("声音导入需要设为Force Mono", clip);
                if (clip.length > AudioCheck.MAX_LENGTH)
                {
                    AudioImporterSampleSettings setting = model.defaultSampleSettings;
                    if (setting.loadType != AudioClipLoadType.Streaming)
                    {
                        Debug.LogError("长度超过10秒的声音，需要加载类型设置为Streaming", clip);
                    }
                }
            }
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            if (texture.width > ImgSizeCheck.MAX_SIZE || texture.height > ImgSizeCheck.MAX_SIZE)
            {
                Debug.LogError("图片尺寸超过了" + ImgSizeCheck.MAX_SIZE + "px: ", texture);
            }
        }
    }
}
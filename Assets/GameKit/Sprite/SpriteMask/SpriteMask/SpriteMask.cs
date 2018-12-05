using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteMask : MonoBehaviour {

	public enum MaskType{
		None,
		MaskCanRotate,
		MaskedCanRotate
	}

	public MaskType maskType = MaskType.MaskedCanRotate;
	public Rect maskSize = new Rect(0f,0f,1f,1f);
	public Material[] maskMaterials;

	public bool updateAlways = true;

	private Vector4 m_ClipRect;

	// Use this for initialization
	void Start () {
		UpdateClip();
	}

	void LateUpdate () {
		if(updateAlways) UpdateClip();
	}

	public void UpdateClip(){
		if(maskMaterials!=null){

			maskSize.width = Mathf.Max(0.0001f,maskSize.width);
			maskSize.height = Mathf.Max(0.0001f,maskSize.height);

			Vector3 bl = transform.TransformPoint(new Vector3(maskSize.x,maskSize.y,0f));
			Vector3 tr = transform.TransformPoint(new Vector3(maskSize.x + maskSize.width,maskSize.y + maskSize.height,0f));

			m_ClipRect.x = bl.x;
			m_ClipRect.y = bl.y;

			m_ClipRect.z = tr.x;
			m_ClipRect.w = tr.y;

			for(int i=0;i<maskMaterials.Length;++i){
				Material m = maskMaterials[i];
				if(m) {
					m.SetFloat("_MaskType",(int)maskType);
					m.SetVector("_MaskClip",m_ClipRect);
				}
			}
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Vector3 pos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		Matrix4x4 cubeTransform = Matrix4x4.TRS(pos, transform.rotation, transform.lossyScale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Gizmos.matrix *= cubeTransform;
		Gizmos.DrawWireCube(new Vector3(maskSize.x,maskSize.y,0f)+new Vector3(maskSize.width*0.5f,maskSize.height*0.5f,0f),new Vector3(maskSize.width,maskSize.height,0.1f));
		Gizmos.matrix = oldGizmosMatrix;
	}
}

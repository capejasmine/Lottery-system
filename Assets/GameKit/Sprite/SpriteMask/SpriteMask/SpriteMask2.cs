using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteMask2 : MonoBehaviour {

	public Rect maskSize = new Rect(0f,0f,1f,1f);
	public List<Renderer> maskRenderer;

	public bool updateAlways = true;

	private Vector4 m_ClipRect;
	private MaterialPropertyBlock _block;

	// Use this for initialization
	void Start () {
		UpdateClip();
	}

	void LateUpdate () {
		if(updateAlways) UpdateClip();
	}

	public void UpdateClip(){
		if(maskRenderer!=null){
			if (_block==null) _block = new MaterialPropertyBlock();

			maskSize.width = Mathf.Max(0.0001f,maskSize.width);
			maskSize.height = Mathf.Max(0.0001f,maskSize.height);

			Vector3 bl = transform.TransformPoint(new Vector3(maskSize.x,maskSize.y,0f));
			Vector3 tr = transform.TransformPoint(new Vector3(maskSize.x + maskSize.width,maskSize.y + maskSize.height,0f));

			for(int i=0;i<maskRenderer.Count;++i){
				Renderer render = maskRenderer[i];
				if(render && render.sharedMaterial) {

					Vector3 bbl = render.transform.InverseTransformPoint(bl);
					Vector3 ttr = render.transform.InverseTransformPoint(tr);

					m_ClipRect.x = bbl.x;
					m_ClipRect.y = bbl.y;

					m_ClipRect.z = ttr.x;
					m_ClipRect.w = ttr.y;

					float angle = transform.InverseTransformDirection(render.transform.eulerAngles-transform.eulerAngles).z;

					render.GetPropertyBlock(_block);
					_block.SetFloat("_Rotation",-angle);
					_block.SetVector("_MaskClip",m_ClipRect);
					render.SetPropertyBlock(_block);
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

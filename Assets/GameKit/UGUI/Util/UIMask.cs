using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIMask : MonoBehaviour {


	public Rect maskSize = new Rect(0f,0f,100f,100f);
	public Material[] maskMaterials;

	private Vector4 m_rect;

	// Use this for initialization
	void Start () {
		Clip();
	}

	void LateUpdate () {
		Clip();
	}

	void Clip(){
		if(maskMaterials!=null){
			m_rect.x = maskSize.x*transform.lossyScale.x+transform.position.x;
			m_rect.y = maskSize.y*transform.lossyScale.y+transform.position.y;
			m_rect.z = maskSize.width*transform.lossyScale.x;
			m_rect.w = maskSize.height*transform.lossyScale.y;
			for(int i=0;i<maskMaterials.Length;++i){
				if(maskMaterials[i])
					maskMaterials[i].SetVector("_UIClipRect",m_rect);
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

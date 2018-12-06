using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public float pw_x;
	public float pw_y;
	public float pw_z;
	public GameObject objs;

	public GameObject cube;
	private float angle = 0;
	private float r = 5;

	// Use this for initialization
	void Start () {
//		Vector3 center = cube.transform.position;
//		for(int i = 0; i < 72; i++){
//			GameObject temp = (GameObject)GameObject.Instantiate (cube);
//			float hudu = (angle / 180) * Mathf.PI;
//			float xx = center.x + r * Mathf.Cos (hudu);
//			float yy = center.x + r * Mathf.Sin (hudu);
//			temp.transform.position = new Vector3 (xx, yy, center.z);
//			temp.transform.LookAt (center);
//			angle = angle + 5;
//		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			for (int i = 0; i < objs.transform.childCount; i++) {
				pw_x = Random.value > 0.5 ? pw_x : -pw_x; 
				pw_z = Random.value > 0.5 ? pw_z : -pw_z; 
				pw_y = Random.Range (20, 30);

				objs.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(pw_x,pw_y,pw_z);
			}
				
		}
	}
}

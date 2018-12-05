using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public float pw_x;
	public float pw_y;
	public float pw_z;
	public GameObject objs;

	// Use this for initialization
	void Start () {
		
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

			Debug.Log("按住Up");
		}
	}
}

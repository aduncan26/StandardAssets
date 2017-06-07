using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryFollow : MonoBehaviour {

	[SerializeField] Transform playerTrans;

	bool playerLanded = false;
	
	// Update is called once per frame
	void Update () {
		if (!playerLanded) {
			transform.parent.position = new Vector3 (playerTrans.position.x, transform.parent.position.y, playerTrans.position.z);
		} else {
			this.enabled = false;
		}
	}

	void OnCollisionEnter(Collision col){
		if (col.transform == playerTrans)
			playerLanded = true;
	}
}

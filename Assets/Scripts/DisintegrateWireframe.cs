using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateWireframe : MonoBehaviour {

	float t = 0;
	const float MAX_DISINT = 40f;
	float step = 0;

	const float TIME_ADVANCE = 0.002f;

	[SerializeField] float minWait = 2f;
	[SerializeField] float maxWait = 3f;
	[SerializeField] float deteriorateSpeed = 0.9f;

	[SerializeField] GameObject gallery;
	[SerializeField] GameObject terrain;

	public AnimationCurve curve;

	bool disintegrate = true;

	float lineThickness = 1f;
	[SerializeField] float lineSlimSpeed = 0.01f;

	void Awake(){
		Shader.SetGlobalFloat ("_DisintegrationX", 0);
		Shader.SetGlobalFloat ("_DisintegrationY", 0);
		Shader.SetGlobalFloat ("_DisintegrationZ", 0);
		Shader.SetGlobalFloat ("_Thickness", lineThickness);
	}

	// Use this for initialization
	void OnEnable () {
	}

	void Update(){

		if (disintegrate) {
			Disintegrate ();
		} else {
			SlimLines ();
		}

	}

	IEnumerator ChangeGlobalShaderFloat(){
		yield return new WaitForSeconds (Random.Range(minWait, maxWait));

//		float x = Random.Range (t, t + 1);
//		float y = Random.Range (t, t + 1);
//		float z = Random.Range (t, t + 1);
//
//		Shader.SetGlobalFloat ("_DisintegrationX", x);
//		Shader.SetGlobalFloat ("_DisintegrationY", y);
//		Shader.SetGlobalFloat ("_DisintegrationZ", z);
//
//		HangGame.GetInstance ().TakeSnapshot ();
//
//		yield return new WaitForSeconds(0.1f);

//		HangGame.GetInstance ().StopHanging ();

		Shader.SetGlobalFloat ("_DisintegrationX", t);
		Shader.SetGlobalFloat ("_DisintegrationY", t);
		Shader.SetGlobalFloat ("_DisintegrationZ", t);

//		minWait = Mathf.Clamp (minWait * deteriorateSpeed, 0.02f, Mathf.Infinity);
//		maxWait = Mathf.Clamp (maxWait * deteriorateSpeed, 0.03f, Mathf.Infinity);

		if (t < MAX_DISINT) {
			step += TIME_ADVANCE;

			t = curve.Evaluate (step) * (MAX_DISINT + 1);

			StartCoroutine (ChangeGlobalShaderFloat ());

		} else {
			t = MAX_DISINT;
			DropToGallery ();
		}

	}

	void Disintegrate(){
		Shader.SetGlobalFloat ("_DisintegrationX", t);
		Shader.SetGlobalFloat ("_DisintegrationY", t);
		Shader.SetGlobalFloat ("_DisintegrationZ", t);

		if (t < MAX_DISINT) {
			step += TIME_ADVANCE;

			StripMaterials.GetInstance ().FadeGrassTextures ((1f - step) * 2);

			t = curve.Evaluate (step) * (MAX_DISINT + 10);
		
		} else {
			t = MAX_DISINT;
			disintegrate = false;
		}
	}

	void SlimLines(){
		lineThickness = Mathf.Clamp01(lineThickness - lineSlimSpeed);
		Shader.SetGlobalFloat ("_Thickness", lineThickness);
		StripMaterials.GetInstance ().FadeGrassTextures (0);

		if (lineThickness == 0) {
			DropToGallery ();
		}
	}


	void DropToGallery(){
		gallery.gameObject.SetActive (true);
		terrain.gameObject.SetActive (false);
		this.enabled = false;
	}

	void OnDisable(){
		Shader.SetGlobalFloat ("_DisintegrationX", 0);
		Shader.SetGlobalFloat ("_DisintegrationY", 0);
		Shader.SetGlobalFloat ("_DisintegrationZ", 0);
		Shader.SetGlobalFloat ("_Thickness", 1);

	}
}

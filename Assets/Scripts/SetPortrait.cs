using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetPortrait : MonoBehaviour {

	private Texture texture;

	private Text writeUpText;


	// Use this for initialization
	void Start () {
		texture = GetComponent<Renderer> ().sharedMaterial.mainTexture;

		ResizeObjects ();

		SetCanvas ();

		GetComponentInChildren<Text> ().text = MakeTextParagraphs(texture.name);

	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void ResizeObjects(){
		Transform canvasChild = transform.GetChild (0);

		Vector3 textureScale = new Vector3 (texture.width, texture.height, 1);

		if (textureScale.x > textureScale.y) {
			transform.localScale = new Vector3 (2, 2.0f * textureScale.y / textureScale.x, 2);

			canvasChild.localScale = new Vector3(0.1f, 0.1f / (textureScale.y/textureScale.x), 0.1f);
		} else {
			transform.localScale = new Vector3 (2.0f * textureScale.x / textureScale.y, 2, 2);

			canvasChild.localScale = new Vector3(0.1f / (textureScale.x/textureScale.y), 0.1f, 0.1f);
		}
	}

	void SetCanvas(){
		CanvasScaler canvas = GetComponentInChildren<CanvasScaler> ();

		canvas.dynamicPixelsPerUnit = 5;

		Text canvasText = GetComponentInChildren<Text> ();

		canvasText.fontSize = 10;
		canvasText.color = Color.black;
	}

	string MakeTextParagraphs(string textToChange){
		char[] splitter = new char[]{ '_' };

		string[] splitString = textToChange.Split (splitter);

		string returnString = "";

		for (int i = 0; i < splitString.Length; i++) {
			returnString += splitString [i];
			returnString += " ";
		}

		return returnString;
	}
}

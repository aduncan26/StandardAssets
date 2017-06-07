using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangGame : MonoBehaviour {

	bool hang = false;
	private Material material;

	bool takePicture = false;

	private static HangGame instance;
	public static HangGame GetInstance(){
		return instance;
	}

	// Creates a private material used to the effect
	void Awake ()
	{
		instance = this;

		material = new Material( Shader.Find("Hidden/FreezeCamera") );
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (!hang)
		{
			Graphics.Blit (source, destination);
			return;
		}

		Graphics.Blit (source, destination, material);
	}

	void OnPostRender(){
		if (takePicture) {
			int width = Screen.width;
			int height = Screen.height;
			Texture2D texture = new Texture2D (width, height, TextureFormat.RGB24, true);
			texture.ReadPixels (new Rect (0, 0, width, height), 0, 0);
			texture.Apply ();

			material.SetTexture("_ScreenShotTex", texture);
			material.SetFloat ("_ScreenShot", 1);

			hang = true;
			takePicture = false;
		}
	}

	public void TakeSnapshot() {
		takePicture = true;
	}

	public void StopHanging(){
		hang = false;
		material.SetFloat ("_ScreenShot", 0);

	}
}

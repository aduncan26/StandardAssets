using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripMaterials : MonoBehaviour {

	[SerializeField] Terrain terrain;

	int baseInt = 0;
	float storeChange = 0; 
	[SerializeField] float changeSpeed = 0.1f;

	//Trees
	[SerializeField] GameObject[] conifers;
	[SerializeField] GameObject[] broadleafs;
	[SerializeField] GameObject[] palms;

	GameObject[][] allTrees;

	TreePrototype[] treesProto;

	//Grass
	[SerializeField] Texture2D[] grassOneTextures;
	[SerializeField] Texture2D[] grassTwoTextures;

	Texture2D[][] allGrassTextures;

	[SerializeField] Color initDryColor;
	[SerializeField] Color initHealthyColor;

	DetailPrototype[] detailsProto;

	//Ground
	[SerializeField] Material[] groundMaterials;

	//Sky

	//Water
	[SerializeField] Renderer[] waterRends;
	[SerializeField] Material[] waterChangeMaterials;
	Material[] storeMats;

	[SerializeField] float waitTimeMin = 4f;
	[SerializeField] float waitTimeMax = 8f;
	[SerializeField] float waitTimeModifier = 0.95f;

	[SerializeField] float frameWaitTime = 0.5f;

	[SerializeField] float maxHangTime = 0.8f;
	[SerializeField] float minHangTime = 0.3f;

	DisintegrateWireframe disintegrateScript;

	AudioSource audioSource;
	float storeVolume;

	private static StripMaterials instance;
	public static StripMaterials GetInstance(){
		return instance;
	}

	void Awake(){
		instance = this;
		storeMats = new Material[waterRends.Length];

		for (int i = 0; i < storeMats.Length; i++) {
			storeMats [i] = waterRends [i].material;
		}

		disintegrateScript = GetComponent<DisintegrateWireframe> ();
		disintegrateScript.enabled = false;

		audioSource = GetComponent<AudioSource> ();
		storeVolume = audioSource.volume;
	}

	// Use this for initialization
	void Start () {
		treesProto = terrain.terrainData.treePrototypes;
		detailsProto = terrain.terrainData.detailPrototypes;

		allTrees = new GameObject[3][];
		allTrees [0] = conifers;
		allTrees [1] = broadleafs;
		allTrees [2] = palms;

		allGrassTextures = new Texture2D[2][];
		allGrassTextures [0] = grassOneTextures;
		allGrassTextures [1] = grassTwoTextures;

		StartCoroutine (FlickerTerrainData ());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
		}
	}

	#region TREES
	IEnumerator FlickerTrees(){
		HangGame.GetInstance ().TakeSnapshot ();

		float hangTime = Random.Range (minHangTime, maxHangTime);
		audioSource.volume = 0f;

		yield return new WaitForSeconds (hangTime);

		HangGame.GetInstance ().StopHanging ();
		audioSource.volume = storeVolume;

		int treeTypeToChange = Random.Range (0, allTrees.Length);
		int randomTree = Random.Range (1 + baseInt, allTrees [treeTypeToChange].Length);

		if (randomTree >= allTrees [treeTypeToChange].Length) {
			randomTree--;
		}

		GameObject switchTree = allTrees [treeTypeToChange] [randomTree];

		SwitchTrees (terrain.terrainData, treeTypeToChange, switchTree);

		yield return new WaitForSeconds (frameWaitTime);

		SwitchTreesBack (terrain.terrainData, treeTypeToChange);

		int randCycle = Random.Range (0, 4);

		for (int i = 0; i < randCycle; i++) {
			audioSource.volume = 0;
			SwitchTrees (terrain.terrainData, treeTypeToChange, switchTree);
			yield return new WaitForSeconds (frameWaitTime);
			audioSource.volume = storeVolume;
			SwitchTreesBack (terrain.terrainData, treeTypeToChange);
			yield return new WaitForSeconds (frameWaitTime);
		}
	}


	void SwitchTrees(TerrainData terrainData, int treeNumberFrom, GameObject treeTo){

		treesProto [treeNumberFrom].prefab = treeTo;
		terrainData.treePrototypes = treesProto;

		terrainData.RefreshPrototypes ();
	}

	void SwitchTreesBack(TerrainData terrainData, int treeNumberFrom){

		treesProto [treeNumberFrom].prefab = allTrees[treeNumberFrom][baseInt];
		terrainData.treePrototypes = treesProto;

		terrainData.RefreshPrototypes ();
	}

	#endregion

	#region GRASS
	IEnumerator FlickerGrass(){
		HangGame.GetInstance ().TakeSnapshot ();

		float hangTime = Random.Range (minHangTime, maxHangTime);
		audioSource.volume = 0f;

		yield return new WaitForSeconds (hangTime);

		HangGame.GetInstance ().StopHanging ();
		audioSource.volume = storeVolume;

		int grassTypeToChange = Random.Range (0, allGrassTextures.Length);
		int randomGrass = Random.Range (1 + baseInt, allGrassTextures [grassTypeToChange].Length);

		if (randomGrass >= allGrassTextures [grassTypeToChange].Length) {
			randomGrass --;
		}

		Texture2D switchGrass = allGrassTextures [grassTypeToChange] [randomGrass];;

		SwitchGrass (terrain.terrainData, grassTypeToChange, switchGrass, randomGrass);

		yield return new WaitForSeconds (frameWaitTime);

		SwitchGrassBack (terrain.terrainData, grassTypeToChange);

		int randCycle = Random.Range (0, 4);

		for (int i = 0; i < randCycle; i++) {
			audioSource.volume = 0f;
			SwitchGrass (terrain.terrainData, grassTypeToChange, switchGrass, randomGrass);
			yield return new WaitForSeconds (frameWaitTime);
			audioSource.volume = storeVolume;
			SwitchGrassBack (terrain.terrainData, grassTypeToChange);
			yield return new WaitForSeconds (frameWaitTime);
		}
	}

	void SwitchGrass(TerrainData terrainData, int grassTypeFrom, Texture2D grassTo, int grassIndex){

		detailsProto [grassTypeFrom].prototypeTexture = grassTo;
		if (grassIndex == allGrassTextures [grassTypeFrom].Length - 1) {
			detailsProto [grassTypeFrom].dryColor = Color.black;
			detailsProto [grassTypeFrom].healthyColor = Color.black;
		} else {
			detailsProto [grassTypeFrom].dryColor = Color.white;
			detailsProto [grassTypeFrom].healthyColor = Color.white;
		}
		terrainData.detailPrototypes = detailsProto;

		terrainData.RefreshPrototypes ();
	}

	void SwitchGrassBack(TerrainData terrainData, int grassTypeFrom){

		detailsProto [grassTypeFrom].prototypeTexture = allGrassTextures[grassTypeFrom][baseInt];
		if (baseInt == 0) {
			detailsProto [grassTypeFrom].dryColor = initDryColor;
			detailsProto [grassTypeFrom].healthyColor = initHealthyColor;
		} else if(baseInt == allGrassTextures[grassTypeFrom].Length - 1){
			detailsProto [grassTypeFrom].dryColor = Color.black;
			detailsProto [grassTypeFrom].healthyColor = Color.black;
		} else {
			detailsProto [grassTypeFrom].dryColor = Color.white;
			detailsProto [grassTypeFrom].healthyColor = Color.white;
		}
		terrainData.detailPrototypes = detailsProto;

		terrainData.RefreshPrototypes ();
	}

	#endregion

	#region GROUND

	IEnumerator FlickerGround(){
		HangGame.GetInstance ().TakeSnapshot ();

		float hangTime = Random.Range (minHangTime, maxHangTime);
		audioSource.volume = 0f;

		yield return new WaitForSeconds (hangTime);

		HangGame.GetInstance ().StopHanging ();
		audioSource.volume = storeVolume;

		int randomGround = Random.Range (baseInt, groundMaterials.Length);

		if (randomGround >= groundMaterials.Length) {
			randomGround --;
		}

		SwitchGroundMaterial (groundMaterials[randomGround]);

		yield return new WaitForSeconds (frameWaitTime);

		SwitchGroundMaterialBack ();

		int randCycle = Random.Range (0, 4);

		for (int i = 0; i < randCycle; i++) {
			audioSource.volume = 0f;
			SwitchGroundMaterial (groundMaterials[randomGround]);
			yield return new WaitForSeconds (frameWaitTime);
			audioSource.volume = storeVolume;
			SwitchGroundMaterialBack ();
			yield return new WaitForSeconds (frameWaitTime);
		}
	}

	void SwitchGroundMaterial(Material newMaterial){
		terrain.materialType = Terrain.MaterialType.Custom;
		terrain.materialTemplate = newMaterial;
	}

	void SwitchGroundMaterialBack(){
		if (baseInt == 0) {
			terrain.materialType = Terrain.MaterialType.BuiltInLegacySpecular;
		} else {
			terrain.materialTemplate = groundMaterials [baseInt - 1];
		}
	}

	#endregion

	#region WATER
	IEnumerator FlickerWater(){
		HangGame.GetInstance ().TakeSnapshot ();

		float hangTime = Random.Range (minHangTime, maxHangTime);
		audioSource.volume = 0f;

		yield return new WaitForSeconds (hangTime);

		HangGame.GetInstance ().StopHanging ();		
		audioSource.volume = storeVolume;

		int rand = Random.Range (baseInt, waterChangeMaterials.Length);

		if (rand >= waterChangeMaterials.Length) {
			rand--;
		}

		Material randomShader = waterChangeMaterials [rand];

		SwitchWater (randomShader);

		yield return new WaitForSeconds (frameWaitTime);

		SwitchWaterBack ();

		int randCycle = Random.Range (0, 4);

		for (int i = 0; i < randCycle; i++) {
			audioSource.volume = 0f;
			SwitchWater (randomShader);
			yield return new WaitForSeconds (frameWaitTime);
			audioSource.volume = storeVolume;
			SwitchWaterBack ();
			yield return new WaitForSeconds (frameWaitTime);
		}
	}


	void SwitchWater(Material newMat){
		for (int i = 0; i < waterRends.Length; i++) {
			waterRends [i].material = newMat;
		}
	}

	void SwitchWaterBack(){
		if (baseInt == 0) {
			for (int i = 0; i < waterRends.Length; i++) {
				waterRends [i].material = storeMats [i];
			}
		} else {
			for (int i = 0; i < waterRends.Length; i++) {
				waterRends [i].material = waterChangeMaterials[baseInt - 1];
			}
		}
	}

	#endregion

	IEnumerator FlickerTerrainData(){
		float randomTime = Random.Range (waitTimeMin, waitTimeMax);
		yield return new WaitForSeconds (randomTime);
	
		int numElementsToChange = Random.Range (1, 4);
		int whatToChange = -1;


		for (int i = 0; i < numElementsToChange; i++) {
			int rand;
			do {
				rand = Random.Range (0, 4);
			} while(rand == whatToChange);
			whatToChange = rand;

			switch (whatToChange) {
			case 0:
				StartCoroutine (FlickerTrees ());
				break;
			case 1:
				StartCoroutine (FlickerGrass ());
				break;
			case 2:
				StartCoroutine (FlickerGround ());
				break;
			case 3:
				StartCoroutine (FlickerWater ());
				break;
			}
		}

		storeChange += changeSpeed;
		baseInt = Mathf.FloorToInt (storeChange);

		waitTimeMin = Mathf.Clamp (waitTimeMin * waitTimeModifier, 1, Mathf.Infinity);
		waitTimeMax = Mathf.Clamp (waitTimeMax * waitTimeModifier, 1, Mathf.Infinity);

		if (baseInt < 4) {
			StartCoroutine (FlickerTerrainData ());
		} else {
			baseInt = 3;
			StartDisintegration ();
		}

	}

	void StartDisintegration(){
		for(int i = 0; i < allTrees.Length; i++){
			SwitchTrees(terrain.terrainData, i, allTrees[i][allTrees[i].Length - 1]); 
		}

		for (int i = 0; i < allGrassTextures.Length; i++) {
			SwitchGrass (terrain.terrainData, i, allGrassTextures [i] [allGrassTextures [i].Length - 1], allGrassTextures [i].Length - 1);
		}

		SwitchGroundMaterial (groundMaterials [groundMaterials.Length - 1]);

		SwitchWater (waterChangeMaterials [waterChangeMaterials.Length - 1]);

		disintegrateScript.enabled = true;
	}

	public void FadeGrassTextures(float alpha){
		for (int i = 0; i < detailsProto.Length; i++) {
			detailsProto [i].maxWidth = alpha;
			detailsProto [i].minWidth = alpha/2;

			detailsProto [i].maxHeight = alpha;
			detailsProto [i].minHeight = alpha/2;

		}
		terrain.terrainData.detailPrototypes = detailsProto;

		terrain.terrainData.RefreshPrototypes ();
	}
			

	public void ResetTerrain(){
		storeChange = 0f;
		baseInt = 0;

		for(int i = 0; i < allTrees.Length; i++){
			SwitchTreesBack (terrain.terrainData, i);
		}

		for (int i = 0; i < detailsProto.Length; i++) {
			detailsProto [i].maxWidth = 2;
			detailsProto [i].minWidth = 1;
			detailsProto [i].maxHeight = 2;
			detailsProto [i].minHeight = 1;
		}

		for (int i = 0; i < allGrassTextures.Length; i++) {
			SwitchGrassBack (terrain.terrainData, i);
		}

		SwitchGroundMaterialBack ();

		SwitchWaterBack ();


	}

}

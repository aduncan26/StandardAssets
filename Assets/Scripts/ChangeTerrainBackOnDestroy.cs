using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTerrainBackOnDestroy : MonoBehaviour {

	void OnDestroy(){
		StripMaterials.GetInstance ().ResetTerrain ();
	}
}

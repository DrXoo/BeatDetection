using UnityEngine;
using System.Collections;

public class CubesManager : MonoBehaviour {

	public CubeSound prefabObject;
	public SubbandBeatDetection selectedBeatDetection;

	public int cubesToSpawn;

	void Awake(){

		int j = 0;
		for (int i = 0; i < cubesToSpawn; i++) {
			CubeSound newCube = Instantiate<CubeSound> (prefabObject);
			int[] toEar = { j, j+1 };
			j += 2;
			newCube.selectedBeatDetection = selectedBeatDetection;
			newCube.subbandsToEar = toEar;
			newCube.smoothnessChange = 2f;


			newCube.transform.position = Vector3.right * (-cubesToSpawn + i * 2);
			newCube.transform.SetParent (transform);
		}
	}
}

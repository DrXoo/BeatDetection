using UnityEngine;
using System.Collections;

public class CubeSound : MonoBehaviour {

	public SubbandBeatDetection selectedBeatDetection;
	public int[] subbandsToEar;

	private MeshRenderer myMeshRenderer;
	private Color beatedColor;
	public float smoothnessChange;

	void Start(){
		for (int i = 0; i < subbandsToEar.Length; i++) {
			selectedBeatDetection.subBands [subbandsToEar [i]].OnBeat+=OnBeat;
		}

		myMeshRenderer = GetComponent<MeshRenderer> ();
		beatedColor = Color.black;
	}

		
	void Update(){
		beatedColor = Color.Lerp (beatedColor, Color.black, smoothnessChange*Time.deltaTime);

		myMeshRenderer.material.color = beatedColor;
	}

	void OnBeat(){
		beatedColor = Color.yellow;
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class SimpleBeatDetection : MonoBehaviour {
	
	public AudioSource audioSource;

	public delegate void OnBeatHandler();
	public event OnBeatHandler OnBeat; 

	// These variables are meant to get the left and right channels of the music
	private float[] samples0Channel;
	private float[] samples1Channel;

	// Inside the History buffer are the previous instant energies
	// Using a history buffer saves a lot of CPU
	// Refer to 1.2 in references to get more info
	private float[] historyBuffer;

	// The size of our sampled arrays. Usually : 1024
	public int bufferSize;

	// The type of window we are going to apply at the Spectrum
	public FFTWindow FFTWindow;

	void Start () {

		// Initialize all things
		samples0Channel = new float[bufferSize];
		samples1Channel = new float[bufferSize];
		historyBuffer = new float[43];
	}

	void Update () {

		// Get the instant energy of the song this frame
		float instantEnergy = GetInstantEnergy ();

		// Use the History Buffer to compute the average energy of the sound the past 1 second
		// Refer to reference to more info
		float localAverageEnergy = GetLocalAverageEnergy ();

		// Compute the variance with the history buffer and the average energy
		float varianceEnergies = ComputeVariance (localAverageEnergy);

		// Use the linear equation described in the reference to compute the constant C
		double constantC = (-0.0025714 * varianceEnergies) + 1.5142857;

		// Shift the history buffer one position to the right
		// Save it in another array
		float[] shiftedHistoryBuffer = ShiftArray (historyBuffer, 1);

		// Make the first position to be our instantEnergy
		shiftedHistoryBuffer [0] = instantEnergy;

		// Override the elements of the new array on the old one
		OverrideElementsToAnotherArray (shiftedHistoryBuffer, historyBuffer);

		// Ask if a beat has been done
		if (instantEnergy > constantC * localAverageEnergy) {
			// Beat!
			if(OnBeat != null)
				OnBeat();
		}


	}

	#region FOR_SIMPLE_ALGORITHM_USE
	public float GetInstantEnergy(){

		float result = 0;

		// Fill the samples arrays
		// Each value of these arrays are floats between 0 and 1
		// and are the frequency percentage of use
		audioSource.GetSpectrumData (samples0Channel, 0, FFTWindow);
		audioSource.GetSpectrumData (samples1Channel, 1, FFTWindow);

		// Refer to reference: e = sum(a[i]^2 + b[i]^2)
		for (int i = 0; i < bufferSize; i++) {
			result += (float) System.Math.Pow(samples0Channel [i],2)  + (float) System.Math.Pow(samples1Channel [i],2);
		}

		return result;
	}

	private float GetLocalAverageEnergy(){
		float result = 0; 

		// Refer to reference: <E> = sum(E[i]^2) / 43

		for (int i = 0; i < historyBuffer.Length; i++) {
			// TODO See if getting out the square of this element works better
			result += historyBuffer [i];
		}

		return result / historyBuffer.Length;
	}

	private float ComputeVariance(float _averageEnery){
		float result = 0; 

		// Refer to reference: V = sum( (E[i] - <E>)^2 ) / 43

		for (int i = 0; i < historyBuffer.Length; i++) {
			result += (float) System.Math.Pow (  historyBuffer[i] - _averageEnery , 2);
		}

		return result / historyBuffer.Length;
	}

	#endregion

	#region UTILITY_USE
	private void OverrideElementsToAnotherArray(float[] _from, float[] _to){
		for (int i = 0; i < _from.Length; i++) {
			_to [i] = _from [i];
		}
	}

	private float[] ShiftArray(float[] _array, int amount){

		float[] result = new float[_array.Length];

		for (int i = 0; i < _array.Length - amount; i++) {
			result [i + amount] = _array [i];
		}

		return result;

	}

	private string historybuffer() {
		string s = "";
		for (int i = 0; i<historyBuffer.Length; i++) {
			s += (historyBuffer[i] + ",");
		}
		return s;
	}

	#endregion
}

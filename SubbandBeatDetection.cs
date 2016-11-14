using UnityEngine;
using System.Collections;

public class SubbandBeatDetection : MonoBehaviour {

	#region PUBLIC_VARIABLES
	public AudioSource 	audioSource;

	[Header("Algorithm Configuration")]
	public int 			numSubbands;
	public int 			numSamples;

	public FFTWindow 	FFTWindow;
	#endregion

	#region PRIVATE_VARIABLES
	private float[] samplesLeftChannel;
	private float[] samplesRightChannel;

	public SubBand[] subBands { get; set;}
	#endregion

	#region CONSTANTS
	// These two variables are meant to be use at our linear calculation of the subband's with
	private const float a = 0.44f;
	private const float b = 1.56f;

	private const int 	beatSensivity = 5;
	private const float varianceSensivity = 0.00001f;
	#endregion

	void Awake(){

		// Initialize arrays
		samplesLeftChannel = new float[numSamples];
		samplesRightChannel = new float[numSamples];
		subBands = new SubBand[numSubbands];

		// Create each subbband
		for (int i = 0; i < subBands.Length; i++) {
			subBands [i] = new SubBand (i + 1);
		}


	}

	void Update () {
		
		// Fill the samples arrays with data
		audioSource.GetSpectrumData (samplesLeftChannel, 0, FFTWindow);
		audioSource.GetSpectrumData (samplesRightChannel, 1, FFTWindow);

		for (int i = 0; i < subBands.Length; i++) {
			
			int startPoint = 0;
			for (int j = 0; j <= i - 1; j++) {
				startPoint += subBands [j].frequencyWidth;
			}
			int endPoint = 0;
			for (int j = 0; j <= i; j++) {
				endPoint += subBands [j].frequencyWidth;
			}
				

			subBands [i].ComputeInstantEnergy (startPoint, endPoint, samplesLeftChannel, samplesRightChannel);
			subBands [i].ComputeAverageEnergy ();
			subBands [i].ComputeInstantVariance ();
	
			subBands [i].hasBeated ();

		}
	}

	public class SubBand{

		// For Event use
		public delegate void OnBeatHandler();
		public event OnBeatHandler OnBeat;

		// Our calculated variables each frame
		public float instantEnergy;
		public float averageEnergy;
		public float instantVariance;

		// The frequency width we are going to use for this subband
		public int frequencyWidth;
		// The history buffer of the previous 43s instant energies
		private float[] historyBuffer;

		public SubBand(int _index){
			// Calculate the width using a linear equation and our a and b constants
			// Refer to reference to know more
			frequencyWidth = Mathf.RoundToInt (a * _index + b);
			// Initialize our historybuffer
			historyBuffer = new float[43];
		}


		public void ComputeInstantEnergy(int _start, int _end, float[] _samples0, float[] _samples1){
			float result = 0;
			// Start and End means where we are going to do the calculations
			// since each subband works on a certain part of our 1024 samples array
			// Go to reference to know more
			for (int i = _start; i < _end; i++) {
				result += (float) System.Math.Pow(_samples0 [i],2)  + (float) System.Math.Pow(_samples1 [i],2);;
			}
				

			instantEnergy = result ;
		}

		public void ComputeAverageEnergy(){
			float result = 0;

			for (int i = 0; i < historyBuffer.Length; i++) {
				result += historyBuffer [i];
			}

			averageEnergy = result / historyBuffer.Length;

			// Shift the history buffer one position to the right
			// Save it in another array
			float[] shiftedHistoryBuffer = ShiftArray (historyBuffer, 1);

			// Make the first position to be our instantEnergy
			shiftedHistoryBuffer [0] = instantEnergy;

			// Override the elements of the new array on the old one
			OverrideElementsToAnotherArray (shiftedHistoryBuffer, historyBuffer);

		}

		public void ComputeInstantVariance(){
			float result = 0; 

			for (int i = 0; i < historyBuffer.Length; i++) {
				result += (float) System.Math.Pow (  historyBuffer[i] - averageEnergy , 2);
			}

			instantVariance = result / historyBuffer.Length;
		}

		public void hasBeated(){
			if (instantEnergy > (beatSensivity * averageEnergy) && instantVariance > varianceSensivity) {
				if(OnBeat != null)
					OnBeat ();
			}
				
		}

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
		#endregion
	}
}

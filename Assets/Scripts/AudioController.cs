using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour 
{
	public Transform musicLayersParent;
	public AudioSource[] musicLayers;

	private float _currentTime = 0;
	private float _maxTime = 120;

	private float _musicComplexityNormal = 0;
	private float _layerNormal = 0;

	public AnimationCurve volumeIncreaseCurve;
	private AnimationCurve _volCurveByLayerNorm;

	// Use this for initialization
	void Start ()
	{
		musicLayers = musicLayersParent.GetComponentsInChildren<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		_currentTime += Time.deltaTime;

		_musicComplexityNormal = Mathf.Min(_currentTime/_maxTime , 1);
		_volCurveByLayerNorm = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,_musicComplexityNormal));

		for (int i =0; i < musicLayers.Length; i++)
		{
			_layerNormal = (float)i / (float)musicLayers.Length;
			musicLayers[i].volume = volumeIncreaseCurve.Evaluate( _volCurveByLayerNorm.Evaluate(_layerNormal) );
		}
	}
}

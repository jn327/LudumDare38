using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[System.Serializable]
public class MapLayer : MonoBehaviour
{
	[HideInInspector]
	public int index;
	public PositionValueWeight[] curveValues;
	public Gradient colorGradient;

	private SpriteRenderer _spriteRenderer;
	private Texture2D _texture;
	private bool _validateTexture = false;

	public bool revealAtStart = false;

	public void init( int levelWidth, int levelHeight)
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();

		_texture = new Texture2D(levelWidth, levelHeight, TextureFormat.RGBA32, false);

		_spriteRenderer.sprite = Sprite.Create (_texture, new Rect(0,0,levelWidth,levelHeight), new Vector2(0,0), 1);

		_spriteRenderer.material.name = name + "_sprite";
		_spriteRenderer.material.mainTexture = _texture as Texture;
		_spriteRenderer.material.shader = Shader.Find ("Sprites/Default");
	}

	public float GenerateValueForPos( int x, int y )
	{
		float currValue = 0;
		for (int i = 0; i < curveValues.Length; i++)
		{
			switch (curveValues[i].operationType)
			{
			case PositionValueWeight.OPERATION_TYPES.ADDITION:
				currValue += curveValues[i].getValForType( x, y );
				break;
			case PositionValueWeight.OPERATION_TYPES.SUBTRACTION:
				currValue -= curveValues[i].getValForType( x, y );
				break;
			case PositionValueWeight.OPERATION_TYPES.MULTIPLICATION:
				currValue *= curveValues[i].getValForType( x, y );
				break;
			case PositionValueWeight.OPERATION_TYPES.DIVISION:
				currValue /= curveValues[i].getValForType( x, y );
				break;
			}
		}
		return currValue;
	}

	public void UpdateTexture()
	{
		if (_validateTexture == true)
			ValidateTexture();
	}

	public void setTextureForLevelPos( float gradientVal, int x, int y )
	{
		_texture.SetPixel(x, y, colorGradient.Evaluate(gradientVal));
		_validateTexture = true;
	}

	public void clearTextureForLevelPos ( int x, int y )
	{
		_texture.SetPixel(x, y, new Color(1,1,1,0));
		_validateTexture = true;
	}

	public void ValidateTexture()
	{
		_texture.Apply();
		_spriteRenderer.material.mainTexture = _texture as Texture;
		_validateTexture = false;
	}
}

[System.Serializable]
public class PositionValueWeight
{
	public string name;

	public enum TYPES {NONE, RANDOM, PERLIN, SINX, SINY, COSX, COSY, XPOS, YPOS };
	public TYPES initializationType;
	[Range(0,1)]
	public float weighting = 1; //normalized value 0-1;
	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	public MapLayer otherLayerVal;

	public float perlinNoiseScale = 1;
	public float perlinXOrg = 0;
	public float perlinYOrg = 0;

	public enum OPERATION_TYPES {ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION };
	public OPERATION_TYPES operationType;

	public float getValForType( int x, int y )
	{
		float curveVal = 0;

		switch (initializationType)
		{
		case TYPES.NONE:
			break;
		case TYPES.RANDOM:
			curveVal += Random.value;
			break;
		case TYPES.PERLIN:
			float xCoord = perlinXOrg + (((float)x / (float)LevelGenerator.LEVEL_WIDTH) * perlinNoiseScale);
			float yCoord = perlinYOrg + (((float)y / (float)LevelGenerator.LEVEL_HEIGHT) * perlinNoiseScale);
			curveVal += Mathf.PerlinNoise(xCoord, yCoord);
			break;
		case TYPES.SINX:
			curveVal += Mathf.Sin(x);
			break;
		case TYPES.SINY:
			curveVal += Mathf.Sin(y);
			break;
		case TYPES.COSX:
			curveVal += Mathf.Cos(x);
			break;
		case TYPES.COSY:
			curveVal += Mathf.Cos(y);
			break;
		case TYPES.XPOS:
			curveVal += (float)x/(float)LevelGenerator.LEVEL_WIDTH;
			break;
		case TYPES.YPOS:
			curveVal += (float)y/(float)LevelGenerator.LEVEL_HEIGHT;
			break;
		}

		if (otherLayerVal != null)
		{
			curveVal += LevelGenerator.instance.getMapLayerValueAtPos( x, y, otherLayerVal.index );
		}

		curveVal = curve.Evaluate(curveVal);
		curveVal *= weighting;
		return curveVal;
	}
}
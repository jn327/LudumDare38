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

		for (int i = 0; i < curveValues.Length; i++)
		{
			curveValues[i].init();
		}
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

	public enum TYPES {NONE, RANDOM, PERLIN, SINX, SINY, COSX, COSY, XPOS, YPOS, PERLINYPOS, PERLINXPOS };
	public TYPES initializationType;
	[Range(0,1)]
	public float weighting = 1; //normalized value 0-1;
	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	public MapLayer otherLayerVal;

	private float _perlinNoiseScale;
	public float perlinNoiseScaleMin = 1;
	public float perlinNoiseScaleMax = 1;
	public float perlinXOrg = 0;
	public float perlinYOrg = 0;

	public enum OPERATION_TYPES {ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION };
	public OPERATION_TYPES operationType;

	public void init()
	{
		_perlinNoiseScale = Random.Range(perlinNoiseScaleMin, perlinNoiseScaleMax);
	}

	public float getValForType( int x, int y )
	{
		float curveVal = 0;

		if (initializationType == TYPES.NONE)
		{
		}
		else if (initializationType == TYPES.RANDOM)
		{
			curveVal += Random.value;
		}
		else if (initializationType == TYPES.PERLIN)
		{
			float xCoord = perlinXOrg + (((float)x / (float)LevelGenerator.LEVEL_WIDTH) * _perlinNoiseScale);
			float yCoord = perlinYOrg + (((float)y / (float)LevelGenerator.LEVEL_HEIGHT) * _perlinNoiseScale);
			curveVal += Mathf.PerlinNoise(xCoord, yCoord);
		}
		else if (initializationType == TYPES.SINX)
		{
			curveVal += Mathf.Sin(x);
		}
		else if (initializationType == TYPES.SINY)
		{
			curveVal += Mathf.Sin(y);
		}
		else if (initializationType == TYPES.COSX)
		{
			curveVal += Mathf.Cos(x);
		}
		else if (initializationType == TYPES.COSY)
		{
			curveVal += Mathf.Cos(y);
		}
		else if (initializationType == TYPES.XPOS)
		{
			curveVal += (float)x/(float)LevelGenerator.LEVEL_WIDTH;
		}
		else if (initializationType == TYPES.YPOS)
		{
			curveVal += (float)y/(float)LevelGenerator.LEVEL_HEIGHT;
		}
		else if (initializationType == TYPES.PERLINXPOS)
		{
			float xCoord = perlinXOrg + (((float)x / (float)LevelGenerator.LEVEL_WIDTH) * _perlinNoiseScale);
			float yCoord = perlinYOrg + (((float)y / (float)LevelGenerator.LEVEL_HEIGHT) * _perlinNoiseScale);
			curveVal += Mathf.PerlinNoise(xCoord, yCoord);
			curveVal *= curve.Evaluate((float)x/(float)LevelGenerator.LEVEL_WIDTH);

		}
		else if (initializationType == TYPES.PERLINYPOS)
		{
			float xCoord = perlinXOrg + (((float)x / (float)LevelGenerator.LEVEL_WIDTH) * _perlinNoiseScale);
			float yCoord = perlinYOrg + (((float)y / (float)LevelGenerator.LEVEL_HEIGHT) * _perlinNoiseScale);
			curveVal += Mathf.PerlinNoise(yCoord, xCoord);
			curveVal *= curve.Evaluate((float)y/(float)LevelGenerator.LEVEL_HEIGHT);
		}

		if (otherLayerVal != null)
		{
			curveVal += LevelGenerator.instance.getMapLayerValueAtPos( x, y, otherLayerVal.index );
		}

		if (initializationType != TYPES.PERLINYPOS && initializationType != TYPES.PERLINXPOS)
		{
			curveVal = curve.Evaluate(curveVal);
		}
		curveVal *= weighting;
		return curveVal;
	}
}
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

	public bool revealAtStart = false;

	public void init( )
	{
		for (int i = 0; i < curveValues.Length; i++)
		{
			curveValues[i].init();
		}
	}

	public float GenerateValueForPos( int x, int y, int xOffset, int yOffset )
	{
		float currValue = 0;
		float valueForType;
		for (int i = 0; i < curveValues.Length; i++)
		{
			valueForType = curveValues[i].getValForType( x, y, xOffset, yOffset );
			switch (curveValues[i].operationType)
			{
			case PositionValueWeight.OPERATION_TYPES.ADDITION:
				currValue += valueForType;
				break;
			case PositionValueWeight.OPERATION_TYPES.SUBTRACTION:
				currValue -= valueForType;
				break;
			case PositionValueWeight.OPERATION_TYPES.MULTIPLICATION:
				currValue *= valueForType;
				break;
			case PositionValueWeight.OPERATION_TYPES.DIVISION:
				currValue /= valueForType;
				break;
			}
		}
		return currValue;
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

	public void init( )
	{
		_perlinNoiseScale = Random.Range(perlinNoiseScaleMin, perlinNoiseScaleMax);
	}

	public float getValForType( int x, int y, int xOffset, int yOffset )
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
			float xCoord = perlinXOrg + (((float)(x + xOffset) / (float)(LevelGenerator.SECTION_WIDTH * LevelGenerator.LEVEL_WIDTH)) * _perlinNoiseScale); //TODO: Offset based on section pos
			float yCoord = perlinYOrg + (((float)(y + yOffset) / (float)(LevelGenerator.SECTION_HEIGHT * LevelGenerator.LEVEL_HEIGHT)) * _perlinNoiseScale); //TODO: Offset based on section pos
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
			curveVal += (float)(x + xOffset) / (float)(LevelGenerator.SECTION_WIDTH * LevelGenerator.LEVEL_WIDTH); //TODO: Offset based on section pos
		}
		else if (initializationType == TYPES.YPOS)
		{
			curveVal += (float)(y + yOffset) / (float)(LevelGenerator.SECTION_HEIGHT * LevelGenerator.LEVEL_HEIGHT); //TODO: Offset based on section pos
		}
		else if (initializationType == TYPES.PERLINXPOS)
		{
			float sectionXNormal = (float)(x + xOffset) / (float)(LevelGenerator.SECTION_WIDTH * LevelGenerator.LEVEL_WIDTH); //TODO: Offset based on section pos
			float xCoord = perlinXOrg + (sectionXNormal * _perlinNoiseScale);
			float yCoord = perlinYOrg + (((float)(y + yOffset) / (float)(LevelGenerator.SECTION_HEIGHT * LevelGenerator.LEVEL_HEIGHT)) * _perlinNoiseScale); //TODO: Offset based on section pos
			curveVal += Mathf.PerlinNoise(xCoord, yCoord);
			curveVal *= curve.Evaluate(sectionXNormal);
		}
		else if (initializationType == TYPES.PERLINYPOS)
		{
			float sectionYNormal = (float)(y + yOffset) / (float)(LevelGenerator.SECTION_HEIGHT * LevelGenerator.LEVEL_HEIGHT); //TODO: Offset based on section pos
			float xCoord = perlinXOrg + (((float)(x + xOffset) / (float)(LevelGenerator.SECTION_WIDTH * LevelGenerator.LEVEL_WIDTH)) * _perlinNoiseScale); //TODO: Offset based on section pos
			float yCoord = perlinYOrg + (sectionYNormal * _perlinNoiseScale);
			curveVal += Mathf.PerlinNoise(yCoord, xCoord);
			curveVal *= curve.Evaluate(sectionYNormal); 
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
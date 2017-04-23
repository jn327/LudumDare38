using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour 
{
	[HideInInspector]
	public SectionLayer[] layers;
	public GameObject layerPrefab;

	private int _xIndex;
	private int _yIndex;
	
	public void init ( int xIndex, int yIndex )
	{
		_xIndex = xIndex;
		_yIndex = yIndex;

		int numLayers = LevelGenerator.instance.mapLayers.Length;
		layers = new SectionLayer[numLayers];
		for (int i =0; i < numLayers; i++)
		{
			layers[i] = Instantiate(layerPrefab, transform.position, Quaternion.identity, transform).GetComponent<SectionLayer>();
			layers[i].init(i);
		}

		generateMap();
	}

	public void Update()
	{
		//TODO: Maybe check this less frequently?
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i].UpdateTexture();
		}
	}

	public void generateMap ( )
	{
		MapLayer mapLayer;
		for (int x = 0; x < LevelGenerator.SECTION_WIDTH; x ++ )
		{
			for (int y = 0; y < LevelGenerator.SECTION_HEIGHT; y ++ )
			{
				//levelPos = levelPositions[x,y] = new LevelPosition();
				//levelPos.layerValues = new float[layers.Length];
				for (int i = 0; i < layers.Length; i++)
				{
					mapLayer = LevelGenerator.instance.mapLayers[i];

					if (mapLayer.revealAtStart == true)
					{
						float posValue = mapLayer.GenerateValueForPos(x + _xIndex * LevelGenerator.SECTION_WIDTH, y + _yIndex * LevelGenerator.SECTION_HEIGHT);
						layers[i].setLayerValue ( x, y, posValue);
						layers[i].setTextureForLevelPos( x, y );
					}
					else
					{
						layers[i].setLayerValue ( x, y, float.NaN);
						layers[i].clearTextureForLevelPos ( x, y );
					}
				}
			}
		}
	}
	
	public float getMapLayerValueAtPos( int x, int y, int layerIndex )
	{
		LevelPosition levelPos = layers[layerIndex].levelPositions[x,y];
		if ( float.IsNaN(levelPos.layerValue) )
		{
			levelPos.layerValue = LevelGenerator.instance.mapLayers[layerIndex].GenerateValueForPos(x + _xIndex * LevelGenerator.SECTION_WIDTH, y + _yIndex * LevelGenerator.SECTION_HEIGHT);
		}
		return levelPos.layerValue;
	}

	public void RevealMapForPosition ( int x, int y )
	{
		if (x >= LevelGenerator.SECTION_WIDTH || y >= LevelGenerator.SECTION_HEIGHT
			|| x < 0 || y < 0 ) 
		{
			Debug.LogAssertion("This shouldnt happen "+x +", "+y);
			return;
		}

		MapLayer mapLayer;
		for (int i = 0; i < layers.Length; i++)
		{
			LevelPosition levelPos = layers[i].levelPositions[x,y];

			//lets not do anything if this position is visible
			if (levelPos.isVisible == true) 
				return;

			levelPos.isVisible = true;


			mapLayer = LevelGenerator.instance.mapLayers[i];
			if ( mapLayer.revealAtStart != true )
			{
				if ( float.IsNaN(levelPos.layerValue) )
				{
					levelPos.layerValue = mapLayer.GenerateValueForPos(x + _xIndex * LevelGenerator.SECTION_WIDTH, y + _yIndex * LevelGenerator.SECTION_HEIGHT);
				}

				layers[i].setTextureForLevelPos( x, y );
			}
		}
	}
}

[System.Serializable]
public class LevelPosition
{
	public bool isVisible = false;
	public float layerValue;
}
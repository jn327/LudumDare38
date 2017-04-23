using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour 
{
	public LevelPosition[,] levelPositions;
	[HideInInspector]
	public SectionLayer[] layers;
	public GameObject layerPrefab;
	
	public void init ( int xIndex, int yIndex )
	{
		int numLayers = LevelGenerator.instance.mapLayers.Length;
		layers = new SectionLayer[numLayers];
		for (int i =0; i < numLayers; i++)
		{
			if (LevelGenerator.instance.mapLayers[i].isActiveAndEnabled)
			{
				layers[i] = Instantiate(layerPrefab, transform.position, Quaternion.identity, transform).GetComponent<SectionLayer>();
				layers[i].init(LevelGenerator.instance.mapLayers[i].colorGradient);
			}
		}

		levelPositions = new LevelPosition[LevelGenerator.SECTION_WIDTH,LevelGenerator.SECTION_HEIGHT];

		generateMap(xIndex, yIndex);
	}

	public void Update()
	{
		//TODO: Maybe check this less frequently?
		for (int i = 0; i < layers.Length; i++)
		{
			if (layers[i].isActiveAndEnabled)
			{
				layers[i].UpdateTexture();
			}
		}
	}

	public void generateMap ( int xIndex, int yIndex )
	{
		LevelPosition levelPos;
		MapLayer mapLayer;
		for (int x = 0; x < LevelGenerator.SECTION_WIDTH; x ++ )
		{
			for (int y = 0; y < LevelGenerator.SECTION_HEIGHT; y ++ )
			{
				levelPos = levelPositions[x,y] = new LevelPosition();
				levelPos.layerValues = new float[layers.Length];
				for (int i = 0; i < layers.Length; i++)
				{
					mapLayer = LevelGenerator.instance.mapLayers[i];
					levelPos.layerValues[i] = mapLayer.GenerateValueForPos(x, y, xIndex * LevelGenerator.SECTION_WIDTH, yIndex * LevelGenerator.SECTION_HEIGHT);

					if (mapLayer.revealAtStart == true && mapLayer.isActiveAndEnabled)
					{
						layers[i].setTextureForLevelPos( levelPos.layerValues[i], x, y );
					}
					else
					{
						layers[i].clearTextureForLevelPos ( x, y );
					}
				}
			}
		}
	}
	
	public float getMapLayerValueAtPos( int x, int y, int layerIndex )
	{
		return levelPositions[x,y].layerValues[layerIndex];
	}

	public void RevealMapForPosition ( int x, int y )
	{
		if (x >= LevelGenerator.SECTION_WIDTH || y >= LevelGenerator.SECTION_HEIGHT
			|| x < 0 || y < 0 ) 
		{
			Debug.LogAssertion("This shouldnt happen "+x +", "+y);
			return;
		}

		LevelPosition levelPos = levelPositions[x,y];

		//lets not do anything if this position is visible
		if (levelPos.isVisible == true) 
			return;

		levelPos.isVisible = true;

		for (int i = 0; i < layers.Length; i++)
		{
			//TODO: ART: DRAW A LINE OR a sea pixel or SOMETHING !!!!
			layers[i].setTextureForLevelPos( levelPos.layerValues[i], x, y );
		}
	}
}

[System.Serializable]
public class LevelPosition
{
	public bool isVisible = false;

	public float[] layerValues;
}
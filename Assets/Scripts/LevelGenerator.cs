using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour 
{
	public const int LEVEL_WIDTH = 1024;
	public const int LEVEL_HEIGHT = 1024;

	public static LevelGenerator instance;

	public LevelPosition[,] levelPositions;

	//	TODO : HEIGHT
	//		http://mapsof.net/the-world/elevation

	//	TODO TEMPERATURE
	//		https://www.nasa.gov/sites/default/files/thumbnails/image/15-115.jpg 
	// > in the centre (55%)
	// bit of sine wave variation (15%)
	// a little perlin (5%)
	// height (25%) (particularly harsh below warterline) (colder near the edges hotter in the centre(0.5???)) ( base this on an animation curve ? )

	//	TODO RAINFALL
	//		http://go.grolier.com/map?id=mtlr080&pid=go
	// temperature map (75%) - perlin (25%)
	private MapLayer[] _mapLayers;

	public GameObject playerPrefab;
	private GameObject player;

	public CameraController camController;

	private Stack mapRevealQueue; //TODO:!:!?

	private void Awake()
	{
		//setup singleton
		if (instance != null)
		{
			GameObject.Destroy(instance.gameObject);
		}
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		_mapLayers = GetComponentsInChildren<MapLayer>(); 
		for (int i = 0; i < _mapLayers.Length; i++)
		{
			_mapLayers[i].init(LEVEL_WIDTH, LEVEL_HEIGHT);
			_mapLayers[i].index = i;
		}

		levelPositions = new LevelPosition[LEVEL_WIDTH,LEVEL_HEIGHT];

		float normalX = 0.5f;
		float normalY = 0.5f;
		if (player == null)
		{
			player = Instantiate(playerPrefab, new Vector3(LEVEL_WIDTH*normalX, LEVEL_HEIGHT*normalY, 0), Quaternion.identity);
		}
		else
		{
			player.transform.position = new Vector3(LEVEL_WIDTH*normalX, LEVEL_HEIGHT*normalY, 0);
		}
		player.GetComponent<PlayerController>().levelGen = this;
		camController.target = player.transform;
		camController.gameObject.transform.position = player.transform.position;

		generateMap();

		//TODO: Should we go ahead and generate a 2d mesh & collider based on heightmap
		
	}

	public void Update()
	{
		//TODO: Maybe check this less frequently?
		for (int i = 0; i < _mapLayers.Length; i++)
		{
			_mapLayers[i].UpdateTexture();
		}
	}

	public void generateMap ()
	{
		LevelPosition levelPos;
		for (int x = 0; x < LEVEL_WIDTH; x ++ )
		{
			for (int y = 0; y < LEVEL_HEIGHT; y ++ )
			{
				levelPos = levelPositions[x,y] = new LevelPosition();
				levelPos.layerValues = new float[_mapLayers.Length];
				for (int i = 0; i < _mapLayers.Length; i++)
				{
					levelPos.layerValues[i] = _mapLayers[i].GenerateValueForPos(x, y);

					if (_mapLayers[i].revealAtStart == true)
					{
						_mapLayers[i].setTextureForLevelPos( levelPos.layerValues[i], x, y );
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
		if (x >= LEVEL_WIDTH || y >= LEVEL_HEIGHT
			|| x < 0 || y < 0 ) 
		{
			Debug.Log("no map to show at "+x+", "+y); 
			return;
		}

		LevelPosition levelPos = levelPositions[x,y];

		//lets not do anything if this position is visible
		if (levelPos.isVisible == true) 
			return;
		
		levelPos.isVisible = true;

		for (int i = 0; i < _mapLayers.Length; i++)
		{
			//TODO: ART: DRAW A LINE OR a sea pixel or SOMETHING !!!!
			_mapLayers[i].setTextureForLevelPos( levelPos.layerValues[i], x, y );
		}
	}
}

[System.Serializable]
public class LevelPosition
{
	public bool isVisible = false;

	public float[] layerValues;
}


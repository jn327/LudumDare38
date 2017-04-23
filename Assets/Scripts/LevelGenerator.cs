using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour 
{
	public const int LEVEL_WIDTH = 64;
	public const int LEVEL_HEIGHT = 64;
	public const int SECTION_WIDTH = 128;
	public const int SECTION_HEIGHT = 128;

	public static LevelGenerator instance;
	public LevelSection levelSectionPrefab;
	[HideInInspector]
	public LevelSection[,] sectionMap;

	//how far we have generated from 0,0.
	private int _currGeneratedXMin = 0;
	private int _currGeneratedXMax = 0;
	private int _currGeneratedYMin = 0;
	private int _currGeneratedYMax = 0;

	//How far off the currentMin before we start generating adjacent section(s).
	private int _sectionPadding = 256;

	public GameObject playerPrefab;
	[HideInInspector]
	private PlayerController player;

	public CameraController camController;

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
	[HideInInspector]
	public MapLayer[] mapLayers;

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
		sectionMap = new LevelSection[LEVEL_WIDTH, LEVEL_HEIGHT];
		for (int x = 0; x < LEVEL_WIDTH; x ++ )
		{
			for (int y = 0; y < LEVEL_HEIGHT; y ++ )
			{
				sectionMap[x,y] = null;
			}
		}

		mapLayers = GetComponentsInChildren<MapLayer>(); 
		for (int i = 0; i < mapLayers.Length; i++)
		{
			mapLayers[i].init();
			mapLayers[i].index = i;
		}
		
		if (player == null)
		{
			player = Instantiate(playerPrefab, new Vector3((LEVEL_WIDTH * SECTION_WIDTH) * 0.5f, (LEVEL_HEIGHT * SECTION_HEIGHT) * 0.5f, 0), Quaternion.identity).GetComponent<PlayerController>();
		}
		player.levelGen = this;
		camController.target = player;
		camController.sendOrthSizeToTarget();
		camController.gameObject.transform.position = player.transform.position;

	}

	public float getMapLayerValueAtPos( int x, int y, int layerIndex )
	{
		int xSectionIndex = x/SECTION_WIDTH;
		int ySectionIndex = y/SECTION_HEIGHT;
		LevelSection section = sectionMap[xSectionIndex,ySectionIndex];
		return section.getMapLayerValueAtPos( x, y, layerIndex);
	}

	public void RevealMapForPosition ( int x, int y )
	{
		/*
		//TODO: CORNERS - 8 dir
		if ( x - _sectionPadding < _currGeneratedXMin )
		{
			//TODO: Generate a section to the left
			_currGeneratedXMin -= SECTION_WIDTH;
		}
		if ( x + _sectionPadding < _currGeneratedXMax )
		{
			//TODO: Generate a section to the right....
			_currGeneratedXMax += SECTION_WIDTH;
		}
		if ( y - _sectionPadding < _currGeneratedYMin )
		{
			//TODO: Generate a section to the bottom....
			_currGeneratedYMin -= SECTION_HEIGHT;
		}
		if ( y + _sectionPadding < _currGeneratedYMax )
		{
			//TODO: Generate a section to the top....
			_currGeneratedYMax += SECTION_HEIGHT;
		}*/

		//TODO: Figure out which section x and y are in and update it.
		int xSectionIndex = x/SECTION_WIDTH;
		int ySectionIndex = y/SECTION_HEIGHT;
		LevelSection section = sectionMap[xSectionIndex,ySectionIndex];

		//Debug.Log(">===============================================<");
		//Debug.Log(xSectionIndex+", " + ySectionIndex);
		//Debug.Log(((float)x/((float)LEVEL_WIDTH * (float)SECTION_WIDTH))+", " + ((float)y/((float)LEVEL_HEIGHT* (float)SECTION_HEIGHT)));
		//Debug.Log(x+", " + y);
		//Debug.Log((x - (xSectionIndex*SECTION_WIDTH))+", " + (y -(ySectionIndex*SECTION_HEIGHT)));
		//Debug.Log("<===============================================>");
		//Debug.Break();

		if (section == null)
		{
			section = generateSectionAt(x, y, xSectionIndex, ySectionIndex);
		}

		int localX = (x - (xSectionIndex*SECTION_WIDTH));
		int localY = (y -(ySectionIndex*SECTION_HEIGHT));
		section.RevealMapForPosition(localX, localY);
	}

	private LevelSection generateSectionAt( int x, int y, int xIndex, int yIndex )
	{
		LevelSection section = Instantiate(levelSectionPrefab, new Vector3(xIndex * SECTION_WIDTH, yIndex * SECTION_HEIGHT, transform.position.z), Quaternion.identity).GetComponent<LevelSection>();

		sectionMap[xIndex,yIndex] = section;

		if (x < _currGeneratedXMin)
		{
			_currGeneratedXMin = x - (int)(SECTION_WIDTH * 0.5f);
		}
		if (x > _currGeneratedXMax)
		{
			_currGeneratedXMax = x + (int)(SECTION_WIDTH * 0.5f);
		}
		if (y < _currGeneratedYMin)
		{
			_currGeneratedYMin = y - (int)(SECTION_HEIGHT * 0.5f);
		}
		if (y > _currGeneratedYMax)
		{
			_currGeneratedYMax = y + (int)(SECTION_HEIGHT * 0.5f);
		}

		section.init( xIndex, yIndex );

		return section;
	}
}


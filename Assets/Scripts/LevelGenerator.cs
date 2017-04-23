using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour 
{
	public const int LEVEL_WIDTH = 32;
	public const int LEVEL_HEIGHT = 32;
	public const int SECTION_WIDTH = 512;
	public const int SECTION_HEIGHT = 512;

	public static LevelGenerator instance;
	public LevelSection levelSectionPrefab;
	[HideInInspector]
	public LevelSection[,] sectionMap;

	private int _sectionGenDist = 1200;
	
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

	public void checkPosForSectionsGeneration( int x, int y )
	{
		int xSectionIndex = x/SECTION_WIDTH;
		int ySectionIndex = y/SECTION_HEIGHT;

		LevelSection section;

		int indexXCheckDist = 1 + Mathf.CeilToInt(_sectionGenDist/SECTION_WIDTH);
		int indexYCheckDist = 1 + Mathf.CeilToInt(_sectionGenDist/SECTION_HEIGHT);
		float checkDist;
		Vector2 sectionCentre;
		for (int xIndex = xSectionIndex - indexXCheckDist; xIndex < xSectionIndex + 1 + indexXCheckDist; xIndex ++)
		{
			for (int yIndex = ySectionIndex - indexYCheckDist; yIndex < ySectionIndex + 1 + indexYCheckDist; yIndex ++)
			{
				sectionCentre = new Vector2((xIndex * SECTION_WIDTH) + (SECTION_WIDTH * 0.5f), (yIndex*SECTION_HEIGHT) + (SECTION_HEIGHT * 0.5f));
				checkDist = Vector2.Distance(new Vector2(x, y), sectionCentre);

				//Debug.DrawLine(new Vector2(x,y), sectionCentre, new Color(0,0,1, 0.25f), 0.33f );

				if (checkDist < _sectionGenDist)
				{
					//Debug.DrawLine(new Vector2(x,y), sectionCentre,new Color(0,1,0, 0.25f), 0.33f );
					section = generateSectionAt(xIndex, yIndex);
				}
				else
				{
					//Debug.DrawLine(new Vector2(x,y), sectionCentre,new Color(1,0,0, 0.25f), 0.33f );
				}
			}
		}
	}

	public void RevealMapForPosition ( int x, int y )
	{
		// Figure out which section x and y are in and update it.
		int xSectionIndex = x/SECTION_WIDTH;
		int ySectionIndex = y/SECTION_HEIGHT;

		if (xSectionIndex >= LevelGenerator.LEVEL_WIDTH || ySectionIndex >= LevelGenerator.LEVEL_HEIGHT
			|| xSectionIndex < 0 || ySectionIndex < 0 ) 
		{
			return;
		}

		LevelSection section = sectionMap[xSectionIndex,ySectionIndex];
		
		if (section == null)
		{
			return;
		}

		int localX = (x - (xSectionIndex*SECTION_WIDTH));
		int localY = (y - (ySectionIndex*SECTION_HEIGHT));
		section.RevealMapForPosition(localX, localY );
	}

	private LevelSection generateSectionAt( int xIndex, int yIndex )
	{
		if (xIndex >= LevelGenerator.LEVEL_WIDTH || yIndex >= LevelGenerator.LEVEL_HEIGHT
			|| xIndex < 0 || yIndex < 0 ) 
		{
			return null;
		}

		if (sectionMap[xIndex,yIndex] != null) 
			return null;

		//TODO: Maybe we could check for any (empty?) ones far away from the player and use those!!!

		//If we're using a sprite, uncomment.
		//LevelSection section = Instantiate(levelSectionPrefab, new Vector3(xIndex * SECTION_WIDTH, yIndex * SECTION_HEIGHT, transform.position.z), Quaternion.identity).GetComponent<LevelSection>();
		LevelSection section = Instantiate(levelSectionPrefab, new Vector3((xIndex * SECTION_WIDTH) + (SECTION_WIDTH * 0.5f), (yIndex * SECTION_HEIGHT) + (SECTION_HEIGHT * 0.5f), transform.position.z), Quaternion.identity).GetComponent<LevelSection>();

		section.transform.localScale = new Vector3(SECTION_WIDTH,SECTION_HEIGHT,1);

		sectionMap[xIndex,yIndex] = section;

		//TODO: Maybe we can stagger this out a little by making it a coroutine!!!
		section.init( xIndex, yIndex );

		return section;
	}
}


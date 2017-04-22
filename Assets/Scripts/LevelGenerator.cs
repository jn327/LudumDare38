using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour 
{
	private int _levelWidth = 1024;
	private int _levelHeight = 1024;

	public LevelPosition[,] levelPositions;

	public WeatherProperty[] weatherProperties;

	private Texture2D _mapTexture;
	private SpriteRenderer _meshRenderer;
	public GameObject playerPrefab;
	private GameObject player;

	public CameraController camController;
	private bool _mapTextureHasChanged = false;

	private Stack mapRevealQue;

	// Use this for initialization
	void Start () 
	{
		_meshRenderer = GetComponent<SpriteRenderer>();

		_mapTexture = new Texture2D(_levelWidth, _levelHeight);
		//_meshRenderer.material.mainTexture = _mapTexture;

		_meshRenderer.sprite = Sprite.Create (_mapTexture, new Rect(0,0,_levelWidth,_levelHeight), new Vector2(0,0), 1);

		_meshRenderer.material.name = _meshRenderer.name + "_sprite";
		_meshRenderer.material.mainTexture = _mapTexture as Texture;
		_meshRenderer.material.shader = Shader.Find ("Sprites/Default");

		levelPositions = new LevelPosition[_levelWidth,_levelHeight];

		float normalX = 0.5f;
		float normalY = 0.5f;
		if (player == null)
		{
			player = Instantiate(playerPrefab, new Vector3(_levelWidth*normalX, _levelHeight*normalY, 0), Quaternion.identity);
		}
		else
		{
			player.transform.position = new Vector3(_levelWidth*normalX, _levelHeight*normalY, 0);
		}
		player.GetComponent<PlayerController>().levelGen = this;
		camController.target = player.transform;
		camController.gameObject.transform.position = player.transform.position;

		//TODO: Could have this off infinitely!!!?
		generateMap();

		//TODO: Should we go ahead and generate a 2d mesh & collider based on heightmap
		
	}

	public void Update()
	{
		//TODO: Maybe check this less frequently?
		if (_mapTextureHasChanged)
		{
			UpdateMapTexture();
		}
	}

	public void generateMap ()
	{
		LevelPosition levelPos;
		for (int x = 0; x < _levelWidth; x ++ )
		{
			for (int y = 0; y < _levelHeight; y ++ )
			{
				levelPos = levelPositions[x,y] = new LevelPosition();

				//TODO : This is a hard one look at the world height map http://mapsof.net/the-world/elevation
				levelPos.terrainHeight = Random.value;

				//TODO: Aim for something https://www.nasa.gov/sites/default/files/thumbnails/image/15-115.jpg 
				// > in the centre (55%)
				// bit of sine wave variation (15%)
				// a little perlin (5%)
				// height (25%) (particularly harsh below warterline) (colder near the edges hotter in the centre(0.5???)) ( base this on an animation curve ? )
				levelPos.temperature = Random.value;

				//TODO : Similar to the temperature map : http://go.grolier.com/map?id=mtlr080&pid=go
				// temperature map (75%) - perlin (25%)
				levelPos.rainfall = Random.value;

				//showPositionForPlayer(x,y);
			}
		}
	}
	
	public void RevealMapForPosition ( int x, int y )
	{
		if (x >= _levelWidth || y >= _levelHeight
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

		//TODO: ART: DRAW A LINE OR a sea pixel or SOMETHING !!!!
		_mapTexture.SetPixel(x, y, new Color(levelPos.terrainHeight, levelPos.rainfall, levelPos.temperature));
		_mapTextureHasChanged = true;
	}

	public void UpdateMapTexture()
	{
		_mapTexture.Apply();
		_meshRenderer.material.mainTexture = _mapTexture as Texture;
		_mapTextureHasChanged = false;
	}
}

[System.Serializable]
public class LevelPosition
{
	public bool isVisible = false;

	public float terrainHeight = 0;
	public float rainfall = 0;
	public float temperature = 0;
}

[System.Serializable]
public class WeatherProperty
{
	public string name;
	public float value = 0;
	public CurveWeight[] curveValues;
}

[System.Serializable]
public class CurveWeight
{
	public float weighting; //normalized value 0-1;
	public AnimationCurve curve;
}


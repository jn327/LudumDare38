using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectionLayer : MonoBehaviour 
{
	//private SpriteRenderer _spriteRenderer;
	private MeshRenderer _meshRenderer;
	private Texture2D _texture;
	private bool _validateTexture = false;

	public LevelPosition[,] levelPositions;

	private Color _materialStartColor = new Color(1,1,1,0);
	private Color _materialValidateColor = new Color(1,1,1,1);
	private Color _clearColor = new Color(1,1,1,0);

	private int _mapIndex;

	public void init( int mapIndex )
	{
		_texture = new Texture2D(LevelGenerator.SECTION_WIDTH, LevelGenerator.SECTION_HEIGHT, TextureFormat.RGBA32, true);

		//_spriteRenderer = GetComponent<SpriteRenderer>();
		//_spriteRenderer.sprite = Sprite.Create (_texture, new Rect(0,0,LevelGenerator.SECTION_WIDTH,LevelGenerator.SECTION_HEIGHT), new Vector2(0,0), 1);
		//_spriteRenderer.material.name = name + "_sprite";
		//_spriteRenderer.material.mainTexture = _texture as Texture;
		//_spriteRenderer.material.shader = Shader.Find ("Sprites/Default");
		//_spriteRenderer.material.color = _materialStartColor;

		_meshRenderer = GetComponent<MeshRenderer>();
		_meshRenderer.material.mainTexture = _texture as Texture;
		_meshRenderer.material.color = _materialStartColor;


		levelPositions = new LevelPosition[LevelGenerator.SECTION_WIDTH,LevelGenerator.SECTION_HEIGHT];

		_mapIndex = mapIndex;
	}

	public void setLayerValue ( int x, int y, float layerVal )
	{
		if (levelPositions[x,y] == null) 
		{
			levelPositions[x,y] = new LevelPosition();
		}
		levelPositions[x,y].layerValue = layerVal;
	}

	public void UpdateTexture()
	{
		if (_validateTexture == true)
			ValidateTexture();
	}

	public void setTextureForLevelPos( int x, int y )
	{
		Color col = LevelGenerator.instance.mapLayers[_mapIndex].colorGradient.Evaluate(levelPositions[x,y].layerValue);
		_texture.SetPixel(x, y, col);
		_validateTexture = true;
	}

	public void clearTextureForLevelPos ( int x, int y )
	{
		_texture.SetPixel(x, y, _clearColor);
		_validateTexture = true;
	}

	public void ValidateTexture()
	{
		_texture.Apply();

		//_spriteRenderer.material.color = _materialValidateColor;
		//_spriteRenderer.material.mainTexture = _texture as Texture;

		_meshRenderer.material.color = _materialValidateColor;
		_meshRenderer.material.mainTexture = _texture as Texture;

		_validateTexture = false;
	}
}

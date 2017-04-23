using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectionLayer : MonoBehaviour 
{
	private SpriteRenderer _spriteRenderer;
	private Texture2D _texture;
	private bool _validateTexture = false;

	public Gradient colorGradient;

	public void init( Gradient initGradient )
	{
		_texture = new Texture2D(LevelGenerator.SECTION_WIDTH, LevelGenerator.SECTION_HEIGHT, TextureFormat.RGBA32, false);

		_spriteRenderer = GetComponent<SpriteRenderer>();

		_spriteRenderer.sprite = Sprite.Create (_texture, new Rect(0,0,LevelGenerator.SECTION_WIDTH,LevelGenerator.SECTION_HEIGHT), new Vector2(0,0), 1);

		_spriteRenderer.material.name = name + "_sprite";
		_spriteRenderer.material.mainTexture = _texture as Texture;
		_spriteRenderer.material.shader = Shader.Find ("Sprites/Default");
		_spriteRenderer.material.color = new Color(0,0,0, 0);

		colorGradient = initGradient;
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
		_spriteRenderer.material.color = new Color(1,1,1, 1);
		_spriteRenderer.material.mainTexture = _texture as Texture;
		_validateTexture = false;
	}
}

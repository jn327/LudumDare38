using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	private float _xAxisInput = 0;
	private float _yAxisInput = 0;

	private float _currVelocity;

	private float _moveSpeed = 3;
	private float _maxSpeed = 10;

	private Rigidbody2D _rb2d;

	public LevelGenerator levelGen;
	private float _levelUpdateFreq = 0.5f;
	public int visibilityRange = 25;

	void Start () 
	{
		_rb2d = GetComponent<Rigidbody2D>();

		InvokeRepeating( "UpdateLevelVisibility", 0, _levelUpdateFreq);
	}

	void UpdateLevelVisibility()
	{
		int xPos = Mathf.FloorToInt(transform.position.x);
		int yPos = Mathf.FloorToInt(transform.position.y);
		int halfVisRange = Mathf.FloorToInt(visibilityRange*0.5f);
		Vector2 pos;
		for (int x = -halfVisRange; x < halfVisRange; x++)
		{
			for (int y = -halfVisRange; y < halfVisRange; y++)
			{
				pos = new Vector2(xPos+x, yPos+y);
				if (Vector2.Distance(pos, new Vector2(xPos, yPos)) < halfVisRange)
				{
					levelGen.RevealMapForPosition(xPos+x, yPos+y);
				}
			}
		}
	}
	
	void FixedUpdate () 
	{
		UpdateMove();
	}

	void UpdateMove ()
	{
		_xAxisInput = Input.GetAxis("Horizontal");
		_yAxisInput = Input.GetAxis("Vertical");

		//TODO: Boat's do not work like this ... MAKE IT MORE BOATY !!! ...

		_rb2d.AddForce( new Vector2(_xAxisInput, _yAxisInput) * _moveSpeed );

		_currVelocity = _rb2d.velocity.magnitude;
		if (_currVelocity > _maxSpeed)
		{
			float velocityDifNormal = _maxSpeed / _currVelocity;
			_rb2d.velocity = new Vector2( _rb2d.velocity.x * velocityDifNormal, _rb2d.velocity.y * velocityDifNormal);
		}
	}
}

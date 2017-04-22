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

	void Start () 
	{
		_rb2d = GetComponent<Rigidbody2D>();
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

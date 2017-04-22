using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	public Transform target;

	private Vector3 _targetOffset = new Vector3(0,0,-10);
	private float _moveSpeed = 12;

	private float _minOrthSize = 5;
	private float _maxOrthSize = 20;

	private Camera _cam;
	private float _scrollInput;
	private float _camOrthSize;
	private float _camZoomSpeed = 10;

	void Start () 
	{
		_cam = GetComponent<Camera>();
	}
	
	void Update () 
	{
		if (target != null)
		{
			transform.position = Vector3.MoveTowards(transform.position, target.transform.position + _targetOffset, _moveSpeed * Time.deltaTime);
		}

		UpdateCameraZoom();
	}

	void UpdateCameraZoom()
	{
		_scrollInput = Input.GetAxis("Mouse ScrollWheel");
		_camOrthSize = _cam.orthographicSize;
		_camOrthSize += _scrollInput * Time.deltaTime * _camZoomSpeed;

		if (_camOrthSize < _minOrthSize)
		{
			_camOrthSize = _minOrthSize;
		}

		if (_camOrthSize > _maxOrthSize)
		{
			_camOrthSize = _maxOrthSize;
		}

		_cam.orthographicSize = _camOrthSize;
	}
}

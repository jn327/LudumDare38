using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	public PlayerController target;

	private Vector3 _targetOffset = new Vector3(0,0,-10);
	private float _moveSpeed = 150;

	private float _minOrthSize = 100;
	private float _maxOrthSize = 500;

	private Camera _cam;
	private float _scrollInput;
	private float _camOrthSize;
	private float _camZoomSpeed = 500;

	void Awake () 
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

		if (_cam.orthographicSize != _camOrthSize)
		{
			_cam.orthographicSize = _camOrthSize;

			sendOrthSizeToTarget();
		}
	}

	public void sendOrthSizeToTarget()
	{
		if (target != null)
		{
			float orthSizeNormal = ((_cam.orthographicSize-_minOrthSize) / (_maxOrthSize-_minOrthSize));
			target.recieveCameraOrthSizeChange(orthSizeNormal);
		}
	}
}

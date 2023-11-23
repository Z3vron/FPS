using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move_player : MonoBehaviour{

    public bool grounded_flag = false;
    [Header("Player movement statistics")]
    #region

        [Tooltip("Player walking speed")]
        [SerializeField] private float _walking_speed = 3f; 

        [Tooltip("Player running speed")]
        [SerializeField] private float _running_speed = 6f;

        [Tooltip("Player jumping height")]
        [SerializeField] private float _jumping_height = 2f;

        [Tooltip("Player jumping timeout - time that needs to pass after beeing grounded to be able to jump again")]
        [SerializeField] private float _jumping_timeout = 0.3f;

        [Tooltip("Player falling timeout - time that needs to pass after beeing not grounded to start falling down")]
        [SerializeField] private float _falling_timeout = 0.2f;

        [Tooltip("Player rotation speed")]
        [SerializeField] private float _rotation_speed = 1f;

        [Tooltip("Player speed change ratio")]
        [SerializeField] private float _speed_change_ratio = 10f;
    #endregion
    [Header("Ground check varaibles")]
    #region

        [Tooltip("Layer that is required on object for player to be grounded on")]
        [SerializeField] private LayerMask _ground_layer;
        
        [Tooltip("Radious of sphere that checks if player is grounded")]
        [SerializeField] private float _ground_radious = 0.4f;
        
        [Tooltip("Off set in vector y where sphere is playced to check for ground")]
        [SerializeField] private float _ground_offset = -0.15f;
    #endregion
    
    [Tooltip("Grafity force that pulls player down")]
    [SerializeField] private float _gravity_force = -10f; 


    public GameObject cinemachine_camera_target;

    private float _cinemachine_target_pitch;
    
    private CharacterController _controller;
    private Take_input _player_input;
    
    // player
    private float _speed;
    private float _rotation_velocity;
    private float _vertical_velocity;
    private float _terminal_velocity = 53.0f;

    private float _jumping_timeout_delta;
    private float _falling_timeout_delta;
    void Start(){

        Cursor.lockState = CursorLockMode.Locked;

        _player_input = GetComponent<Take_input>();
        _controller = GetComponent<CharacterController>();
    }

    void Update(){
        
        Ground_check();
        Gravity_jump();
        Move_character();
    }
    private void LateUpdate() {
        Move_camera();
    }
    private void Move_character(){
		
			float targetSpeed = _player_input.run_flag ? _running_speed : _walking_speed;

			
			if (_player_input.movement_vector == Vector2.zero)
                targetSpeed = 0.0f;

			
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
						
			// if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset){
			// 	_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speed_change_ratio);
			// 	_speed = Mathf.Round(_speed * 1000f) / 1000f;
			// }
			// else{
			// 	_speed = targetSpeed;
			// }
            _speed = targetSpeed;
			
			Vector3 inputDirection = new Vector3(_player_input.movement_vector.x, 0.0f, _player_input.movement_vector.y).normalized;

			if (_player_input.movement_vector != Vector2.zero)
				inputDirection = transform.right * _player_input.movement_vector.x + transform.forward * _player_input.movement_vector.y;
			
           /*  Debug.Log(inputDirection.normalized);
            Debug.Log(_speed);
            Debug.Log(inputDirection.normalized * _speed); */
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _vertical_velocity, 0.0f) * Time.deltaTime);
		}
    private void Ground_check(){
        Vector3 check_position = new Vector3(transform.position.x, transform.position.y - _ground_offset, transform.position.z);
		grounded_flag = Physics.CheckSphere(check_position, _ground_radious, _ground_layer, QueryTriggerInteraction.Ignore);
    }
    private void Gravity_jump(){
        if (grounded_flag){
      
            _falling_timeout_delta = _falling_timeout;

            if (_vertical_velocity < 0.0f)
                _vertical_velocity = -2f;
            
            if (_player_input.jump_flag && _jumping_timeout_delta <= 0.0f)
                _vertical_velocity = Mathf.Sqrt(_jumping_height * -2f * _gravity_force);
            else
                _player_input.jump_flag = false;

            if (_jumping_timeout_delta >= 0.0f)
                _jumping_timeout_delta -= Time.deltaTime;
		}
		else{

            _jumping_timeout_delta = _jumping_timeout;

            if (_falling_timeout_delta >= 0.0f)
                _falling_timeout_delta -= Time.deltaTime;

            _player_input.jump_flag = false;
		}
		if (_vertical_velocity < _terminal_velocity)
			_vertical_velocity += _gravity_force * Time.deltaTime;

    }
    private void Move_camera(){
        if (_player_input.camera_vector.sqrMagnitude >= 0.01f){

    
            _cinemachine_target_pitch += _player_input.camera_vector.y * _rotation_speed ;
            _rotation_velocity = _player_input.camera_vector.x * _rotation_speed;

            _cinemachine_target_pitch = ClampAngle(_cinemachine_target_pitch, -90f, 90f);

            cinemachine_camera_target.transform.localRotation = Quaternion.Euler(_cinemachine_target_pitch, 0.0f, 0.0f);

            transform.Rotate(Vector3.up * _rotation_velocity);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
    private void OnDrawGizmosSelected(){
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded_flag) 
            Gizmos.color = transparentGreen;
        else 
            Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _ground_offset, transform.position.z), _ground_radious);
    }
}

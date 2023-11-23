using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

public class Take_input : MonoBehaviour{

  [Header("Input values and flags")]
    public bool jump_flag = false;
    public bool run_flag = false;
    public Vector2 movement_vector;
    public Vector2 camera_vector;
  #if ENABLE_INPUT_SYSTEM
    public void OnJump(InputValue value){
      jump_flag = value.isPressed;
    }
    public void OnRun(InputValue value){
      run_flag = value.isPressed;
    }
    public void OnMovement(InputValue value){
      movement_vector = value.Get<Vector2>();
    }
    public void OnCamera(InputValue value){
      camera_vector = value.Get<Vector2>();
    }
  #endif
}

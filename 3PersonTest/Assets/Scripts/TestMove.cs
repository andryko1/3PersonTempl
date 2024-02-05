using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class TestMove : MonoBehaviour
{
  public Transform targetP;
  public Vector3 targetPos;
  public float target_ry;
  public float moveSpeed = 0f;

 

  public bool g = true;  
  public bool f = false;
  public bool j = false;
  public bool m = false;
  public System.DateTime lastUpdate;

  public float r = 0;

  public float SpeedChangeRate = 10.0f;
  public float RotationSmoothTime = 0.12f;
  private CharacterController _controller;
  private float _animationBlend;
  private float _targetRotation = 0.0f;

  private float _rotationVelocity;
  private float _verticalVelocity;

  private Animator _animator;
  private bool _hasAnimator;
  private float _speed;
  private int _animIDSpeed;
  private int _animIDGrounded;
  private int _animIDJump;
  private int _animIDFreeFall;
  private int _animIDMotionSpeed;

  

 
  
  // Start is called before the first frame update
  void Start()
  {
    targetPos.x = transform.position.x;
    targetPos.y = transform.position.y;
    targetPos.z = transform.position.z;

    _hasAnimator = TryGetComponent(out _animator);
   
    //_controller = GetComponent<CharacterController>();
    AssignAnimationIDs();
  }

  // Update is called once per frame
  void Update()
  {
    //Given some means of determining a target point.
    //var targetPoint = FindTargetPoint();
    //However you want to do that.

    //MoveTowardsTarget (targetP.position);
    Move();
    Jump();

  }

  private void Move()
  {
   
   if ((System.DateTime.Now - lastUpdate).TotalMilliseconds >= DataHolder.updateDelay*2 )
    {
      g = true;
      j = false;
      f = false; 
      m = false;           
    }
    var offset = targetPos - transform.position;
    // set target speed based on move speed, sprint speed and if sprint is pressed
    
    float targetSpeed=0;
    if (moveSpeed >=1F && moveSpeed < 5f) {targetSpeed = 2f;}
    else {
      if (moveSpeed >=4F) {targetSpeed = 6f;}
    }

   // Debug.Log("targetSpeed "+targetSpeed.ToString());
    // Debug.Log("offset " + offset.ToString());

    //Debug.Log("offsetM " + offset.magnitude.ToString());



    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is no input, set the target speed to 0


    // a reference to the players current horizontal velocity
   // float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

   // float speedOffset = 0.1f;
    float inputMagnitude = 1f;



    // accelerate or decelerate to target speed
    // if (currentHorizontalSpeed < targetSpeed - speedOffset ||
    //     currentHorizontalSpeed > targetSpeed + speedOffset)
    // {
    //   // creates curved result rather than a linear one giving a more organic speed change
    //   // note T in Lerp is clamped, so we don't need to clamp our speed
    //   _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
    //       Time.deltaTime * SpeedChangeRate);

    //   // round speed to 3 decimal places
    //   _speed = Mathf.Round(_speed * 1000f) / 1000f;
    // }
    // else
    // {
    //   _speed = targetSpeed;
    // }
    
 

    _animationBlend = targetSpeed;//Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
    if (_animationBlend < 0.01f) _animationBlend = 0f;

    // normalise input direction
    Vector3 inputDirection = new Vector3(offset.x, 0.0f, offset.z).normalized;
    //Debug.Log("inputDirection " +offset.x.ToString()+" "+offset.x.ToString()+" " + inputDirection.ToString());   
    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is a move input rotate player when the player is moving

   
    var offset1 = new Vector3(offset.x, 0f, offset.z);
   // Debug.Log("_animationBlend "+  _animationBlend.ToString());
   //  Debug.Log("offset1.magnitude < 0.05f && offset.x<0f && g " + offset1.magnitude.ToString());


    if (offset.magnitude < 0.01f && !m)
    {
     moveSpeed = 0f;
     _animationBlend = 0f;
     inputMagnitude = 1f;
      //j = false;
      if (_hasAnimator)
      {
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
      }
      return;

    }

 _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, target_ry, ref _rotationVelocity,
        RotationSmoothTime);

    // rotate to face input direction relative to camera position
    // if (offset1.magnitude >= 0.1f && m)
    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    
    // if (target_ry>=0) {      
    // //transform.rotation.eulerAngles.Set(0.0f, target_ry, 0.0f);
    // transform.rotation = Quaternion.Euler(0.0f, target_ry, 0.0f);
    // Debug.Log(transform.rotation.y.ToString()+" !!! "+ transform.rotation.eulerAngles.y.ToString());
    // Debug.Log("transform.rotation.eulerAngles.Set "+ target_ry.ToString());
    // }

    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

    // move the player
    // _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
    //                          new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime );

    // update animator if using character
    if (_hasAnimator)
    {
      _animator.SetFloat(_animIDSpeed, _animationBlend);
      _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    // Debug.Log("ms1 " + targetDirection.normalized.ToString() + "  " + _verticalVelocity.ToString());
  }

  private void Jump()
  {
    _animator.SetBool(_animIDGrounded, g);
    _animator.SetBool(_animIDJump, j);
    if (j) {
      Debug.Log("JJJ "+ j.ToString()+ "g  "+ g.ToString()+ "f  "+ f.ToString());
    }
    _animator.SetBool(_animIDFreeFall, f);
  }

  private void AssignAnimationIDs()
  {
    _animIDSpeed = Animator.StringToHash("Speed");
    _animIDGrounded = Animator.StringToHash("Grounded");
    _animIDJump = Animator.StringToHash("Jump");
    _animIDFreeFall = Animator.StringToHash("FreeFall");
    _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
  }
  void MoveTowardsTarget(Vector3 target)
  {
    var cc = GetComponent<CharacterController>();
    var offset = target - transform.position;

    //Get the difference.
    if (offset.magnitude > .1f)
    {
      //If we're further away than .1 unit, move towards the target.
      //The minimum allowable tolerance varies with the speed of the object and the framerate. 
      // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
      offset = offset.normalized * moveSpeed;
      //normalize it and account for movement speed.
      cc.Move(offset * Time.deltaTime);
      offset.y = 0;
      transform.rotation = Quaternion.LookRotation(offset);
      //actually move the character.
    }
  }

  private void OnFootstep(AnimationEvent animationEvent)
  {
    return;
    // if (animationEvent.animatorClipInfo.weight > 0.5f)
    // {
    //     if (FootstepAudioClips.Length > 0)
    //     {
    //         var index = Random.Range(0, FootstepAudioClips.Length);
    //         AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
    //     }
    // }
  }

   private void OnLand(AnimationEvent animationEvent)
    {
      return;
      // if (animationEvent.animatorClipInfo.weight > 0.5f)
      // {
      //   AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
      // }
    }
}

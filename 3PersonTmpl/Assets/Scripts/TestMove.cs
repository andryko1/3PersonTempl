using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestMove : MonoBehaviour
{
  public Transform targetP;
  public Vector3 targetPos;
  public float moveSpeed = 5;

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
    _controller = GetComponent<CharacterController>();
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

  }

  private void Move()
  {
    var offset = targetPos - transform.position;
    // set target speed based on move speed, sprint speed and if sprint is pressed
    float targetSpeed = moveSpeed;
    // Debug.Log("offset " + offset.ToString());

    //Debug.Log("offsetM " + offset.magnitude.ToString());



    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is no input, set the target speed to 0


    // a reference to the players current horizontal velocity
    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

    float speedOffset = 0.1f;
    float inputMagnitude = 1f;



    // accelerate or decelerate to target speed
    if (currentHorizontalSpeed < targetSpeed - speedOffset ||
        currentHorizontalSpeed > targetSpeed + speedOffset)
    {
      // creates curved result rather than a linear one giving a more organic speed change
      // note T in Lerp is clamped, so we don't need to clamp our speed
      _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
          Time.deltaTime * SpeedChangeRate);

      // round speed to 3 decimal places
      _speed = Mathf.Round(_speed * 1000f) / 1000f;
    }
    else
    {
      _speed = targetSpeed;
    }

    _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
    if (_animationBlend < 0.01f) _animationBlend = 0f;

    // normalise input direction
    Vector3 inputDirection = new Vector3(offset.x, 0.0f, offset.z).normalized;
    //Debug.Log("inputDirection " +offset.x.ToString()+" "+offset.x.ToString()+" " + inputDirection.ToString());   
    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
    // if there is a move input rotate player when the player is moving

   
    var offset1 = new Vector3(offset.x, 0f, offset.z);
    if (offset1.magnitude < 0.1f)
    {

      _animationBlend = 0f;
      inputMagnitude = 1f;

      if (_hasAnimator)
      {
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
      }
      return;

    }

 _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
        RotationSmoothTime);

    // rotate to face input direction relative to camera position
    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

    // move the player
    _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    // update animator if using character
    if (_hasAnimator)
    {
      _animator.SetFloat(_animIDSpeed, _animationBlend);
      _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    // Debug.Log("ms1 " + targetDirection.normalized.ToString() + "  " + _verticalVelocity.ToString());
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
}

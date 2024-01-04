using Cinemachine;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = false;

		public CinemachineVirtualCamera cameraFollow;
		public GameObject geometry;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{			
			MoveInput(value.Get<Vector2>());
		}

		public void OnZoom(InputValue value)
		{			
			//MoveInput(value.Get<Vector2>());
			//Debug.Log("1");
			//Debug.Log(value.Get<float>());
			ZoomInput(value.Get<float>());
		}

		public void OnLook(InputValue value)
		{
			// Debug.Log("777");
			if(cursorInputForLook 
			//&& Mouse.current.rightButton.isPressed
			)
			{
				//Debug.Log(Mouse.current.rightButton.isPressed.ToString());
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnLock(InputValue value)
		{
			cursorInputForLook = true;
			Debug.Log("Lock "+Mouse.current.rightButton.isPressed.ToString());
			Cursor.lockState = CursorLockMode.Locked ;
		}
		public void OnUnLock(InputValue value)
		{ 
			cursorInputForLook = false;
			Debug.Log("UnLock "+ Mouse.current.rightButton.isPressed.ToString());
			Cursor.lockState =  CursorLockMode.None;
			look = Vector2.zero;
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
			
		} 

		public void ZoomInput(float scroll)
		{
			
			CinemachineComponentBase componentBase = cameraFollow.GetCinemachineComponent(CinemachineCore.Stage.Body);
			if (!(componentBase is Cinemachine3rdPersonFollow))
			{
				return;	
    			
			}
				// Debug.Log("!!! "+scroll.ToString());
				// Debug.Log((componentBase as Cinemachine3rdPersonFollow).CameraDistance);

			if (scroll >0) {
				(componentBase as Cinemachine3rdPersonFollow).CameraDistance = (componentBase as Cinemachine3rdPersonFollow).CameraDistance + 1 ; // your value
			
			} else if (scroll < 0) {
				if ((componentBase as Cinemachine3rdPersonFollow).CameraDistance <= 0) {
					return;
				}

				(componentBase as Cinemachine3rdPersonFollow).CameraDistance = (componentBase as Cinemachine3rdPersonFollow).CameraDistance - 1 ; // your value
			}
			if ((componentBase as Cinemachine3rdPersonFollow).CameraDistance <= 0) {
					geometry.SetActive(false);
				} else {geometry.SetActive(true);
				}

		} 

		public void LookInput(Vector2 newLookDirection)
		{
			// Debug.Log("555");
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		// private void OnApplicationFocus(bool hasFocus)
		// {
		// 	SetCursorState(cursorLocked 
		// 	//&& Mouse.current.rightButton.isPressed
		// 	);
		// }

		// private void SetCursorState(bool newState)
		// {
		// 	Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		// }
	}
	
}
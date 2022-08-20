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
		public bool walk;
		public bool rightClick;
		public bool clickTarget;
		public float scroll;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnWalk(InputValue value)
		{
			WalkInput(value.isPressed);
		}

		public void OnRightClick(InputValue value)
		{
			RightClickInput(value.isPressed);
		}

		public void OnClickTarget(InputValue value)
		{
			ClickTargetInput(value.isPressed);
		}

		public void OnScroll(InputValue value)
		{
			ScrollInput(value.Get<float>());
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void WalkInput(bool newWalkState)
		{
			walk = newWalkState;
		}

		public void RightClickInput(bool newRightClickState)
		{
			rightClick = newRightClickState;
		}

		public void ClickTargetInput(bool newClickTargetState)
		{
			clickTarget = newClickTargetState;
		}

		public void ScrollInput(float scrollValue)
		{
			scroll = scrollValue;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}
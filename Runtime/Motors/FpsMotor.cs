// Creator: Kieran
// Creation Time: 2022/06/06 11:01
using ModularCharacterController.Cameras;

using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularCharacterController.Motors
{
	public class FpsMotor : ModularBehaviour
	{
		public bool IsGrounded { get; private set; }
		
		public Rigidbody Rigidbody { get; private set; }

		[SerializeField] private MovementSettings settings;
		
		private new CapsuleCollider collider;
		private Transform player;
		private new FpsCamera camera;

		private const float SPEED_ON_GROUND_MODIFIER = 1;
		private const float SPEED_IN_AIR_MODIFIER = 1;
		private const float IN_AIR_GROUND_CHECK_DELAY = 0.2f;

		private float lastTimeInAir;
		private bool isJumpPressed;
		
		public override void Init(IMCCPlayer _playerInterface)
		{
			Rigidbody = _playerInterface.Rigidbody;
			collider = (CapsuleCollider) _playerInterface.Collider;
			player = _playerInterface.Transform;
			if(_playerInterface.TryGetBehaviour(out FpsCamera cam))
				camera = cam;

			if(_playerInterface.TryGetBehaviour(out MultiCamera multiCamera))
			{
				if(multiCamera.TryGetBehaviour(out cam))
					camera = cam;
			}

            settings.MoveAction.Enable();
            settings.JumpAction.Enable();
			settings.JumpAction.performed += OnJumpPerformed;
			settings.JumpAction.canceled += OnJumpCanceled;
		}

		protected override void OnProcess(UpdatePhase _phase)
		{
			CheckGrounded();
			HandleMovement(settings.MoveAction.ReadValue<Vector2>());
			ApplyExtraGravity();
		}

		/// <summary>
		/// Detect if the playermotor is actually grounded; ie; not jumping or over nothing
		/// </summary>
		private void CheckGrounded()
		{
			// Use a smaller ground distance check if in air, to prevent suddenly snapping to ground
			float chosenGroundCheckDistance = settings.GetGroundDistanceCheck(IsGrounded);

			// Check if the current time is greater than the required jump check time
			if(Time.time >= lastTimeInAir + IN_AIR_GROUND_CHECK_DELAY)
			{
				// Get all layers below us in the correct distance
				RaycastHit[] hits = CapsuleCastAllInDirection(-player.up, chosenGroundCheckDistance);

				// If there is actually anything in the array, we can loop through it, otherwise, we aren't grounded
				if(hits.Length > 0)
				{
					foreach(RaycastHit hit in hits)
					{
						// Check if we are touching the ground, if so, set grounded to true and return
						if(hit.transform.CompareTag(settings.GroundTag) || hit.transform.gameObject.layer == 0)
						{
							IsGrounded = true;
							return;
						}
					}
				}
				else
				{
					// We are standing above nothing so we aren't grounded
					IsGrounded = false;
				}
			}
		}

		/// <summary>
		/// Handles the movement of the player by the axis of the moveAction
		/// </summary>
		/// <param name="_axis">The axis the controller or keyboard is requesting</param>
		private void HandleMovement(Vector2 _axis)
		{
			// If the camera motor isn't running right now we shouldn't be able to control the player
			if(!camera.Enabled)
				return;

			// Calculate the max speed and the speed modifier by the grounded state
			float maxSpeed = settings.GetMaxSpeed(IsGrounded);
			float modifier = IsGrounded ? SPEED_ON_GROUND_MODIFIER : SPEED_IN_AIR_MODIFIER;

			// Calculate the correct velocity by the axis of the input
			Vector3 forward = player.forward * _axis.y;
			Vector3 right = player.right * _axis.x;
			Vector3 desiredVelocity = (forward + right) * (maxSpeed * modifier) - Rigidbody.velocity;

			// Check we can move this way, if we can apply the velocity
			if(CanMoveInDirection(desiredVelocity))
				Rigidbody.AddForce(new Vector3(desiredVelocity.x, 0, desiredVelocity.z), ForceMode.Impulse);
		}

		/// <summary>
		/// Attempt to apply extra gravity depending on the state of the rigidbody and jump button
		/// </summary>
		private void ApplyExtraGravity()
		{
			// Check if we are falling, if we are apply normal snappy fall
			if(Rigidbody.velocity.y < 0)
			{
				Rigidbody.velocity += Vector3.up * (Physics.gravity.y * settings.FallMultiplier * Time.deltaTime);
			}
			// We are rising, but we aren't pressing the jump button, so fall faster
			else if(Rigidbody.velocity.y > 0 && !isJumpPressed)
			{
				Rigidbody.velocity += Vector3.up * (Physics.gravity.y * settings.LowJumpMultiplier * Time.deltaTime);
			}
		}

		private bool CanMoveInDirection(Vector3 _targetDir)
		{
			// Find everything in the direction we are attempting to move at least half a meter away
			RaycastHit[] hits = CapsuleCastAllInDirection(_targetDir, 0.5f);

			foreach(RaycastHit hit in hits)
			{
				if(hit.collider.CompareTag(settings.WallTag) || hit.transform.gameObject.layer == 0)
				{
					// We will walk into a wall so don't move that way
					return false;
				}
			}

			// We can move this way since we won't walk into a wall
			return true;
		}

		/// <summary>
		/// Cast a Capsule in the passed direction and get all hit objects in that direction.
		/// </summary>
		/// <param name="_direction">The direction to cast the capsule</param>
		/// <param name="_distance">How far the capsule will travel</param>
		private RaycastHit[] CapsuleCastAllInDirection(Vector3 _direction, float _distance)
		{
			Vector3 top = transform.position + collider.center + Vector3.up * ((collider.height * 0.5f) - collider.radius);
			Vector3 bot = transform.position + collider.center - Vector3.up * ((collider.height * 0.5f) - collider.radius);

			// Cast the capsule in the passed direction and distance
			return Physics.CapsuleCastAll(top, bot, collider.radius * 0.95f, _direction, _distance, settings.LayerChecks);
		}

		/// <summary>
		/// Fired when the jump action is pressed.
		/// </summary>
		private void OnJumpPerformed(InputAction.CallbackContext _context)
		{
			// If the camera is not running right now, we won't jump
			if(!camera.Enabled)
				return;

			isJumpPressed = true;

			// If we are grounded jump and store the jump time
			if(IsGrounded)
			{
				Rigidbody.AddForce(Vector3.up * settings.JumpForce, ForceMode.Impulse);
				IsGrounded = false;

				lastTimeInAir = Time.time;
			}
		}
		
		/// <summary>
		/// Fired when the jump action either a) fails or b) is released
		/// </summary>
		private void OnJumpCanceled(InputAction.CallbackContext _context) => isJumpPressed = false;
	}
}
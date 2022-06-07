// Creator: Kieran
// Creation Time: 2022/06/06 12:22
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularCharacterController.Motors
{
	#pragma warning disable CS0649
	[Serializable]
	[CreateAssetMenu(fileName = "Movement Settings", menuName = "MCC/Movement Settings")]
	public class MovementSettings : ScriptableObject
	{
		/// <summary>
		/// The amount of force used to make the player jump
		/// </summary>
		public float JumpForce => jumpForce;

		/// <summary>
		/// The amount of gravity to apply when the player is falling normally
		/// </summary>
		public float FallMultiplier => fallMultiplier - 1;
		/// <summary>
		/// The amount of gravity to apply when the player is rising and not pressing the jump button
		/// </summary>
		public float LowJumpMultiplier => lowJumpMultiplier - 1;

		/// <summary>
		/// The layers to check against when raycasting.
		/// </summary>
		public LayerMask LayerChecks => layerChecks;
		public string GroundTag => groundTag;
		public string WallTag => wallTag;

		/// <summary>
		/// Gets the maximum speed of the player depending if they are in the air or not.
		/// </summary>
		/// <param name="_isGrounded">Whether or not the player is in the air.</param>
		public float GetMaxSpeed(bool _isGrounded = false) => individualMaxSpeeds ? _isGrounded ? maxSpeedOnGround : maxSpeedInAir : maxSpeed;

		/// <summary>
		/// Gets the distance of the ray depending if they are in the air or not.
		/// </summary>
		/// <param name="_isGrounded">Whether or not the player is in the air.</param>
		public float GetGroundDistanceCheck(bool _isGrounded = false) => _isGrounded ? groundDistanceCheck : groundDistanceInAirCheck;

		public InputAction MoveAction => moveAction;
		public InputAction JumpAction => jumpAction;
		
		[SerializeField, Range(1, 20)] private float jumpForce;

		[SerializeField] private bool individualMaxSpeeds = true;
		[SerializeField, Range(1, 20)] private float maxSpeed;
		[SerializeField, Range(1, 20)] private float maxSpeedOnGround;
		[SerializeField, Range(1, 20)] private float maxSpeedInAir;

		[SerializeField, Min(0.1f)] private float groundDistanceCheck = 1;
		[SerializeField, Min(0.2f)] private float groundDistanceInAirCheck = 0.2f;

		[SerializeField, Min(0.1f)] private float fallMultiplier = 2.5f;
		[SerializeField, Min(0.2f)] private float lowJumpMultiplier = 2f;

		[SerializeField] private LayerMask layerChecks;
		[SerializeField] private string groundTag;
		[SerializeField] private string wallTag;

		[SerializeField] private InputActionReference moveAction;
		[SerializeField] private InputActionReference jumpAction;
	}
}
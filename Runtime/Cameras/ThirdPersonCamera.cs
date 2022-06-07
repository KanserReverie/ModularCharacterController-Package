// Creator: Kieran
// Creation Time: 2022/06/06 12:44
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularCharacterController.Cameras
{
	public class ThirdPersonCamera : ModularBehaviour
	{
		private const string CONTROLLER_SCHEME_NAME = "Gamepad";
		
		private Transform CamTransform => camera.transform;
		
		[SerializeField] private CameraSettings settings;

		[Header("Third Person Camera Settings")] 
		[SerializeField] private Transform boom;
		[SerializeField, Range(.05f, 1f)] private float collisionRadius = .3f;

		[SerializeField, Min(.1f)] private float cameraDistance = 4f;
		[SerializeField, Range(.05f, .2f)] private float damping = .1f;
		
		private new Camera camera;
		private PlayerInput input;
		private Transform player;

		private Vector2 rotation = Vector2.zero;
		private Vector3 cameraVelocity = Vector3.zero;
		
		public override void Init(IMCCPlayer _playerInterface)
		{
			player = _playerInterface.Transform;

			camera = boom.GetComponentInChildren<Camera>();
			CamTransform.localPosition = new Vector3(0, 0, -cameraDistance);

			input = _playerInterface.Input;
			input.camera = camera;
		}

		protected override void OnProcess(UpdatePhase _phase)
		{
			UpdateRotation();
			UpdatePosition();
		}

		private void UpdateRotation()
		{
			Vector2 lookVector = settings.Look.ReadValue<Vector2>();

			rotation.x += lookVector.x * settings.GetSensitivity(input.currentControlScheme == CONTROLLER_SCHEME_NAME);
			rotation.y += lookVector.y * settings.GetSensitivity(input.currentControlScheme == CONTROLLER_SCHEME_NAME);
			rotation.y = Mathf.Clamp(rotation.y, -settings.VerticalLookBounds, settings.VerticalLookBounds);
			
			ApplyRotation();
		}

		private void ApplyRotation()
		{
			player.localRotation = Quaternion.identity;
			
			boom.localRotation = Quaternion.AngleAxis(rotation.y, Vector3.left);
			transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
		}

		private void UpdatePosition()
		{
			Vector3 newPos = Vector3.SmoothDamp(CamTransform.position, CalculatePosition(), ref cameraVelocity, damping);
			CamTransform.position = newPos;
		}

		private Vector3 CalculatePosition()
		{
			Vector3 newPos = boom.position - boom.forward * cameraDistance;

			Vector3 direction = -CamTransform.forward;

			if(Physics.Raycast(boom.position, direction, out RaycastHit hit, cameraDistance))
				newPos = hit.point + (hit.normal * collisionRadius);

			return newPos;
		}

		protected override void OnEnabledStateChanged(bool _newState)
		{
			if(camera == null)
				camera = boom.GetComponentInChildren<Camera>();

			camera.GetComponentInChildren<AudioListener>().enabled = _newState;
			camera.enabled = _newState;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * -cameraDistance);
			
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position + transform.forward * -cameraDistance, collisionRadius);
		}
	}
}
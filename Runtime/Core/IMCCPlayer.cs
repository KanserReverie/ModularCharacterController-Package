// Creator: Kieran
// Creation Time: 2022/06/06 11:01

using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularCharacterController
{
	// ReSharper disable once InconsistentNaming
	public interface IMCCPlayer
	{
		public PlayerInput Input { get; }
		public Collider Collider { get; }
		public Rigidbody Rigidbody { get; set; }
		public Transform Transform { get; }

		public bool TryGetBehaviour<BEHAVIOUR>(out BEHAVIOUR _found) where BEHAVIOUR : ModularBehaviour;
	}
}
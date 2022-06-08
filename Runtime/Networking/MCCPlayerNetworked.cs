// Creator: Kieran
// Creation Time: 2022/06/06 11:07
#if MIRROR
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularCharacterController.Networking
{
	public class MCCPlayerNetworked : NetworkBehaviour, IMCCPlayer
	{
		public PlayerInput Input => input;

		public Collider Collider => collider;
		public Rigidbody Rigidbody { get; set; }

		public Transform Transform => transform;
		
		[SerializeField] private new Collider collider;
		[SerializeField] private PlayerInput input;
		[SerializeField] private List<ModularBehaviour> behaviours = new List<ModularBehaviour>();

		public bool TryGetBehaviour<BEHAVIOUR>(out BEHAVIOUR _found) where BEHAVIOUR : ModularBehaviour
		{
			foreach(ModularBehaviour behaviour in behaviours)
			{
				if(behaviour.GetType() == typeof(BEHAVIOUR))
				{
					_found = (BEHAVIOUR)behaviour;
					return true;
				}
			}

			_found = null;
			return false;
		}

		public void RegisterBehaviour(ModularBehaviour _behaviour)
		{
			if(!behaviours.Contains(_behaviour))
				behaviours.Add(_behaviour);
		}

		public void DeregisterBehaviour(ModularBehaviour _behaviour)
		{
			if(behaviours.Contains(_behaviour))
				behaviours.Remove(_behaviour);
		}

		private void Start()
		{
			Rigidbody = gameObject.GetComponent<Rigidbody>();
			Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			
			if(collider == null)
				collider = gameObject.GetComponent<Collider>();

			if(collider == null)
			{
				collider = gameObject.AddComponent<BoxCollider>();
				Debug.LogWarning("No collider attached to MCC Player, adding standard box collider... Is this intended?", gameObject);
			}

			input.gameObject.SetActive(isLocalPlayer);
			
			behaviours.ForEach(_behaviour =>
			{
				if(_behaviour.LocalOnly && isLocalPlayer)
				{
					_behaviour.Init(this);
				}
				else
				{
					_behaviour.Enabled = false;
				}
			});
		}

		private void Update() => behaviours.ForEach(_behaviour => _behaviour.Process(UpdatePhase.Update));

		private void FixedUpdate() => behaviours.ForEach(_behaviour => _behaviour.Process(UpdatePhase.FixedUpdate));

		private void LateUpdate() => behaviours.ForEach(_behaviour => _behaviour.Process(UpdatePhase.LateUpdate));
		
		/// <summary>
		/// Called when component is attached to object.
		/// </summary>
		private void Reset()
		{
			if(collider == null)
			{
				collider = gameObject.GetComponent<Collider>();
				
				if(collider != null) 
					Debug.Log($"Added attached {collider}",this);
			}

			if(behaviours.Count == 0)
			{
				ModularBehaviour[] _behavioursInChildren = GetComponentsInChildren<ModularBehaviour>();

				foreach(ModularBehaviour modularBehaviourBehaviour in _behavioursInChildren)
				{
					behaviours.Add(modularBehaviourBehaviour);
					Debug.Log($"Added attached {modularBehaviourBehaviour} to {behaviours}", this);
				}
			}

			if(input == null)
			{
				input = GetComponentInChildren<PlayerInput>();
				if(input != null)
					Debug.Log($"Added attached {input}",this);
			}
		}
	}
}
#endif
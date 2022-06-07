// Creator: Kieran & James
// Creation Time: 2022/04/11 09:01
using UnityEngine;

namespace ModularCharacterController
{
	/// <summary>
	/// These are an abstract class that is used as a base of all all components.
	/// E.g. Camera and Motion.
	/// </summary>
	public abstract class ModularBehaviour : MonoBehaviour
	{
		public bool Enabled
		{
			get => intEnabled;
			set
			{
				intEnabled = value;
				OnEnabledStateChanged(intEnabled);
			}
		}

		public bool LocalOnly => localOnly;

		public UpdatePhase UpdatePhase => updatePhase;
		
		[SerializeField] private bool localOnly = true;
		[SerializeField] private UpdatePhase updatePhase = UpdatePhase.Update;
		private bool intEnabled = true;

		public virtual void Init(IMCCPlayer _playerInterface) { }

		/// <summary>
		/// Called in all update functions on the MCCPlayer, with a reference to the phase being called from,
		/// so that the ModularBehaviour can be updated in specific loops.
		/// </summary>
		/// <param name="_phase"> The update loop that this process function is being called from. </param>
		public void Process(UpdatePhase _phase)
		{
			if((_phase != updatePhase && _phase != UpdatePhase.Any) || !Enabled)
				return;
			
			OnProcess(_phase);
		}

		protected abstract void OnProcess(UpdatePhase _phase);

		protected virtual void OnEnabledStateChanged(bool _newState) { }
	}
}
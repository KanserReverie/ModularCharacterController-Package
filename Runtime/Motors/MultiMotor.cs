// Creator: Kieran
// Creation Time: 2022/06/06 12:46
using System.Collections.Generic;
using UnityEngine;

namespace ModularCharacterController.Cameras
{
	public class MultiMotor : ModularBehaviour
	{
		[SerializeField] private List<ModularBehaviour> subMotors = new List<ModularBehaviour>();

		private int currentMotorIndex;

		public bool TryGetBehaviour<BEHAVIOUR>(out BEHAVIOUR _found) where BEHAVIOUR : ModularBehaviour
		{
			foreach(ModularBehaviour behaviour in subMotors)
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

		public void ActivateMotor(int _camIndex)
		{
			subMotors[currentMotorIndex].Enabled = false;
			
			currentMotorIndex = _camIndex;
			
			subMotors[currentMotorIndex].Enabled = true;
		}

		public void NextMotor()
		{
			subMotors[currentMotorIndex].Enabled = false;
			
			currentMotorIndex++;
			if(currentMotorIndex >= subMotors.Count)
				currentMotorIndex = 0;
			
			subMotors[currentMotorIndex].Enabled = true;
		}
		
		public void PreviousMotor()
		{
			subMotors[currentMotorIndex].Enabled = false;
			
			currentMotorIndex--;
			if(currentMotorIndex < 0)
				currentMotorIndex = subMotors.Count - 1;
			
			subMotors[currentMotorIndex].Enabled = true;
		}

		public override void Init(IMCCPlayer _playerInterface)
		{
			foreach(ModularBehaviour subMotor in subMotors)
			{
				subMotor.Init(_playerInterface);
				subMotor.Enabled = false;
			}
			
			subMotors[currentMotorIndex].Enabled = true;
		}

		protected override void OnProcess(UpdatePhase _phase)
		{
			UpdatePhase currentMotorPhase = subMotors[currentMotorIndex].UpdatePhase;
			
			if((_phase != currentMotorPhase && _phase != UpdatePhase.Any) || !Enabled)
				return;
			
			subMotors[currentMotorIndex].Process(_phase);
		}
	}
}
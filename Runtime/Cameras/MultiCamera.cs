// Creator: Kieran
// Creation Time: 2022/06/06 12:38
using System.Collections.Generic;
using UnityEngine;

namespace ModularCharacterController.Cameras
{
	public class MultiCamera : ModularBehaviour
	{
		[SerializeField] private List<ModularBehaviour> subCameras = new List<ModularBehaviour>();

		private int currentCameraIndex;

		public bool TryGetBehaviour<BEHAVIOUR>(out BEHAVIOUR _found) where BEHAVIOUR : ModularBehaviour
		{
			foreach(ModularBehaviour behaviour in subCameras)
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

		public void ActivateCamera(int _camIndex)
		{
			subCameras[currentCameraIndex].Enabled = false;
			
			currentCameraIndex = _camIndex;
			
			subCameras[currentCameraIndex].Enabled = true;
		}

		public void NextCamera()
		{
			subCameras[currentCameraIndex].Enabled = false;
			
			currentCameraIndex++;
			if(currentCameraIndex >= subCameras.Count)
				currentCameraIndex = 0;
			
			subCameras[currentCameraIndex].Enabled = true;
		}
		
		public void PreviousCamera()
		{
			subCameras[currentCameraIndex].Enabled = false;
			
			currentCameraIndex--;
			if(currentCameraIndex < 0)
				currentCameraIndex = subCameras.Count - 1;
			
			subCameras[currentCameraIndex].Enabled = true;
		}

		public override void Init(IMCCPlayer _playerInterface)
		{
			foreach(ModularBehaviour subCamera in subCameras)
			{
				subCamera.Init(_playerInterface);
				subCamera.Enabled = false;
			}
			
			subCameras[currentCameraIndex].Enabled = true;
		}

		protected override void OnProcess(UpdatePhase _phase)
		{
			UpdatePhase currentCamPhase = subCameras[currentCameraIndex].UpdatePhase;
			
			if((_phase != currentCamPhase && _phase != UpdatePhase.Any) || !Enabled)
				return;
			
			subCameras[currentCameraIndex].Process(_phase);
		}
	}
}
using Cinemachine;
//using FMODUnity;
using UnityEngine;

namespace Koakuma.Game.GameCamera
{
    public class CameraSetting : MonoBehaviour
    {
        public CinemachineBrain brain;
        public CinemachineVirtualCamera mainCamera;
        public GameObject mask;
        //public StudioListener listener;
        public CinemachineImpulseListener impulseListener;

        private void Reset()
        {
            brain = GetComponentInChildren<CinemachineBrain>();
            mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            //listener = GetComponentInChildren<StudioListener>();
            impulseListener = GetComponentInChildren<CinemachineImpulseListener>();
        }
    }
}

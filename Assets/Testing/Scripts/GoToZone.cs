using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuideCube;

namespace Testing
{
    public class GoToZone : MonoBehaviour
    {
        [SerializeField] private GCubeController controller;
        [SerializeField] private Transform waitingPoint;

        private GCubeState lastState;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Target>() != null)
            {
                lastState = controller.CurrentState;

                controller.SetState(
                    new GCSGoTo(waitingPoint.position),
                    new GCSIdle()
                    );
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Target>() != null)
            {
                controller.SetState(lastState);
            }
        }
    }
}

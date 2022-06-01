using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuideCube;

namespace Testing
{
    public class StateController : MonoBehaviour
    {
        [SerializeField] private GCubeController gCController;
        [SerializeField] private Transform objectToFollow;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            gCController.SetState(new GCSFollowObject(objectToFollow, 0));
        }
    }
}
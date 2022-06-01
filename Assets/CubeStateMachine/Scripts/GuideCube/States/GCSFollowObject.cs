using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuideCube
{
    public class GCSFollowObject : GCubeState
    {
        private readonly Transform objectToFollow;
        private readonly float keepingDistance;

        private const float updateDelay = 0.2f;
        private const float minMove = 0.1f;

        private Vector3 lastObjectPosition;
        private IEnumerator chcker;

        public GCSFollowObject(Transform objectToFollow, float keepingDistance)
        {
            this.objectToFollow = objectToFollow;
            this.keepingDistance = keepingDistance;
        }

        protected override void Start()
        {
            UpdateTarget();

            chcker = CheckObject();
            controller.StartCoroutine(chcker);
        }

        public override void End()
        {
            controller.StopCoroutine(chcker);
        }

        private IEnumerator CheckObject()
        {
            while (true)
            {
                if (Vector3.Distance(objectToFollow.position, lastObjectPosition) > minMove)
                    UpdateTarget();

                yield return new WaitForSeconds(updateDelay);
            }
        }

        private void UpdateTarget()
        {
            lastObjectPosition = objectToFollow.position;

            controller.GoToTarget(objectToFollow.position, keepingDistance);
        }
    }
}
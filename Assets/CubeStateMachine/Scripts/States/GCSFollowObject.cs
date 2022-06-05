using System.Collections;
using UnityEngine;

namespace GuideCube
{
    public class GCSFollowObject : GCubeState
    {
        private readonly Transform targetTransform;
        private readonly float distanceToKeep;

        private const float updateDelay = 0.2f;
        private const float positionUpdateThreshold = 0.1f;

        private Vector3 targetLastPosition;
        private Coroutine activeCoroutine;

        public GCSFollowObject(Transform targetTransform, float distanceToKeep)
        {
            this.targetTransform = targetTransform;
            this.distanceToKeep = distanceToKeep;
        }

        protected override void Start()
        {
            UpdateTargetPosition();
            activeCoroutine = controller.StartCoroutine(UpdateLoop());
        }

        public override void End()
        {
            controller.StopCoroutine(activeCoroutine);
        }

        private IEnumerator UpdateLoop()
        {
            while (true)
            {
                if (Vector3.Distance(targetTransform.position, targetLastPosition) > positionUpdateThreshold)
                {
                    UpdateTargetPosition();
                }
                yield return new WaitForSeconds(updateDelay);
            }
        }

        private void UpdateTargetPosition()
        {
            targetLastPosition = targetTransform.position;
            controller.GoToTarget(targetLastPosition, distanceToKeep);
        }
    }
}

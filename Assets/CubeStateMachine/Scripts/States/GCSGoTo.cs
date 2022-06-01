using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuideCube
{
    public class GCSGoTo : GCubeState
    {
        private readonly Vector3 targetPosition;
        private readonly float avaDistToTarget;

        public GCSGoTo(Vector3 targetPosition, float avaDistToTarget = 0)
        {
            this.targetPosition = targetPosition;
            this.avaDistToTarget = avaDistToTarget;
        }

        protected override void Start()
        {
            controller.GoToTarget(targetPosition, avaDistToTarget);
        }

        public override void Update()
        {
            if (!controller.IsOnTheWay)
            {
                controller.EndCurrentState();
            }
        }
    }
}
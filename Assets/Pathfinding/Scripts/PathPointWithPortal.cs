using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Pathfinding
{
    public class PathPointWithPortal : PathPoint
    {
        [SerializeField, Required] private PathPointWithPortal connectedPortalPoint;
        [SerializeField] private Transform portalOffset;

        public PathPointWithPortal ConnectedPortalPoint => connectedPortalPoint;
        public Transform PortalOffset => portalOffset;
        public void SetConnectedPortalPoint(PathPointWithPortal point) => connectedPortalPoint = point;


        /// <summary>
        /// Add other path points in range and connectedPortalPoint to connectedPoints list
        /// </summary>
        /// <param name="allPoints">All path points on the map</param>
        public override void FindConnectedPoints(PathPoint[] allPoints)
        {
            base.FindConnectedPoints(allPoints);

            if (connectedPortalPoint != null && !connectedPoints.Contains(connectedPortalPoint))
                connectedPoints.Add(connectedPortalPoint);
        }


        /// <summary>
        /// Calculate the distance to the second point taking into account the portal
        /// </summary>
        public override float Distance(PathPoint secondPoint)
        {
            if (secondPoint == connectedPortalPoint)
                return 0;

            return base.Distance(secondPoint);
        }

        private void OnValidate()
        {
            if (connectedPortalPoint == this)
            {
                connectedPortalPoint = null;
                Debug.LogWarning($"{name}: cant set {nameof(connectedPortalPoint)} at itself!");
            }
        }

        protected override void OnDrawGizmos()
        {
            if (Pathfinder.ShowConnections)
            {
                Gizmos.color = Color.yellow;

                foreach (PathPoint point in connectedPoints)
                    if (point && point != connectedPortalPoint)
                        Gizmos.DrawLine(Position, point.Position);

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(Position, 0.2f);

                if (connectedPortalPoint != null)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(Position, connectedPortalPoint.Position);
                }

                if (portalOffset != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(Position, portalOffset.position);
                }
            }
        }
        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            foreach (PathPoint point in connectedPoints)
                if (point && point != connectedPortalPoint)
                    Gizmos.DrawLine(Position, point.Position);

            Gizmos.DrawSphere(Position, 0.3f);

            if (connectedPortalPoint != null)
            {
                Gizmos.DrawSphere(connectedPortalPoint.Position, 0.6f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(Position, connectedPortalPoint.Position);
            }

            if (portalOffset != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Position, portalOffset.position);
            }
        }
    }
}

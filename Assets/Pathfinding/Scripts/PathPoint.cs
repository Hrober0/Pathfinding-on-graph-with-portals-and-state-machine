using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Pathfinding
{
    public class PathPoint : MonoBehaviour
    {
        [HideInInspector] public PathPoint lastPoint;
        [HideInInspector] public float fCost; // summary cost
        [HideInInspector] public float gCost; // distance to start
        [HideInInspector] public float hCost; // distance to end


        [SerializeField, Min(0)] float maxNeighbourDistance = 60f;


        [SerializeField, ReadOnly] protected List<PathPoint> connectedPoints = new List<PathPoint>();
        public IReadOnlyList<PathPoint> ConnectedPoints => connectedPoints;


        /// <summary>
        /// Add other path points in range to connectedPoints list
        /// </summary>
        /// <param name="allPoints">All path points on the map</param>
        public virtual void FindConnectedPoints(PathPoint[] allPoints)
        {
            if (connectedPoints != null)
                connectedPoints.Clear();
            else
                connectedPoints = new List<PathPoint>();

            LayerMask pathBlockers = Pathfinder.PathBlockers;

            foreach (PathPoint point in allPoints)
            {
                if (point == this)
                    continue;

                float dist = Vector3.Distance(transform.position, point.transform.position);
                if (dist <= maxNeighbourDistance)
                {
                    Ray ray = new Ray(transform.position, (point.transform.position - transform.position));
                    Physics.Raycast(ray, out RaycastHit hit, dist - 0.5f, pathBlockers);

                    if (hit.collider == null)
                        connectedPoints.Add(point);
                }
            }
        }

        /// <summary>
        /// Remove path points from connectedPoints list that do have not enough range 
        /// </summary>
        public virtual void FixConnectedPoints()
        {
            for (int i = 0; i < connectedPoints.Count; i++)
                if (!connectedPoints[i].connectedPoints.Contains(this))
                {
                    connectedPoints.RemoveAt(i);
                    i--;
                }
        }


        [Button("Update Points")]
        private void ButtonUdatePoints() => Pathfinder.UpdatePoints();


        /// <summary>
        /// Calculate distance to second point
        /// </summary>
        public virtual float Distance(PathPoint secondPoint) => Vector3.Distance(Position, secondPoint.Position);

        public Vector3 Position => transform.position;
        public string Name => $"{name}({transform.parent.name})";


        protected virtual void OnDrawGizmos()
        {
            if (Pathfinder.ShowConnections)
            {
                Gizmos.color = Color.yellow;

                foreach (PathPoint point in connectedPoints)
                    if (point)
                        Gizmos.DrawLine(Position, point.Position);

                Gizmos.DrawSphere(Position, 0.2f);
            }
        }
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            foreach (PathPoint point in connectedPoints)
                if (point)
                    Gizmos.DrawLine(Position, point.Position);

            Gizmos.DrawSphere(Position, 0.3f);
        }
    }
}
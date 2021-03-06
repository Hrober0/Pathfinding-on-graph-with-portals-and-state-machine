using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        private const float distanceToEndMtltiplier = 0.4f; // min 0
        private const float checkedDistanceInLineFinding = 100f;

        [SerializeField] private bool showConnections = false;

        private const int defultPathBlockers = 1 << 8;
        [SerializeField] private LayerMask pathBlockers = defultPathBlockers;

        private static PathPoint[] allPoints;


        [Button("Update Points")]
        private void ButtonUdatePoints() => UpdatePoints();


        private static Pathfinder instance;
        private static Pathfinder Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<Pathfinder>();
                return instance;
            }
        }

        public static LayerMask PathBlockers => Instance == null ? (LayerMask)defultPathBlockers : Instance.pathBlockers;
        public static bool ShowConnections => Instance == null ? false : Instance.showConnections;


        private void Awake()
        {
            UpdatePoints();
        }


        /// <summary>
        /// Update connections between path points
        /// </summary>
        public static void UpdatePoints()
        {
            allPoints = FindObjectsOfType<PathPoint>();
            foreach (PathPoint point in allPoints)
                point.FindConnectedPoints(allPoints);

            foreach (PathPoint point in allPoints)
                point.FixConnectedPoints();
        }


        /// <summary>
        /// Find path from start position to target position using pat points
        /// </summary>
        /// <returns>list of path points and the closest position near the target</returns>
        public static (List<PathPoint> path, Vector3 fixedTarget) FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            UpdatePoints();


            if (allPoints.Length <= 1)
                return (null, targetPosition);


            (PathPoint start, _) = FindClosestLine(startPosition);
            (PathPoint end, PathPoint endSecondPoint) = FindClosestLine(targetPosition);


            if (start == null || end == null)
            {
                Debug.LogWarning($"Dont found {(start == null ? "start" : "end")} path! (start: {startPosition} target: {targetPosition})");
                return (null, targetPosition);
            }


            if (start == end && endSecondPoint != null)
            {
                // swap end and endSecondPoint
                PathPoint t = end;
                end = endSecondPoint;
                endSecondPoint = t;
            }

            List<PathPoint> path = CreatePath(start, end);


            if (path == null)
                return (null, targetPosition);


            if (endSecondPoint != null && !path.Contains(endSecondPoint))
                path.Add(endSecondPoint);


            // fix target position
            if (path.Count == 1)
                targetPosition = path[path.Count - 1].Position;
            else if (path.Count > 1)
            {
                Vector3 previousPoint = path[path.Count - 2].Position;
                Vector3 endPoint = path[path.Count - 1].Position;
                Vector3 dir = endPoint - previousPoint;
                Vector3 step = dir.normalized * 0.5f;
                Vector3 fixedTarget = previousPoint;
                float closesetDistance = Vector3.Distance(fixedTarget, targetPosition);
                int steps = Mathf.FloorToInt(dir.magnitude / step.magnitude);
                for (int i = 0; i < steps; i++)
                {
                    fixedTarget += step;
                    float dist = Vector3.Distance(fixedTarget, targetPosition);
                    if (dist > closesetDistance)
                    {
                        fixedTarget -= step;
                        break;
                    }

                    closesetDistance = dist;
                }
                //Debug.Log($"Fixed target {targetPosition} => {fp} => {fixedTarget}");
                targetPosition = fixedTarget;
            }

            return (path, targetPosition);
        }

        /// <summary>
        /// Create path based on cost, that was set before
        /// </summary>
        private static List<PathPoint> CreatePath(PathPoint startPoint, PathPoint endPoint)
        {
            SortedCollection<PathPoint> openSet = new SortedCollection<PathPoint>((PathPoint first, PathPoint second) => first.fCost >= second.fCost);
            HashSet<PathPoint> closeSet = new HashSet<PathPoint>();

            startPoint.gCost = 0;
            startPoint.hCost = 0;
            startPoint.lastPoint = null;
            openSet.Add(startPoint);

            bool found = false;

            while (openSet.Count > 0)
            {
                PathPoint currPoint = openSet.GetFirst();

                closeSet.Add(currPoint);

                //Debug.Log($"Pathfinfing checking {currPoint.Name} (target: {endPoint.Name})");
                if (currPoint == endPoint)
                {
                    found = true;
                    break;
                }

                foreach (PathPoint neighbour in currPoint.ConnectedPoints)
                {
                    if (closeSet.Contains(neighbour) || openSet.Contains(neighbour))
                        continue;

                    // set consts
                    neighbour.gCost = currPoint.gCost + currPoint.Distance(neighbour);
                    neighbour.hCost = Vector3.Distance(neighbour.transform.position, endPoint.transform.position);
                    neighbour.fCost = neighbour.hCost * distanceToEndMtltiplier + neighbour.gCost;
                    neighbour.lastPoint = currPoint;

                    // add to open set
                    openSet.Add(neighbour);
                }
            }

            if (found)
            {
                // recrate path
                List<PathPoint> points = new List<PathPoint>() { startPoint };
                PathPoint point = endPoint;
                while (startPoint != point)
                {
                    if (points == null || points.Contains(point))
                    {
                        Debug.LogWarning("Path is invalid");
                        return null;
                    }

                    points.Insert(1, point);
                    point = point.lastPoint;
                }
                return points;
            }
            else
            {
                Debug.LogWarning($"Dont found valid path from {startPoint.Name} to {endPoint.Name}", endPoint);
                return null;
            }
        }


        /// <summary>
        /// Find the closest point near to given position
        /// </summary>
        public static PathPoint FindClosestPoint(Vector3 position)
        {
            if (allPoints.Length == 0)
                return null;

            PathPoint nearestPoint = allPoints[0];
            float minDist = Dist(nearestPoint);
            for (int i = 1; i < allPoints.Length; i++)
            {
                float dist = Dist(allPoints[i]);
                if (dist < minDist)
                {
                    nearestPoint = allPoints[i];
                    minDist = dist;
                }
            }
            return nearestPoint;

            float Dist(PathPoint point) => Vector3.Distance(point.transform.position, position);
        }

        /// <summary>
        /// Find the line between two connected points where distance to the position is the shortest 
        /// </summary>
        /// <returns>two extreme points of the found line where one is closer to given position</returns>
        private static (PathPoint closestPoint, PathPoint secondLinePoint) FindClosestLine(Vector3 position)
        {
            PathPoint firstLinePoint = null;
            PathPoint secondLinePoint = null;
            float shortestDistance = float.MaxValue;
            foreach (PathPoint firstpoint in allPoints)
            {
                float distanceToFPoint = Vector3.Distance(position, firstpoint.Position);
                if (distanceToFPoint > checkedDistanceInLineFinding)
                    continue;

                foreach (PathPoint secondPoint in firstpoint.ConnectedPoints)
                {
                    if (firstpoint is PathPointWithPortal pointWithPortal && secondPoint == pointWithPortal.ConnectedPortalPoint)
                    {
                        if (distanceToFPoint < shortestDistance)
                        {
                            firstLinePoint = firstpoint;
                            secondLinePoint = null;
                        }
                        continue;
                    }

                    float distanceToLine = DistanceToLine(position, firstpoint.Position, secondPoint.Position);
                    if (distanceToLine < shortestDistance)
                    {
                        shortestDistance = distanceToLine;
                        firstLinePoint = firstpoint;
                        secondLinePoint = secondPoint;
                    }
                }
            }

            if (firstLinePoint == null)
                return (FindClosestPoint(position), null);

            if (secondLinePoint == null)
                return (firstLinePoint, null);

            if (Vector3.Distance(position, firstLinePoint.Position) < Vector3.Distance(position, secondLinePoint.Position))
                return (firstLinePoint, secondLinePoint);
            else
                return (secondLinePoint, firstLinePoint);
        }


        private static float DistanceToLine(Vector3 point, Vector3 lineA, Vector3 lineB)
        {
            Vector3 ab = lineB - lineA;
            Vector3 av = point - lineA;

            if (Vector3.Dot(av, ab) <= 0f)
                return av.magnitude;

            Vector3 bv = point - lineB;

            if (Vector3.Dot(bv, ab) >= 0f)
                return bv.magnitude;

            return (Vector3.Cross(ab, av)).magnitude / (ab).magnitude;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PathPlanningUtility
{
    public NavMeshPath Path = null;
    public Vector3[] WayPoints = null;
    public bool FinalTargetReached = false;
    public int CurrTarget = 0;

    public PathPlanningUtility()
    {
        Path = new NavMeshPath();

    }

    public Vector3[] ComputePath(Vector3 pointA, Vector3 pointB)
    {

        //Find closest point
        NavMeshHit ClosestToA;
        if (!NavMesh.SamplePosition(pointA, out ClosestToA, 1.0f, 1))
            Debug.Log("No close point found for Point A");

        //Find closest point
        NavMeshHit ClosestToB;
        if (!NavMesh.SamplePosition(pointB, out ClosestToB, 1.0f, 1))
            Debug.Log("No close point found for Point B");

        if (!NavMesh.CalculatePath(ClosestToA.position, ClosestToB.position, 1, Path))
        {
            Debug.Log("Invalid path!");
        }
        else
        {
            WayPoints = new Vector3[Path.corners.Length];
            WayPoints[0] = pointA;
            WayPoints[WayPoints.Length - 1] = pointB;
            for (int i = 1; i < Path.corners.Length - 1; i++)
            {
                WayPoints[i] = Path.corners[i];
            }

            WayPoints = PreProcessWayPoints();
        }
        return WayPoints;
    }

    public Vector3[] PreProcessWayPoints()
    {
        List<Vector3> PrunedWayPoints = new List<Vector3>();
        PrunedWayPoints.Add(WayPoints[0]);
        for (int i = 1; i < WayPoints.Length - 1; i++)
        {
            // Get rid of points which are too close to each other 
            if (Vector3.Distance(WayPoints[i], WayPoints[WayPoints.Length - 1]) > 0.5f)
                PrunedWayPoints.Add(WayPoints[i]);
        }
        PrunedWayPoints.Add(WayPoints[WayPoints.Length - 1]);
        WayPoints = PrunedWayPoints.ToArray();
        return WayPoints;
    }

    public Vector3 GetNextMove(Vector3 root, Vector3 currForward, Vector3 finalTarget)
    {
        float thresh = 0.3f;

        if (FinalTargetReached || CurrTarget == (WayPoints.Length - 1))
        {
            FinalTargetReached = true;
            Debug.Log("Returning Final Target");
            return finalTarget;
        }
        else
        {
            var nextWayPoint = WayPoints[CurrTarget];
            if (Vector3.Distance(root, nextWayPoint) <= thresh)
            {
                CurrTarget += 1;
            }

            Vector3 ToCurrTarget = nextWayPoint - root;
            Vector3 move = Vector3.zero;

            if (Vector3.Angle(currForward, ToCurrTarget) < 45)
            {
                move.z += 1f; //Move forward
            }
            return move;
        }

    }

    public float GetNextTurn(Vector3 root, Vector3 currForward)
    {
        float thresh = 0.5f;
        var nextWayPoint = WayPoints[CurrTarget];
        if (Vector3.Distance(root, nextWayPoint) <= thresh)
        {
            CurrTarget += 1;
        }
        Vector3 ToCurrTarget = nextWayPoint - root;

        float turn = 0f;
        if (Vector3.SignedAngle(currForward, ToCurrTarget, Vector3.up) > 10)
        {
            turn += 90f;
        }
        else if (Vector3.SignedAngle(currForward, ToCurrTarget, Vector3.up) < -10)
        {
            turn -= 90f;
        }
        return turn;
    }

    public void DrawPath()
    {
        if (Path.corners.Length > 0)
        {
            UltiDraw.Begin();
            for (int i = 0; i < WayPoints.Length; i++)
            {
                UltiDraw.DrawSphere(WayPoints[i], Quaternion.identity, 0.2f, UltiDraw.Black);
                if (i < WayPoints.Length - 1)
                { UltiDraw.DrawLine(WayPoints[i], WayPoints[i + 1], 0.03f, UltiDraw.Red); }
            }
            UltiDraw.End();
        }
    }

    public void ResetPath()
    {
        Path.ClearCorners();
        WayPoints = null;
        CurrTarget = 0;
        FinalTargetReached = false;
    }
}

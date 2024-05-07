using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountingRectangles : MonoBehaviour
{
    List<Point> points;

    private void Start()
    {
        points = FindObjectsOfType<Transform>()
            .Where(t => t.name.StartsWith("Sphere"))
            .Select(t => new Point(t.position.x, t.position.y))
            .ToList();

        Debug.Log("# of points: " + points.Count);

        StartCoroutine(CountRectangles(points));
    }

    private class Point : IComparable<Point>
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, 0);
        }

        public int CompareTo(Point p2)
        {
            if (x == p2.x)
            {
                return y.CompareTo(p2.y);
            }
            return x.CompareTo(p2.x);
        }

        public override string ToString()
        {
            return x.ToString() + "," + y.ToString();
        }
    }

    private IEnumerator CountRectangles(List<Point> points)
    {
        int count = 0;
        float delay = 0.5f;

        SortedSet<Point> pointsSet = new();

        foreach (Point p in points)
        {
            pointsSet.Add(p);
        }

        for (int i = 0; i < pointsSet.Count - 1; i++)
        {
            for (int j = i + 1; j < pointsSet.Count; j++)
            {
                Point p1 = pointsSet.ElementAt(i);
                Point p2 = pointsSet.ElementAt(j);

                if (p2.x == p1.x || p2.y == p1.y)
                {
                    continue;
                }

                Point p3 = new(p1.x, p2.y);
                Point p4 = new(p2.x, p1.y);

                DrawRectangle(p2, p3, p4, p1, delay);
                yield return new WaitForSeconds(delay);

                if (pointsSet.Contains(p3) && pointsSet.Contains(p4))
                {
                    count++;
                }
            }
        }

        Debug.Log("Rect Count (each rect counted twice): " + count + " / 2 = " + count / 2);
    }

    private void DrawRectangle(Point p1, Point p2, Point p3, Point p4, float duration)
    {
        Debug.Log(p1 + " | " + p2 + " | " + p3 +" | " + p4);
        Debug.DrawLine(p1.ToVector3(), p2.ToVector3(), Color.red, duration);
        Debug.DrawLine(p1.ToVector3(), p3.ToVector3(), Color.green, duration);
        Debug.DrawLine(p4.ToVector3(), p2.ToVector3(), Color.blue, duration);
        Debug.DrawLine(p4.ToVector3(), p3.ToVector3(), Color.yellow, duration);
    }
}

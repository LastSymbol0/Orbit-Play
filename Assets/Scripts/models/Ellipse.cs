using UnityEngine;

namespace models
{
    [System.Serializable]
    public class Ellipse
    {
        public float xAxis;
        public float yAxis;

        public Ellipse(float x, float y)
        {
            xAxis = x;
            yAxis = y;
        }

        public Vector2 Evaluate(float t)
        {
            float angle = Mathf.Deg2Rad * 360f * t;

            float x = Mathf.Sin(angle) * xAxis;
            float y = Mathf.Cos(angle) * yAxis;

            return new Vector2(x, y);
        }
    }
}
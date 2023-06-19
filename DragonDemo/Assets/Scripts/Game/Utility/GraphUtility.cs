using UnityEngine;

namespace Koakuma.Utility
{
    public static class GraphUtility
    {
        /// <summary>
        /// Բ����ײ
        /// </summary>
        /// <param name="center1"></param>
        /// <param name="radius1"></param>
        /// <param name="center2"></param>
        /// <param name="radius2"></param>
        /// <returns></returns>
        public static bool CircleCollide2D(Vector3 center1, float radius1, Vector3 center2, float radius2)
        {
            float sqrDist = (center1 - center2).ToXZPlane().sqrMagnitude;
            float sqrRadius = radius1 * radius1 + radius2 * radius2 + 2 * radius1 * radius2;
            return sqrDist <= sqrRadius;
        }

        /// <summary>
        /// ����Բ�ε���ײ
        /// </summary>
        /// <param name="sectorCenter"></param>
        /// <param name="sectorRotation"></param>
        /// <param name="sectorRadius"></param>
        /// <param name="sectorAngle"></param>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <returns></returns>
        public static bool SectorCircleCollide2D(Vector3 sectorCenter, Quaternion sectorRotation, float sectorRadius, float sectorAngle, Vector3 circleCenter, float circleRadius)
        {
            if (!CircleCollide2D(sectorCenter, sectorRadius, circleCenter, circleRadius))
                return false;

            Vector3 forward = sectorRotation * Vector3.forward;
            float angle = Vector3.Angle(forward, (circleCenter - sectorCenter).ToXZPlane());
            if (angle > sectorAngle / 2f)
                return false;

            return true;
        }

        /// <summary>
        /// ������Բ����ײ
        /// +----width---+
        /// |      z     |
        /// |      ^     |
        /// |      |     |
        /// |      |     |
        /// |      |   height
        /// |      |     |
        /// |      |     |
        /// |      |     |
        /// |      |     |
        /// +-rectCenter-+
        /// </summary>
        /// <param name="rectCenter"></param>
        /// <param name="rectRotation"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="circleCenter"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool RectangleCircleCollide2D(Vector3 rectCenter, Quaternion rectRotation, float width, float height, Vector3 circleCenter, float radius)
        {
            Quaternion worldToLocalRotation = Quaternion.Inverse(rectRotation);
            Vector3 toCircle = circleCenter - rectCenter;
            Vector3 locallTargetPosition = worldToLocalRotation * toCircle;
            if (locallTargetPosition.x - radius > width / 2f)
                return false;

            if (locallTargetPosition.x + radius < -width / 2f)
                return false;

            if (locallTargetPosition.z + radius < 0)
                return false;

            if (locallTargetPosition.z - radius > height)
                return false;

            return true;
        }
    }
}

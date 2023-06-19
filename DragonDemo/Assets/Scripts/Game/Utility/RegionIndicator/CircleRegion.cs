using Koakuma.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Koakuma.Utility
{
    public class CircleRegion : MonoBehaviour
    {
        public float radius = 1;
        public int segment = 24;
        public bool autoGenerate;

        private MeshFilter filter;

        private void Awake()
        {
            if (autoGenerate)
            {
                GenerateMesh(radius);
            }
        }

        private void Update()
        {
            if (releaseDelay <= 0)
            {
                GameManager.Asset.UnloadGameObject(gameObject);
            }
            releaseDelay--;
        }

        public void GenerateMesh(float radius)
        {
            if (filter == null)
            {
                filter = GetComponent<MeshFilter>();
            }

            this.radius = radius;
            Mesh mesh = new Mesh();

            Vector3 pos = Vector3.forward * radius;
            Quaternion rot = Quaternion.Euler(0, 360f / segment, 0);

            List<Vector3> vertices = new List<Vector3>() { Vector3.zero, pos };
            List<int> triangles = new List<int>();

            for (int i = 0; i < segment; i++)
            {
                Vector3 next = rot * pos;
                vertices.Add(next);

                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i + 2);

                pos = next;
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            filter.mesh = mesh;
        }

        private int releaseDelay;
        public void Init()
        {
            releaseDelay = 3;
        }

        public static void Show(float radius, Vector3 center)
        {
            if (!GameSettings.RegionIndicatorEnabled)
                return;

            CircleRegion region = GameManager.Asset.LoadGameObject("Assets/Prefabs/RegionIndicator/CircleSkill.prefab").GetComponent<CircleRegion>();
            region.gameObject.SetActive(true);
            region.GenerateMesh(radius);
            region.transform.position = center + Vector3.up * 0.1f;
            region.Init();
        }
    }
}

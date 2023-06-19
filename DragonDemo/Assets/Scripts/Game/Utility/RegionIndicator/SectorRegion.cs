using Koakuma.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Koakuma.Utility
{
    public class SectorRegion : MonoBehaviour
    {
        public float radius = 1;
        public int segment = 24;
        public float angle = 30;
        public bool autoGenerate;

        private MeshFilter filter;

        private void Awake()
        {
            filter = GetComponent<MeshFilter>();

            if (autoGenerate)
            {
                GenerateMesh(radius, angle, transform.forward);
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

        public void GenerateMesh(float radius, float angle, Vector3 forward)
        {
            this.radius = radius;
            this.angle = angle;
            Mesh mesh = new Mesh();

            Vector3 pos = Quaternion.Euler(0, -angle / 2, 0) * Vector3.forward * radius;
            Quaternion rot = Quaternion.Euler(0, angle / segment, 0);

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

            transform.forward = forward;
        }

        private int releaseDelay;
        public void Init()
        {
            releaseDelay = 3;
        }

        public static void Show(Vector3 center, float radius, float angle, Vector3 forward)
        {
            if (!GameSettings.RegionIndicatorEnabled)
                return;

            SectorRegion region = GameManager.Asset.LoadGameObject("Assets/Prefabs/RegionIndicator/SectorSkill.prefab").GetComponent<SectorRegion>();
            region.gameObject.SetActive(true);
            region.GenerateMesh(radius, angle, forward);
            region.transform.position = center + Vector3.up * 0.1f;
            region.Init();
        }
    }
}

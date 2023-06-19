using Koakuma.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Koakuma.Utility
{
    public class RectangleRegion : MonoBehaviour
    {
        public float width = 0.5f;
        public float height = 1f;
        public bool autoGenerate;

        private MeshFilter filter;

        private void Awake()
        {
            filter = GetComponent<MeshFilter>();

            if (autoGenerate)
            {
                GenerateMesh(width, height, transform.forward);
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

        public void GenerateMesh(float width, float height, Vector3 forward)
        {
            this.width = width;
            this.height = height;
            transform.forward = forward;
            Mesh mesh = new Mesh();


            List<Vector3> vertices = new List<Vector3>()
            {
                new Vector3(-width / 2, 0, 0),
                new Vector3(-width / 2, 0, height),
                new Vector3(width / 2, 0, height),
                new Vector3(width / 2, 0, 0),
            };
            List<int> triangles = new List<int>()
            {
                0,1,2,
                0,2,3
            };

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            filter.mesh = mesh;
        }

        private int releaseDelay;
        public void Init()
        {
            releaseDelay = 3;
        }

        public static void Show(Vector3 center, float width, float height, Vector3 forward)
        {
            if (!GameSettings.RegionIndicatorEnabled)
                return;

            RectangleRegion region = GameManager.Asset.LoadGameObject("Assets/Prefabs/RegionIndicator/RectangleSkill.prefab").GetComponent<RectangleRegion>();
            region.gameObject.SetActive(true);
            region.GenerateMesh(width, height, forward);
            region.transform.position = center + Vector3.up * 0.1f;
            region.Init();
        }
    }
}

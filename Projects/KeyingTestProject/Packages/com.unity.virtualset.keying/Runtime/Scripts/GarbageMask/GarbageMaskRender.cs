
namespace UnityEngine.VirtualSet.Keying
{
    public class GarbageMaskRender : MonoBehaviour
    {
        // Start is called before the first frame update
        private Mesh m_Mesh;

        // Baked green screen
        Vector3[] positions = {
            new Vector3(-1.43f, 0, -0.38f),
            new Vector3(-1.45f, 0, -1.05f),
            new Vector3(-1.47f, 0.70f, -1.60f),
            new Vector3(-1.47f, 2.27f, -1.63f),
            new Vector3(1.29f, 2.29f, -1.66f),
            new Vector3(1.30f, 0.67f, -1.63f),
            new Vector3(1.32f, 0, -1.10f),
            new Vector3(1.33f, 0, -0.39f)
        };

        int[] triangles = {
            5, 4, 3,
            5, 3, 2,
            5, 2, 1,
            5, 1, 6,
            6, 1, 0,
            6, 0, 7
        };

        void Start()
        {
            m_Mesh = new Mesh();

            m_Mesh.vertices = positions;
            m_Mesh.triangles = triangles;

            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();

            Material newMat = new Material(Shader.Find("Unlit/Color"));
            newMat.color =  new Color(1f,1f,1f,1f);;

            GetComponent<MeshRenderer>().sharedMaterial = newMat;

            //m_Mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = m_Mesh;
        }
    }
}

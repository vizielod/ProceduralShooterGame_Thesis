using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{

    [SerializeField] public GameObject m_fogOfWarRaycaster;
    
    [SerializeField] public GameObject m_fogOfWarPlane;

    [SerializeField] public Transform m_player;

    [SerializeField] public LayerMask m_fogLayer;

    [SerializeField] public float m_radius = 5f;

    public bool startUpdate = false;
    private float m_radiusSqr
    {
        get { return m_radius * m_radius; }
    }

    private Mesh m_mapMesh;
    
    private Vector3[] m_vertices;

    private List<Vector3> vertices = new List<Vector3>();

    private Color[] m_colors;

    private bool LocalToWorldTransformed = false;
    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 0; i < m_vertices.Length; i++)
        {
            vertices[i] = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (startUpdate)
        {
            /*if (!LocalToWorldTransformed)
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    vertices[i] = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
                }

                LocalToWorldTransformed = true;
            }*/

            Ray r = new Ray(m_fogOfWarRaycaster.transform.position,
                m_player.position - m_fogOfWarRaycaster.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, m_fogLayer, QueryTriggerInteraction.Collide))
            {
                //TODO: Optimize it to not to check collision with all the vertices of the plane
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    //Vector3 v = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
                    Vector3 v = vertices[i];
                    float dist =
                        Vector3.SqrMagnitude(v - hit.point); //compute distance between intersection point and vertices
                    if (dist < m_radiusSqr)
                    {
                        //set the vertex from black to transparent
                        //alpha gets lover -> more transparent, if a point is closer to the intersection point
                        float
                            alpha = Mathf.Min(m_colors[i].a,
                                dist /
                                m_radiusSqr); //We use Min so it cannot be set back to black, alpha can get only smaller -> increasing transparency
                        m_colors[i].a = alpha;
                    }

                    UpdateColors();
                }
            }
        }
    }

    public void TransformVerticesFromLocalToWorld()
    {
        for (int i = 0; i < m_vertices.Length; i++)
        {
            vertices.Add(m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]));
        }
    }

    public void Initialize()
    {
        m_mapMesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_fogOfWarPlane.GetComponent<MeshCollider>();
        m_vertices = m_mapMesh.vertices;
        m_colors = new Color[m_vertices.Length];
        for (int i = 0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.black;
        }

        UpdateColors();
    }

    void UpdateColors()
    {
        m_mapMesh.colors = m_colors;
    }
    
}

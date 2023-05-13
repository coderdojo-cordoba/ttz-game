using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientBackground : MonoBehaviour
{

    public Gradient color;
    public Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        
    }

    // Update is called once per frame
    void Update()
    {
        Color[] colors = new Color[mesh.vertices.Length];
        
        colors[0] = color.Evaluate(0f);
        colors[1] = color.Evaluate(0.25f);
        colors[2] = color.Evaluate(0.75f);
        colors[3] = color.Evaluate(1f);
        mesh.colors = colors;
    }
}

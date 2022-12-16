using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Draws a 2D grid in the scene for spaptial orientation
/// </summary>

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    [SerializeField] private float drawDistance = 1000f;

    [SerializeField] private bool showXY;
    [SerializeField] private bool showXZ;
    [SerializeField] private bool showYZ;
    
    
    [SerializeField] private GridLineProperties major;
    [SerializeField] private GridLineProperties minor;


    private new Camera camera;

    [System.Serializable]
    public class GridLineProperties
    {
        [Min(0.01f)] public float spacing = 10f;
        [Min(0f)] public float thickness = 0f;
        public Material material;
        public Color color = Color.white;
        public bool show = true;
    }

    void Update() { }
    
    void OnEnable() => RenderPipelineManager.beginCameraRendering += RegisterCamera;
    void OnDisable() => RenderPipelineManager.beginCameraRendering -= RegisterCamera;
    

    private void RegisterCamera(ScriptableRenderContext context, Camera camera)
    {
        if (Application.isPlaying) this.camera = camera;
        else if (Camera.current != null) this.camera = Camera.current;
    }
    
    void OnRenderObject()
    {
        
        //Debug.Log(camera + " " + Camera.main+" "+Camera.current+" "+(Camera.current == null)+" "+(camera == Camera.current));
        if (enabled && camera == Camera.main || camera == Camera.current)
        {
            GL.Begin(GL.LINES);
            if (showXY) DrawPlane(Vector3.forward);
            if (showXZ) DrawPlane(Vector3.right);
            if (showYZ) DrawPlane(Vector3.up);
            GL.End();
        }
        
    }


    private void DrawPlane(Vector3 up)
    {
        Vector3 x = Quaternion.Euler(90f * Vector3.up) * up;
        Vector3 y = Quaternion.Euler(90f * Vector3.right) * up;

        Vector3 base0_x = -y * drawDistance;
        Vector3 base1_x = y * drawDistance;
        Vector3 base0_y = -x * drawDistance;
        Vector3 base1_y = x * drawDistance;
        
        if (minor.show)
        {
            if (minor.material != null) minor.material.color = minor.color;
            if (minor.material != null) minor.material.SetPass(0);
            GL.Color(minor.color);
            int Nminor = (int)(drawDistance / minor.spacing);
            DrawLines(x, base0_x, base1_x, Nminor, minor.spacing, minor.thickness);
            DrawLines(y, base0_y, base1_y, Nminor, minor.spacing, minor.thickness);
        }

        GL.End();
        GL.Begin(GL.LINES);
        if (major.show)
        {
            if (major.material != null) major.material.color = major.color;
            if (major.material != null) major.material.SetPass(0);
            GL.Color(major.color);
            int Nmajor = (int)(drawDistance / major.spacing);
            DrawLines(x, base0_x, base1_x, Nmajor, major.spacing, major.thickness);
            DrawLines(y, base0_y, base1_y, Nmajor, major.spacing, major.thickness);
        }
    }

    private void DrawLines(Vector3 direction, Vector3 base0, Vector3 base1, int N, float spacing, float thickness = 0f)
    {
        Vector3 p0, p1;
        for (int i = -N; i <= N; i++)
        {
            p0 = base0 + i * spacing * direction;
            p1 = base1 + i * spacing * direction;
            DrawLine(p0, p1, thickness);
        }
    }

    // http://answers.unity.com/answers/1614973/view.html
    public void DrawLine(Vector3 p1, Vector3 p2, float width)
    {
        int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
        {
            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);
        }
        else
        {
            //Camera camera = Camera.main;
            if (camera == null) return;
            
            var scp1 = camera.WorldToScreenPoint(p1);
            var scp2 = camera.WorldToScreenPoint(p2);
 
            Vector3 v1 = (scp2 - scp1).normalized; // line direction
            Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector
 
            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = camera.ScreenToWorldPoint(scp1 + o);
                Vector3 dest = camera.ScreenToWorldPoint(scp2 + o);
                GL.Vertex3(origin.x, origin.y, origin.z);
                GL.Vertex3(dest.x, dest.y, dest.z);
            }
        }
    }
    
}

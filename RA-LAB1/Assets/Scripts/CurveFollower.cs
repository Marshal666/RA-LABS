using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveFollower : MonoBehaviour
{

    public BCurve Curve;

    public float speed = 0.5f;

    int followMode = 0;

    public MeshFilter mf;

    public bool drawTangent = true;
    public LineRenderer TangentDrawer;

    Mesh original;

	private void Start()
	{
        if (!mf)
            mf = GetComponent<MeshFilter>();
        if (!TangentDrawer)
        {
            TangentDrawer = GetComponent<LineRenderer>();
            if (!TangentDrawer)
            {
                TangentDrawer = gameObject.AddComponent<LineRenderer>();
            }
        }
        original = mf.mesh.Clone();
        Vector3 avg = original.vertices.Average();
        original.vertices.AddTo(-avg);
	}

	public void ChangeMode(Dropdown change)
    {
        followMode = change.value;
        mf.mesh = original;
    }

    public void UnityTransformFollow(float t)
	{
        transform.position = Curve.Evaluate(t);
        transform.rotation = Quaternion.LookRotation(Curve.GetDirection(t), Vector3.up);
    }

    public void MeshVerticesEditFollow(float t)
	{
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        Vector3 w = Curve.GetDirection(t);
        Vector3 sd = Curve.SecondDerivative(t);
        if(sd == Vector3.zero)
		{
            sd = Vector3.up;
		}
        Vector3 u = Vector3.Cross(w, sd.normalized).normalized;
        Vector3 v = Vector3.Cross(w, u).normalized;

        Matrix4x4 R = new Matrix4x4(
            u.ToVector4(), -v.ToVector4(), w.ToVector4(), new Vector4(0, 0, 0, 1)
            );
        Matrix4x4 RI = R.inverse;
        Mesh newMesh = original.Clone();
        Vector3[] verts = newMesh.vertices;
        Vector3 newPos = (Curve.Evaluate(t)/* + Curve.Evaluate(0)*/) * 2f;
        for(int i = 0; i < verts.Length; i++)
		{
            verts[i] = R * verts[i];
            verts[i] += newPos;
		}
        newMesh.vertices = verts;
        newMesh.RecalculateNormals();
        mf.mesh = newMesh;
	}

    public void MeshVerticesEditFollowQ(float t)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        //
        Quaternion rot = Quaternion.LookRotation(Curve.GetDirection(t));

        Mesh newMesh = original.Clone();
        Vector3[] verts = newMesh.vertices;
        Vector3 newPos = (Curve.Evaluate(t)/* + Curve.Evaluate(0)*/) * 2f;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = rot * verts[i];
            verts[i] += newPos;
        }
        newMesh.vertices = verts;
        newMesh.RecalculateNormals();
        mf.mesh = newMesh;
    }

    float t = 0;
    // Update is called once per frame
    void Update()
    {
        if (t >= 1f)
            t = 0f;
        if (t < 0f)
            t = 1f;
        switch(followMode)
		{
            case 0:
                UnityTransformFollow(t);
                break;
            case 1:
                MeshVerticesEditFollow(t);
                break;
            case 2:
                MeshVerticesEditFollowQ(t);
                break;
            default:
                break;
		}

        //draw tangent
        Vector3 p = Curve.Evaluate(t);
        Vector3 dir = Curve.GetDirection(t);
        if (drawTangent)
        {
            TangentDrawer.SetPosition(0, p);
            TangentDrawer.SetPosition(1, p + dir * 2f);
        }

        t += Time.deltaTime * speed;
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Vector3 p = Curve.Evaluate(t);
        Vector3 dir = Curve.GetDirection(t);
        Gizmos.DrawLine(p, p + dir * 2);
	}
}

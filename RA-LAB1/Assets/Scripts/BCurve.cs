using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BCurve : MonoBehaviour
{

    public Vector3[] Points;

	new public LineRenderer renderer;

	public bool drawCurve = true;

	public void SwapDrawCurve(Toggle t)
	{
		drawCurve = t.isOn;
		if (renderer)
			renderer.enabled = drawCurve;
	}

	private void Start()
	{
		if (!renderer)
			renderer = GetComponent<LineRenderer>();
		InitRenderer();
	}

	static readonly System.Numerics.Matrix4x4 M = new System.Numerics.Matrix4x4(
	   -1, 3, -3, 1,
	    3, -6, 3, 0,
       -3, 0, 3, 0,
	    1, 4, 1, 0
	);

	static readonly System.Numerics.Matrix4x4 MD = new System.Numerics.Matrix4x4(
		-1, 3, -3, 1,
		2, -4, 2, 0,
	   -1, 0, 1, 0,
		0, 0, 0, 0
		//new Vector4(-1, 2, -1, 0),
		//new Vector4(3, -4, 0, 0),
		//new Vector4(-3, 2, 1, 0),
		//new Vector4(1, 0, 0, 0)

		);

    //public System.Numerics.Matrix4x4 P = new System.Numerics.Matrix4x4();

    public Vector3 Evaluate(float t, int i)
	{
		System.Numerics.Vector4 T = new System.Numerics.Vector4(t * t * t, t * t, t, 1) * (1f / 6f);
		System.Numerics.Matrix4x4 P = new System.Numerics.Matrix4x4(
			Points[i - 1].x, Points[i - 1].y, Points[i - 1].z, 1,
			Points[i].x, Points[i].y, Points[i].z, 1,
			Points[i + 1].x, Points[i + 1].y, Points[i + 1].z, 1,
			Points[i + 2].x, Points[i + 2].y, Points[i + 2].z, 1
			);
		System.Numerics.Vector4 tmp = System.Numerics.Vector4.Transform(T, M);
		System.Numerics.Vector4 res = System.Numerics.Vector4.Transform(tmp, P);
        return new Vector3(res.X, res.Y, res.Z);
	}

	public Vector3 Evaluate(float t)
	{
		int cnt = Points.Length - 3;
		int inx = Mathf.CeilToInt(t * cnt);
		if (inx > Points.Length - 3)
			inx = Points.Length - 3;
		else if (inx < 1)
			inx = 1;
		return Evaluate(t * cnt - (inx - 1), inx);		
	}

	public Vector3 GetDirection(float t, int i)
	{
		System.Numerics.Vector4 T = new System.Numerics.Vector4(t * t, t, 1, 0) * (1f / 2f);
		System.Numerics.Matrix4x4 P = new System.Numerics.Matrix4x4(
			Points[i - 1].x, Points[i - 1].y, Points[i - 1].z, 1,
			Points[i].x, Points[i].y, Points[i].z, 1,
			Points[i + 1].x, Points[i + 1].y, Points[i + 1].z, 1,
			Points[i + 2].x, Points[i + 2].y, Points[i + 2].z, 1
			);
		
		System.Numerics.Vector4 tmp = System.Numerics.Vector4.Transform(T, MD);
		System.Numerics.Vector4 res = System.Numerics.Vector4.Transform(tmp, P);
		return new Vector3(res.X, res.Y, res.Z).normalized;
	}

	public Vector3 GetDirection(float t)
	{
		int cnt = Points.Length - 3;
		int inx = Mathf.CeilToInt(t * cnt);
		if (inx > Points.Length - 3)
			inx = Points.Length - 3;
		else if (inx < 1)
			inx = 1;
		return GetDirection(t * cnt - (inx - 1), inx);
	}

	public Vector3 SecondDerivative(float t, float keps = 0.0000000001f)
	{
		if(t + keps >= 1f)
		{
			return ((GetDirection(t) - GetDirection(t - keps)) / keps);
		} else
		{
			return ((GetDirection(t + keps) - GetDirection(t)) / keps);
		}
	}

	// Update is called once per frame
	public void Update()
    {

    }

	public float drawKEpsilon = 0.005f;
	void InitRenderer()
	{
		Vector3[] pts = new Vector3[Mathf.CeilToInt(1f / drawKEpsilon)];
		int inx = 1;
		for(float t = drawKEpsilon; t <= 1f; t += drawKEpsilon * 2)
		{
			pts[inx - 1] = Evaluate(t - drawKEpsilon);
			pts[inx] = Evaluate(t);
			inx++;
			inx++;
		}
		renderer.positionCount = pts.Length;
		renderer.SetPositions(pts);
		
	}

	public bool drawGizmos = true;
	public float kEpsilon = 0.01f;
	public float tangentkEpsilon = 0.1f;
	public bool drawTangents = true;
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0.6f, 0, 1f);
		foreach (var p in Points)
			Gizmos.DrawSphere(p, 0.075f);
		if (drawGizmos)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(transform.position, 0.075f);
			
			Gizmos.color = Color.red;
			DrawCurve();

			if (drawTangents)
			{
				Gizmos.color = Color.green;
				for (float t = tangentkEpsilon; t <= 1f; t += tangentkEpsilon)
				{
					Vector3 p1 = Evaluate(t);
					Vector3 dir = GetDirection(t);
					Gizmos.DrawLine(p1, p1 + dir * 2f);
				}
			}

			void DrawCurve()
			{
				for (int i = 1; i <= Points.Length - 3; i++)
				{
					for (float t = kEpsilon; t <= 1f; t += kEpsilon)
					{
						Vector3 p1 = Evaluate(t - kEpsilon, i);
						Vector3 p2 = Evaluate(t, i);
						Gizmos.DrawLine(p1, p2);
					}
				}
			}
		}
	}
}

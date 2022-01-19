
using System.Linq;

public static class Utils
{

	public static UnityEngine.Matrix4x4 ToUnityMatrix(this System.Numerics.Matrix4x4 m)
	{
		return new UnityEngine.Matrix4x4(
			new UnityEngine.Vector4(m.M11, m.M21, m.M31, m.M41),
			new UnityEngine.Vector4(m.M12, m.M22, m.M32, m.M42),
			new UnityEngine.Vector4(m.M13, m.M23, m.M33, m.M43),
			new UnityEngine.Vector4(m.M14, m.M24, m.M34, m.M44)
			);
	}

	public static UnityEngine.Mesh Clone(this UnityEngine.Mesh m)
	{
		UnityEngine.Mesh ret = new UnityEngine.Mesh();
		ret.vertices = m.vertices;
		ret.triangles = m.triangles.ToArray();
		ret.name = m.name;
		ret.boneWeights = m.boneWeights.ToArray();
		ret.colors = m.colors;
		ret.uv = m.uv;
		return ret;
	}

	public static UnityEngine.Vector4 ToVector4(this UnityEngine.Vector3 v)
	{
		return new UnityEngine.Vector4(v.x, v.y, v.z, 0);
	}

	public static UnityEngine.Vector3 Average(this UnityEngine.Vector3[] arr)
	{
		UnityEngine.Vector3 ret = new UnityEngine.Vector3();
		if (arr == null || arr.Length == 0)
			return ret;
		foreach (var v in arr)
			ret += v;
		return ret / arr.Length;
	}

	public static void AddTo(this UnityEngine.Vector3[] arr, UnityEngine.Vector3 n)
	{
		for (int i = 0; i < arr.Length; i++)
			arr[i] += n;
	}

}

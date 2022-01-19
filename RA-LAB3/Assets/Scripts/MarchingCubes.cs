using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{

    public float ValueMax = 1;

    public float ValueMin = 0;

    public float Surface = 0.4f;

    public int Width = 8, Height = 8, Depth = 8;

    public float SegmentSize = 0.5f;

    public Vector3Int ChunkSize = new Vector3Int(8, 8, 8);

    public MarchingCubesChunk[,,] Chunks;

    public Material ChunkMaterial;

    public PhysicMaterial DefaultChunkPhysicMaterial;

    public static MarchingCubes Instance;

    public Vector3 MinPoint => transform.position - new Vector3(Width * SegmentSize * ChunkSize.x, Height * SegmentSize * ChunkSize.y, Depth * SegmentSize * ChunkSize.z) / 2;
    public Vector3 MaxPoint => transform.position + new Vector3(Width * SegmentSize * ChunkSize.x, Height * SegmentSize * ChunkSize.y, Depth * SegmentSize * ChunkSize.z) / 2;

    public Vector3 FullSize => new Vector3((Width - 1) * SegmentSize * ChunkSize.x, (Height - 1) * SegmentSize * ChunkSize.y, (Depth - 1) * SegmentSize * ChunkSize.z);

    public Vector3 ChunckVolume => (ChunkSize - Vector3Int.one).ToVector3() * SegmentSize;

    public Vector3Int ValuesCount => ChunkSize * new Vector3Int(Width, Height, Depth);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3Int ChunckPos(Vector3 pos)
    {
        Vector3 res = pos - MinPoint;
        res.x /= (ChunkSize.x - 1) * SegmentSize;
        res.y /= (ChunkSize.y - 1) * SegmentSize;
        res.z /= (ChunkSize.z - 1) * SegmentSize;
        return new Vector3Int((int)res.x, (int)res.y, (int)res.z);

    }

    public MarchingCubesChunk ChunkAt(Vector3Int i) => Chunks[i.x, i.y, i.z];

    public MarchingCubesChunk ChunkAt(Vector3 p) => ChunkAt(ChunckPos(p));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ValueAt(int x, int y, int z)
    {
        Vector3Int chk = new Vector3Int(x / ChunkSize.x, y / ChunkSize.y, z / ChunkSize.z);
        return ChunkAt(chk).Values[x - chk.x * ChunkSize.x, y - chk.y * ChunkSize.y, z - chk.z * ChunkSize.z];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetValue(int x, int y, int z, float v, bool redraw = false)
    {
        Vector3Int chk = new Vector3Int(x / ChunkSize.x, y / ChunkSize.y, z / ChunkSize.z);
        Vector3Int inx = new Vector3Int(x - chk.x * ChunkSize.x, y - chk.y * ChunkSize.y, z - chk.z * ChunkSize.z);
        var c = ChunkAt(chk);
        if(c == null)
            throw new System.Exception("Non existent chunk at: " + chk);
        c.Values[inx.x, inx.y, inx.z] = v;
        if(redraw)
            ChunkAt(chk).Redraw();

        if (inx.x == 0 && (chk.x - 1) >= 0)
        {
            chk.x--;
            ChunkAt(chk).Values[ChunkSize.x - 1, inx.y, inx.z] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.x++;
        }
        if (inx.x == ChunkSize.x - 1 && (chk.x + 1) < Width)
        {
            chk.x++;
            ChunkAt(chk).Values[0, inx.y, inx.z] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.x--;
        }

        if (inx.y == 0 && (chk.y - 1) >= 0)
        {
            chk.y--;
            ChunkAt(chk).Values[inx.x, ChunkSize.y - 1, inx.z] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.y++;
        }
        if (inx.y == ChunkSize.y - 1 && (chk.y + 1) < Height)
        {
            chk.y++;
            ChunkAt(chk).Values[inx.x, 0, inx.z] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.y--;
        }

        if (inx.z == 0 && (chk.z - 1) >= 0)
        {
            chk.z--;
            ChunkAt(chk).Values[inx.x, inx.y, ChunkSize.z - 1] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.z++;
        }
        if (inx.z == ChunkSize.z - 1 && (chk.z + 1) < Depth)
        {
            chk.z++;
            ChunkAt(chk).Values[inx.x, inx.y, 0] = v;
            if (redraw)
                ChunkAt(chk).Redraw();
            chk.z--;
        }
    }

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Chunks = new MarchingCubesChunk[Width, Height, Depth];
        Vector3 p = MinPoint;
        Vector3 o = p;
        for(int i = 0; i < Chunks.GetLength(0); i++)
        {
            p.y = o.y;
            for(int j = 0; j < Chunks.GetLength(1); j++)
            {
                p.z = o.z;
                for(int k = 0; k < Chunks.GetLength(2); k++)
                {
                    GameObject g = new GameObject("Chunk_" + i + "_" + j + "_" + k);
                    Transform t = g.transform;
                    t.position = p;
                    t.parent = transform;
                    var ch = Chunks[i, j, k] = g.AddComponent<MarchingCubesChunk>();

                    var mr = g.GetComponent<MeshRenderer>();
                    mr.sharedMaterial = ChunkMaterial;
#if UNITY_EDITOR
                    ch.DebugDraw = false;
#endif

                    
                    p.z += SegmentSize * (ChunkSize.z - 1);
                }
                p.y += SegmentSize * (ChunkSize.y - 1);
            }
            p.x += SegmentSize * (ChunkSize.x - 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawInSphere(Vector3 pos, float radius, float value)
    {

        Vector3Int ZpInx = ChunckPos(pos + Vector3.forward * radius);
        Vector3Int ZnInx = ChunckPos(pos - Vector3.forward * radius);

        Vector3Int XpInx = ChunckPos(pos + Vector3.right * radius);
        Vector3Int XnInx = ChunckPos(pos - Vector3.right * radius);

        Vector3Int YpInx = ChunckPos(pos + Vector3.up * radius);
        Vector3Int YnInx = ChunckPos(pos - Vector3.up * radius);

        for(int i = XnInx.x; i <= XpInx.x; i++)
        {
            if (i < 0 || i >= Width)
                continue;
            for(int j = YnInx.y; j <= YpInx.y; j++)
            {
                if (j < 0 || j >= Height)
                    continue;
                for(int k = ZnInx.z; k <= ZpInx.z; k++)
                {
                    if (k < 0 || k >= Depth)
                        continue;
                    Chunks[i,j,k].DrawInSphere(pos, radius, value);
                }
            }
        }
    }

    public void DrawInSphereLossy(Vector3 pos, float radius, float value)
    {
        Vector3Int ZpInx = ChunckPos(pos + Vector3.forward * radius);
        Vector3Int ZnInx = ChunckPos(pos - Vector3.forward * radius);

        Vector3Int XpInx = ChunckPos(pos + Vector3.right * radius);
        Vector3Int XnInx = ChunckPos(pos - Vector3.right * radius);

        Vector3Int YpInx = ChunckPos(pos + Vector3.up * radius);
        Vector3Int YnInx = ChunckPos(pos - Vector3.up * radius);

        for (int i = XnInx.x; i <= XpInx.x; i++)
        {
            if (i < 0 || i >= Width)
                continue;
            for (int j = YnInx.y; j <= YpInx.y; j++)
            {
                if (j < 0 || j >= Height)
                    continue;
                for (int k = ZnInx.z; k <= ZpInx.z; k++)
                {
                    if (k < 0 || k >= Depth)
                        continue;
                    Chunks[i, j, k].DrawInSphereLossy(pos, radius, value);
                    //Chunks[i, j, k].DebugDraw = true;
                }
            }
        }
    }

    public void Redraw()
    {
        foreach(var c in Chunks)
        {
            c.Redraw();
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(MinPoint, 0.1f);
        Gizmos.DrawWireSphere(MaxPoint, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(MinPoint + FullSize / 2, FullSize);
    }

#endif

}

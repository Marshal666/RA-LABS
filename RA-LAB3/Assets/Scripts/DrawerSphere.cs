using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerSphere : MonoBehaviour
{

    public float Speed = 5f;
    public float ScaleSpeed = 5f;

    public MarchingCubes cubees;

    // Start is called before the first frame update
    void Start()
    {
        if (!cubees)
            cubees = MarchingCubes.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Upwards"), Input.GetAxisRaw("Vertical")) * Speed;
        transform.position += vel * Time.deltaTime;
        transform.localScale += Vector3.one * -Input.mouseScrollDelta.y * ScaleSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.Mouse0))
        {
            cubees.DrawInSphereLossy(transform.position, transform.localScale.x / 2, cubees.ValueMax);
        } else if(Input.GetKey(KeyCode.Mouse1))
        {
            cubees.DrawInSphereLossy(transform.position, transform.localScale.x / 2, cubees.ValueMin);
        }
        //print("chk pos: " + cubees.ChunckPos(transform.position));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (cubees != null)
        {
            try
            {
                MarchingCubesChunk cc = cubees.ChunkAt(transform.position);
                if (cc != null)
                    Gizmos.DrawWireCube(cc.transform.position + (cc.Size - Vector3Int.one).ToVector3() * cc.SegmentSize / 2, (cc.Size - Vector3Int.one).ToVector3() * cc.SegmentSize);

            }
            catch(Exception) { }
        }
    }

}

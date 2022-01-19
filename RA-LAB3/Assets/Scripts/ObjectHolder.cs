﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{

    public GameObject Object;

    public int Size = 8;

    private HashSet<GameObject> FreeObjects = new HashSet<GameObject>();
    private HashSet<GameObject> TakenObjects = new HashSet<GameObject>();

    private void Awake()
    {
        if(Object == null)
            return;
        for(int i = 0; i < Size; i++)
        {
            GameObject g = Instantiate(Object, transform);
            g.SetActive(false);
            FreeObjects.Add(g);
        }
    }

    private void Start()
    {
        for(int i = 0;i < Size;i++)
        {
            GameObject g = Instantiate(Object, transform);
            g.SetActive(false);
            FreeObjects.Add(g);
        }
    }

    public GameObject GetObject()
    {
        GameObject ret = null;
        if(FreeObjects.Count > 0)
        {
            foreach(GameObject o in FreeObjects)
            {
                ret = o;
                break;
            }
            ret.SetActive(true);
            TakenObjects.Add(ret);
            FreeObjects.Remove(ret);
        }
        else
        {
            ret = Instantiate(Object, transform);
            ret.SetActive(true);
            TakenObjects.Add(ret);
            Size++;
        }
        //print("After Get: Free: {" + string.Join(", ", FreeObjects) + " } Taken : { " + string.Join(", ", TakenObjects) + " }");
        return ret;
    }

    public bool ReturnObject(GameObject g)
    {
        if(TakenObjects.Contains(g))
        {
            TakenObjects.Remove(g);
            FreeObjects.Add(g);
            g.transform.SetParent(transform);
            g.SetActive(false);
            //print("After Return: Free: {" + string.Join(", ", FreeObjects) + " } Taken : { " + string.Join(", ", TakenObjects) + " }");
            return true;
        }
        return false;
    }

}

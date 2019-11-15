using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField]
    int _pointValue;

    string _name;

    public int PointValue{
        get { return _pointValue; }
    }
    // Start is called before the first frame update
    void Start()
    {
        _name = transform.name;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTracker : MonoBehaviour
{
    public Pose pose;

    public static GameObjectTracker Instance;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pose = new Pose(transform.position, transform.rotation);
        
    }
}

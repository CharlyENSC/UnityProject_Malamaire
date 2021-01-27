using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flat : MonoBehaviour
{

    public RobotFreeAnim agent2;
    public int capacity;
    public int robotsActive = 0;
    public int robotsWorking = 0;
    protected Light[] spotLights;
    public float constLight = 1.5f;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name != "Plane")
        {
            Destroy(this.gameObject);
        }
        

    }
    public void Generate()

    {
        if (this.robotsActive < this.capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                RobotFreeAnim a = (RobotFreeAnim)Instantiate(agent2, this.transform.position, Quaternion.identity);
                a.home = this;


                float size = Random.Range(0.4f, 1.6f);
                a.transform.localScale = new Vector3(size, size, size);
                robotsActive++;
            }
        
        }




    }
    void Start()
    {
        spotLights = GetComponentsInChildren<Light>();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (this.robotsWorking < 0)
        {
            this.robotsWorking = 0;
        }
        foreach (var item in spotLights)
        {
            item.intensity = this.constLight * (this.capacity-this.robotsWorking);
        }
    }
}

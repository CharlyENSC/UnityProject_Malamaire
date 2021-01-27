using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work : MonoBehaviour
{
    public RobotFreeAnim agent2;

    public GameObject BaseWork;
    public int robotsInside = 0;
    public List<Flat> workerHomes;
    private float InstantiationTimer = 1.1f;



    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name != "Plane")
        {
            Destroy(this.gameObject);
        }
        

    }

    public void Generate()

    {
        InstantiationTimer -= Time.deltaTime;
        if (this.robotsInside>0  )
        {
            for (int i = 0; i < this.robotsInside; i++)
            {
                if (InstantiationTimer <= 0)
                {
                    RobotFreeAnim a = (RobotFreeAnim)Instantiate(agent2, this.transform.position + transform.forward, Quaternion.identity);
                    a.workplace = this;
                    a.state = "toHome";
                    a.home = this.workerHomes[i];
                    float size = Random.Range(0.4f, 1.6f);
                    a.transform.localScale = new Vector3(size, size, size);
                    this.robotsInside--;
                    this.workerHomes.Remove(this.workerHomes[i]);
                    InstantiationTimer = 1.1f;
                    a.GoToHome();
                }
           



            }
        }




    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

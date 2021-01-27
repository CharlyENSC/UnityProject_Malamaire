using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine;
public class RobotFreeAnim : MonoBehaviour {

    public Work workplace = new Work();
    public Flat home = new Flat();
	Vector3 rot = Vector3.zero;
	float rotSpeed = 40f;
	Animator anim;
    private UnityEngine.AI.NavMeshAgent agent4;
    public GameObject g;
    public string state;
    // public House houseOrg;

    // Use this for initialization
    void Start()
    {
        agent4 = GetComponent<NavMeshAgent>();

        g = GetComponent<GameObject>();
    }
    void Awake()
	{
		anim = gameObject.GetComponent<Animator>();
		gameObject.transform.eulerAngles = rot;
	}

	// Update is called once per frame
	void Update()
	{
        if (this.agent4==null)
        {
        
            this.agent4 = GetComponent<NavMeshAgent>();
            this.g = GetComponent<GameObject>();

        }
      
		gameObject.transform.eulerAngles = rot;

        if (this.state=="toWork")
        {
          
            if (Math.Abs(this.agent4.transform.position.x) <= Math.Abs(workplace.transform.position.x) + 0.6f && Math.Abs(this.agent4.transform.position.x) >= Math.Abs(workplace.transform.position.x) - 0.6f && Math.Abs(this.agent4.transform.position.z) <= Math.Abs(workplace.transform.position.z) + 0.6f && Math.Abs(this.agent4.transform.position.z) >= Math.Abs(workplace.transform.position.z) - 0.6f)
            {
                this.agent4.isStopped = true;
                workplace.robotsInside++;
                workplace.workerHomes.Add(this.home);
                Destroy(this.gameObject);
                this.home.robotsWorking++;
            }
        }
        else if (this.state == "toHome")
        {
       
            this.GoToHome();
            //Debug.Log(this.agent4.transform.position + " " + this.home.transform.position);
            //Debug.Log(Math.Abs(this.agent4.transform.position.x - this.home.transform.position.x));
            if (Math.Abs(this.agent4.transform.position.x - this.home.transform.position.x) < 2 && Math.Abs(this.agent4.transform.position.z - this.home.transform.position.z) <2)
            {
              
                this.agent4.isStopped = true;

                Destroy(this.gameObject);
                home.robotsActive--;
                home.robotsWorking--;
                Debug.Log("working now");
            }
        }





    }
    public void GoToWork()
    {
        if (this.agent4 != null)
        {
            this.state = "toWork";
            this.agent4.SetDestination(workplace.transform.position);
            
        }
       

    }
    public void GoToHome()
    {
       
        if (this.agent4 != null)
        {
   
            this.state = "toHome";
            this.agent4.SetDestination(home.transform.position);
           
        }
     
    }
    void CheckKey()
	{
		// Walk
		if (Input.GetKey(KeyCode.W))
		{
			anim.SetBool("Walk_Anim", true);
		}
		else if (Input.GetKeyUp(KeyCode.W))
		{
			anim.SetBool("Walk_Anim", false);
		}

		// Rotate Left
		if (Input.GetKey(KeyCode.A))
		{
			rot[1] -= rotSpeed * Time.fixedDeltaTime;
		}

		// Rotate Right
		if (Input.GetKey(KeyCode.D))
		{
			rot[1] += rotSpeed * Time.fixedDeltaTime;
		}

		// Roll
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (anim.GetBool("Roll_Anim"))
			{
				anim.SetBool("Roll_Anim", false);
			}
			else
			{
				anim.SetBool("Roll_Anim", true);
			}
		}

		// Close
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			if (!anim.GetBool("Open_Anim"))
			{
				anim.SetBool("Open_Anim", true);
			}
			else
			{
				anim.SetBool("Open_Anim", false);
			}
		}
	}

}

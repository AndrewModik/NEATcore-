using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;


public class CarController : UnitController {

	public float DudThreshold = 0.01f;
    public float Speed = 5f;
    public float TurnSpeed = 180f;
    public int Lap = 1;
    public int CurrentPiece, LastPiece;
    bool MovingForward = true;
    bool IsRunning;
    public float SensorRange = 10;
    public int WallHits; 
	public int AnimalHits;
	public int RoadPieceCount;
	public bool DetctAnimals;
	private bool fail = false;
	private bool fail1 = false;
	private int BackDud = 0;
	Vector2 delt = new Vector2 (0,0);
    IBlackBox box;
	public GameObject Opt;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        //grab the input axes to player
        //var steer = Input.GetAxis("Horizontal");
        //var gas = Input.GetAxis("Vertical");

        // Five sensors: Front, left front, left, right front, right 

        if (IsRunning)												//ALL DOWN - run INPUTS & calc OUTPUTS
        {	
			if (delt != new Vector2 (0, 0))
			if (Mathf.Abs(delt.x - transform.position.x) < DudThreshold && Mathf.Abs(delt.y - transform.position.z) < DudThreshold)
				this.gameObject.active = false;
			delt.x = transform.position.x;
			delt.y = transform.position.z;
			if (BackDud > 3) this.gameObject.active = false;
			if (transform.position.y < -10)
			{
				fail = true;
			}
			if (transform.rotation.x > 10 || transform.rotation.x < -10 || transform.rotation.z > 10 || transform.rotation.z < -10)
			{
				fail1 = true;
			}

            float frontSensor = 0;
            float leftFrontSensor = 0;
            float leftSensor = 0;
            float rightFrontSensor = 0;
            float rightSensor = 0;

			float frontSensor1 = 0;
			float leftFrontSensor1 = 0;
			float leftSensor1 = 0;
			float rightFrontSensor1 = 0;
			float rightSensor1 = 0;
            //5 Sensors
			RaycastHit hit;
			float dsrh = 1.1f; //orig = 1.1f
			Vector3 UPP = new Vector3(0,0.5f,0);
			if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(0, 0, 1).normalized), out hit, SensorRange))
            {
				if (hit.collider.tag.Equals("Wall")|| hit.collider.tag.Equals("Animals"))
                {
                    frontSensor = 1 - hit.distance / SensorRange;
                }
            }

			if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(0.5f, 0, 1).normalized), out hit, SensorRange))
            {
				if (hit.collider.tag.Equals("Wall")|| hit.collider.tag.Equals("Animals"))
                {
                    rightFrontSensor = 1 - hit.distance / SensorRange;
                }
            }

			if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(1, 0, 0).normalized), out hit, SensorRange))
            {
				if (hit.collider.tag.Equals("Wall")|| hit.collider.tag.Equals("Animals"))
                {
                    rightSensor = 1 - hit.distance / SensorRange;
                }
            }

			if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(-0.5f, 0, 1).normalized), out hit, SensorRange))
            {
				if (hit.collider.tag.Equals("Wall")|| hit.collider.tag.Equals("Animals"))
                {
					leftFrontSensor = 1 - hit.distance / SensorRange;
				}										//smart)
            }

			if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(-1, 0, 0).normalized), out hit, SensorRange))
            {
				if (hit.collider.tag.Equals("Wall") || hit.collider.tag.Equals("Animals"))
                {
                    leftSensor = 1 - hit.distance / SensorRange;
                }
            }
			if (DetctAnimals) {
				
				//5 Sensors
				if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(0, 0, 1).normalized), out hit, SensorRange))
				{
					if (hit.collider.tag.Equals("Animals"))
					{
						frontSensor1 = 1 - hit.distance / SensorRange;
					}
				}

				if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(0.5f, 0, 1).normalized), out hit, SensorRange))
				{
					if (hit.collider.tag.Equals("Animals"))
					{
						rightFrontSensor1 = 1 - hit.distance / SensorRange;
					}
				}

				if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(1, 0, 1).normalized), out hit, SensorRange))
				{
					if (hit.collider.tag.Equals("Animals"))
					{
						rightSensor1 = 1 - hit.distance / SensorRange;
					}
				}

				if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(-0.5f, 0, 1).normalized), out hit, SensorRange))
				{
					if (hit.collider.tag.Equals("Animals"))
					{
						leftFrontSensor1 = 1 - hit.distance / SensorRange;
					}									
				}

				if (Physics.Raycast(transform.position + UPP + transform.forward * dsrh, transform.TransformDirection(new Vector3(-1, 0, 1).normalized), out hit, SensorRange))
				{
					if (hit.collider.tag.Equals("Animals"))
					{
						leftSensor1 = 1 - hit.distance / SensorRange;
					}
				}
			}
			//Creating INPUTS vector
            ISignalArray inputArr = box.InputSignalArray;
            inputArr[0] = frontSensor;
            inputArr[1] = leftFrontSensor;
            inputArr[2] = leftSensor;
            inputArr[3] = rightFrontSensor;
            inputArr[4] = rightSensor;
			if (DetctAnimals) {
				inputArr [5] = frontSensor1;
				inputArr [6] = leftFrontSensor1;
				inputArr [7] = leftSensor1;
				inputArr [8] = rightFrontSensor1;
				inputArr [9] = rightSensor1;
			}
			//Submit INPUTS
            box.Activate();
			//Recieve OUTPUTS
            ISignalArray outputArr = box.OutputSignalArray;
			//
            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);
        }
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }
	//Run NeuralNetwork
    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
    }

    public void NewLap()
    {        
        if (LastPiece > 2 && MovingForward)
        {
            Lap++;   
			if (!(transform.position.y < -2 ))
			{
				fail = false;
			}
			if (!(transform.rotation.x > 20 && transform.rotation.x < -20 && transform.rotation.z > 20 && transform.rotation.z < -20))
			{
				fail1 = false;
			}
        }
    }
	//Fitness Function
    public override float GetFitness()
    {
        if (Lap == 1 && CurrentPiece == 0)
        {
            return 0;
        }
        int piece = CurrentPiece;
        if (CurrentPiece == 0)
        {
			piece = RoadPieceCount;
        }
			
		int Gener = Opt.GetComponent<GoalPiece>().GetGener();
		float fit = (Lap - 1) * RoadPieceCount + piece;
		fit *= 4;
		if (Lap > 1)
			fit *= 2f;
		fit -= WallHits * (0.5f + fit * 0.006f + Gener * 0.1f) + AnimalHits * (1 + fit * 0.15f + Gener * 0.015f); //FITNESS FUNCTION
	
		//originaly FF was const with Fee number here * WallHits
		 

        //print(string.Format("Piece: {0}, Lap: {1}, Fitness: {2}", piece, Lap, fit));

		if (fit > 0 && fail == false)
        {
			if (fail1) 
			{
				return fit * 0.5f;
			} 
			else
			{
				return fit;
			}
        }
        return 0;
    }

	//Collision catcher
    void OnCollisionEnter(Collision collision)
    {
		if (collision.collider.tag.Equals ("Road"))
		{
			RoadPiece rp = collision.collider.GetComponent<RoadPiece> ();
            
			if ((rp.PieceNumber != LastPiece) && (rp.PieceNumber == CurrentPiece + 1 || (MovingForward && rp.PieceNumber == 0))) {
				LastPiece = CurrentPiece;
				CurrentPiece = rp.PieceNumber;
				MovingForward = true;                
			} else {
				MovingForward = false;
				BackDud++;
			}
			if (rp.PieceNumber == 0) {
				CurrentPiece = 0;
			}
		} 
		if  (collision.collider.tag.Equals ("Animals")) 
				AnimalHits++;

		if (collision.collider.tag.Equals ("Wall")) 
				WallHits++;

    }


	/*
    void OnGUI()
    {
       GUI.Button(new Rect(650, 200, 50, 40), "Lap: " + Lap);
    }
    */
}

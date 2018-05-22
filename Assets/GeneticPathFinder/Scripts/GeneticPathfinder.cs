 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    int pathIndex = 0;
    public DNA dna;
    public bool hasFinished = false;
    bool hasBeenInitialized = false;
    Vector3 target;
    Vector3 nextPoint;

	// NEW_Individual`s creation
    public void InitCreature(DNA newDna, Vector3 _target)
    {
        dna = newDna;
        target = _target;
        nextPoint = transform.position;
        hasBeenInitialized = true;
    }
	//Actually execution
    private void Update()
    {
        if (hasBeenInitialized && !hasFinished)
        {
			//Check for Finish 
            if(pathIndex == dna.genes.Count)
            {
                hasFinished = true;
            }
			//Steps
            if((Vector3)transform.position == nextPoint)
            {
				nextPoint = transform.position + new Vector3(dna.genes[pathIndex].x, 0, dna.genes[pathIndex].y);
                pathIndex++;
            }
			//Movement 
			else 
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
        }
    }
	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag.Equals ("Finish")) 
			hasFinished = true; 
		 
		if (collider.tag.Equals ("Wall")) 
			hasFinished = true;

	}
	//Evaluating
    public float fitness
    {
        get
        {
            float dist = Vector3.Distance(transform.position, target);
            if(dist == 0)
            {
                dist = 0.0001f;
            }
            return 60/dist;
        }
    }
		
}

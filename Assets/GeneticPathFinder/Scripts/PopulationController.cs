using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    List<GeneticPathfinder> population = new List<GeneticPathfinder>();
    public GameObject creaturePrefab; //link to "Individ"
    public int populationSize = 100;
    public int genomeLenght;
    public float cutoff = 0.3f;
    public Transform spawnPoint; //Link to "SpawnPoint"
    public Transform end; //Link to "Target"

	   
	//Creation of the Populations of Individuals(their objects and genoms)
	//Cloning "Individ"
    void InitPopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(genomeLenght), end.position);
            population.Add(go.GetComponent<GeneticPathfinder>());
        }
    }
	//Create NEW Generation
    void NextGeneration()
    {
		//Selecting to NEW GENERATION
        int survivorCut = Mathf.RoundToInt(populationSize * cutoff);
        List<GeneticPathfinder> survivors = new List<GeneticPathfinder>();
        for(int i = 0; i < survivorCut; i++)
        {
            survivors.Add(GetFittest());
        }
		// delete the rest
        for(int i = 0; i < population.Count; i++) 
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();
		//Creating offsprings - MATING
        while(population.Count < populationSize)
        {
            for(int i = 0; i < survivors.Count; i++)
            {
				GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity); //Actually down here Mating is (only TOP10 best genomes can mating)
				go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(survivors[i].dna, survivors[Random.Range(0, 10)].dna), end.position);//Actually here Mating is (only TOP10)
                population.Add(go.GetComponent<GeneticPathfinder>());
                if(population.Count >= populationSize)
                {
                    break;
                }
            }
        }
        for(int i = 0; i < survivors.Count; i++)
        {
            Destroy(survivors[i].gameObject);
        }
    }
	//It`s a stsrt, obviously
    private void Start()
    {
        InitPopulation();
    }
	//Switch to new generation
    private void Update()
    {
        if (!HasActive())
        {
            NextGeneration();
        }
		Time.timeScale = 1;
    }
	//Find, return and delete from old generation the fittest genome
    GeneticPathfinder GetFittest()
    {
        float maxFitness = float.MinValue;
        int index = 0;
        for(int i = 0; i < population.Count; i++)
        {
            if(population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
                index = i;
            }
        }
        GeneticPathfinder fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }
	//Check for ANY ALIVE Cretins
    bool HasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerBehavior : MonoBehaviour
{
    public GameObject food;
    public GameObject entity;

    // Adjustable variables
    public float gameSpeed = 1f;
    public int generations = 4;

    // Genetic Alg Constants
    private readonly static int MAX_ENTITIES = 20;
    private readonly static int MAX_FOOD = 200;
    private readonly static int GENETIC_TOP_ENTITIES = 5; // How many entities left as winners

    // Local use variables
    private EntityBehavior[] entities = new EntityBehavior[MAX_ENTITIES];
    private int currentGen = 0;
    private bool isGenRunning = false;
    private float[][] crossbreedGenes = new float[GENETIC_TOP_ENTITIES][];

    // Creating one instance of this object
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void FixedUpdate()
    {
        // The speed at which the game is played.  1f is realtime.
        Time.timeScale = gameSpeed;

        // Each update add food if not full
        CreateFood();

        // Collect data on surviving entities if the end condition has been met
        GameObject[] survivingEntities = GameObject.FindGameObjectsWithTag("Entity");
        if (survivingEntities.Length <= GENETIC_TOP_ENTITIES)
        {
            for (int i = 0; i < survivingEntities.Length; i++)
            {
                crossbreedGenes[i] = survivingEntities[i].GetComponent<EntityBehavior>().GetGenes();
            }

            // Last generation special case
            // TODO
            isGenRunning = false;
        }

        // Start a new generation if ended and still generations left
        if (currentGen < generations && isGenRunning == false)
        {
            // Remove the winners of that round
            foreach (GameObject e in survivingEntities)
            {
                if (e != null)
                {
                    Destroy(e);
                }
            }

            // Start with spawning everyone in
            CreateEntities(MAX_ENTITIES, true);

            isGenRunning = true;
            currentGen++;
        }
        else if (currentGen >= generations && isGenRunning == false) //This was the last generation
        {
            // Find the top winner
            // TODO

            // Report winner's genes
            // TODO
        }

    }

    private void CreateEntities(int cap, bool random)
    {
        int i = 0;
        while (i < cap)
        {
            float rx = Random.Range(-100f, 100f);
            float ry = Random.Range(-50f, 50f);

            // Currently not checking if too close to another entity

            GameObject o = Instantiate(entity, new Vector3(rx, ry, 1), Quaternion.identity);

            if (random) o.GetComponent<EntityBehavior>().RandomGenes();

            i++;
        }
    }

    private void CreateFood()
    {
        GameObject[] f = GameObject.FindGameObjectsWithTag("Food");

        if (f.Length < MAX_FOOD)
        {
            float rx = Random.Range(-100f, 100f);
            float ry = Random.Range(-50f, 50f);
            
            Instantiate(food, new Vector3(rx, ry, 1), Quaternion.identity);
        }
    }
}

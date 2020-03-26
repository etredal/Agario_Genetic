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
    private readonly static int GENES = 3;
    private readonly static int MAX_ENTITIES = 20;
    private readonly static int MAX_FOOD = 200;
    private readonly static int GENETIC_TOP_ENTITIES = 5; // How many entities left as winners
    private readonly static int CROSSOVER_CHANCE = 6; // (1 / CROSSOVER_CHANCE - 1) is the probability
    private readonly static float MUTATION_AMOUNT = 0.05f;

    // Local use variables
    private EntityBehavior[] entities = new EntityBehavior[MAX_ENTITIES];
    private int currentGen = 0;
    private bool isGenRunning = false;
    private float[][] crossbreedGenes = new float[GENETIC_TOP_ENTITIES][];

    // Creating one instance of this object SINGLETON
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
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Generation"))
        {
            Generation();
        }
    }

    private void Generation()
    {
        // The speed at which the game is played.  1f is realtime.
        Time.timeScale = gameSpeed;

        // Each update add food if not full
        CreateFood();

        // Collect data on surviving entities if the end condition has been met
        GameObject[] survivingEntities = GameObject.FindGameObjectsWithTag("Entity");
        if (survivingEntities.Length <= GENETIC_TOP_ENTITIES && isGenRunning == true) // isGenRunning to make sure doesn't occur before first run
        {
            for (int i = 0; i < survivingEntities.Length; i++)
            {
                float[] genes = new float[GENES];
                for (int j = 0; j < GENES; j++)
                {
                    genes[j] = survivingEntities[i].GetComponent<EntityBehavior>().GetGene(j);
                }
                crossbreedGenes[i] = genes;
            }

            // Last generation special case
            if (currentGen < generations)
            {
                isGenRunning = false;
            }
            else
            {
                if (survivingEntities.Length <= 1)
                {
                    isGenRunning = false;
                }
            }
        }

        // Start a new generation if ended and still generations left
        if (currentGen < generations && isGenRunning == false)
        {
            // Remove the winners of that round
            DestroyEntities(survivingEntities);

            // Start with spawning everyone in
            if (currentGen == 0)
            {
                CreateEntities(MAX_ENTITIES);
            }
            else
            {
                CreateEntities(MAX_ENTITIES, crossbreedGenes);
            }

            // Control
            isGenRunning = true;
            currentGen++;
        }
        else if (currentGen >= generations && isGenRunning == false) //This was the last generation
        {
            // Remove the winners of that round
            DestroyEntities(survivingEntities);

            // Report winner's genes
            Debug.Log(crossbreedGenes[0][0] + " " + crossbreedGenes[0][1] + " " + crossbreedGenes[0][2]);

            // Return to main menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void DestroyEntities(GameObject[] entityList)
    {
        foreach (GameObject e in entityList)
        {
            if (e != null)
            {
                Destroy(e);
            }
        }
    }

    private void CreateEntities(int cap)
    {
        int i = 0;
        while (i < cap)
        {
            float rx = Random.Range(-100f, 100f);
            float ry = Random.Range(-50f, 50f);

            GameObject o = Instantiate(entity, new Vector3(rx, ry, 1), Quaternion.identity);

            o.GetComponent<EntityBehavior>().RandomGenes();

            i++;
        }
    }

    private void CreateEntities(int cap, float[][] crossbreedGenes)
    {
        float[,] selectedGenes = new float[MAX_ENTITIES, GENES];
        Breed(crossbreedGenes, selectedGenes); // Will change the values of selectedGenes

        int i = 0;
        while (i < cap)
        {
            float rx = Random.Range(-100f, 100f);
            float ry = Random.Range(-50f, 50f);

            GameObject o = Instantiate(entity, new Vector3(rx, ry, 1), Quaternion.identity);
            
            for (int j = 0; j < GENES; j++)
            {
                o.GetComponent<EntityBehavior>().SetGene(j, selectedGenes[i,j]);
            }

            i++;
        }
    }

    private void Breed(float[][] crossbreedGenes, float[,] selectedGenes)
    {
        for (int i = 0; i < MAX_ENTITIES; i++)
        {
            // Get parent's genes
            int randomParent = Random.Range(0, GENETIC_TOP_ENTITIES);
            for (int j = 0; j < GENES; j++)
            {
                selectedGenes[i,j] = crossbreedGenes[randomParent][j];
            }

            // Random chance of crossover event occuring 1 out of 5, since range isn't inclusive
            if (Random.Range(0, CROSSOVER_CHANCE) == 0)
            {
                int crossoverCut = Random.Range(1, 2); // How many other genes are added: 1 or 2 genes could be swapped
                for (int j = 0; j < GENES - crossoverCut; j++)
                {
                    selectedGenes[i, j] = crossbreedGenes[randomParent][j];
                }
            }

            // Mutate genes
            for (int j = 0; j < GENES; j++)
            {
                selectedGenes[i,j] += Random.Range(-MUTATION_AMOUNT, MUTATION_AMOUNT);

                // Keep in range
                selectedGenes[i, j] = Mathf.Max(selectedGenes[i, j], 0f);
                selectedGenes[i, j] = Mathf.Min(selectedGenes[i, j], 1f);
            }
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

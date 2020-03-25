using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerBehavior : MonoBehaviour
{
    public float gameSpeed = 1f;

    public GameObject food;
    public GameObject entity;

    private readonly static int MAX_ENTITIES = 20;
    private readonly static int MAX_FOOD = 200;
    private EntityBehavior[] entities = new EntityBehavior[MAX_ENTITIES];

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

    private void Start()
    {
        // Start with spawning everyone in
        CreateEntities(MAX_ENTITIES, true);
    }

    private void FixedUpdate()
    {
        // The speed at which the game is played.  1f is realtime.
        Time.timeScale = gameSpeed;

        // Each update add food if not full
        CreateFood();
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

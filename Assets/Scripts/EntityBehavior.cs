using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    // Attributes
    public float foodGene = 1.0f;
    public float smallerEntityGene = 1.0f;
    public float largerEntityGene = 1.0f;

    public int size = 5;
    public bool setRandomColor = true;
    public float scaleFactor = 10f;

    private float speed = 0f;
    private float baseSpeed = 0.08f;
    private float speedDecreaseFactor = 10000f; // Higher is slower

    private int loss = 0;
    private int lossTime = 600;

    public void FixedUpdate()
    {
        loss++;
        if (loss > lossTime)
        {
            loss = 0;
            size--;
        }

        if (size <= 0)
        {
            Destroy(gameObject);
        }

        Vector3 target = new Vector3(0f,0f);

        // Food target update
        GameObject fd = ClosestFood();
        if (fd != null)
        {
            //Modeling gravitational force
            Vector3 grav = new Vector3();

            grav.x += fd.transform.position.x - transform.position.x;
            grav.y += fd.transform.position.y - transform.position.y;

            grav.Normalize();

            grav *= foodGene * 1.0f/Mathf.Pow(Vector3.Distance(new Vector3(fd.transform.position.x, fd.transform.position.y), new Vector3(transform.position.x, transform.position.y)), 2);
            target += grav;
        }

        // Smaller Entity target update
        GameObject sm = ClosestSmaller();
        if (sm != null)
        {
            //Modeling gravitational force
            Vector3 grav = new Vector3();

            grav.x += sm.transform.position.x - transform.position.x;
            grav.y += sm.transform.position.y - transform.position.y;

            grav.Normalize();

            grav *= smallerEntityGene * 1.0f / Mathf.Pow(Vector3.Distance(new Vector3(sm.transform.position.x, sm.transform.position.y), new Vector3(transform.position.x, transform.position.y)), 2);
            target += grav;
        }

        // Larger Entity target update
        GameObject lg = ClosestLarger();
        if (lg != null)
        {
            //Modeling gravitational force
            Vector3 grav = new Vector3();

            // Opposite since you want to run away from a larger cell
            grav.x += transform.position.x - lg.transform.position.x;
            grav.y += transform.position.y - lg.transform.position.y;

            grav.Normalize();

            grav *= largerEntityGene * 1.0f / Mathf.Pow(Vector3.Distance(new Vector3(lg.transform.position.x, lg.transform.position.y), new Vector3(transform.position.x, transform.position.y)), 2);
            target += grav;
        }

        // Apply target position movement
        // We normalize this because we want to always have a fast speed
        target = target.normalized * speed;
        transform.position += target;

        // Adjust Scaling
        transform.localScale = new Vector3(1 + (size - 5)/scaleFactor, 1 + (size - 5)/scaleFactor);

        // Adjust Speed
        speed = Mathf.Max(0.01f, baseSpeed - size / speedDecreaseFactor);
    }

    private void Start()
    {
        if (setRandomColor) GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
    }

    public GameObject ClosestFood()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Food");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public GameObject ClosestSmaller()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Entity");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (CanEat(go) && curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public GameObject ClosestLarger()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Entity");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (CanBeEaten(go) && curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (collision.gameObject.GetComponent<FoodBehavior>().active == true)
            {
                size++; // Only get to increase size if it DID NOT just spawn into the game
            }
            
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.transform.parent.gameObject.CompareTag("Entity"))
        {
            if (CanEat(collision.gameObject.transform.parent.gameObject))
            {
                size += collision.gameObject.transform.parent.gameObject.GetComponent<EntityBehavior>().size;
                Destroy(collision.gameObject.transform.parent.gameObject);
            }
        }
    }

    public bool CanEat(GameObject obj)
    {
        if (obj.CompareTag("Entity"))
        {
            if (obj.GetComponent<EntityBehavior>().size < size - 10)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanBeEaten(GameObject obj)
    {
        if (obj.CompareTag("Entity"))
        {
            if (obj.GetComponent<EntityBehavior>().size - 10 > size)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2 GetCoordinates()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void RandomGenes()
    {
        foodGene = Random.Range(0f,1f);
        smallerEntityGene = Random.Range(0f,1f);
        largerEntityGene = Random.Range(0f,1f);
    }
}

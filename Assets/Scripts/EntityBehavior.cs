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

    public void FixedUpdate()
    {
        float targetX = 0;
        float targetY = 0;

        // Food target update
        GameObject fd = ClosestFood();
        if (fd != null)
        {
            targetX += foodGene * (fd.transform.position.x - transform.position.x);
            targetY += foodGene * (fd.transform.position.y - transform.position.y);
        }

        // Smaller Entity target update
        GameObject sm = ClosestSmaller();
        if (sm != null)
        {
            targetX += smallerEntityGene * (sm.transform.position.x - transform.position.x);
            targetY += smallerEntityGene * (sm.transform.position.y - transform.position.y);
        }

        // Larger Entity target update *Moves in opposite direction
        GameObject lg = ClosestLarger();
        if (lg != null)
        {
            targetX -= largerEntityGene * (lg.transform.position.x - transform.position.x);
            targetY -= largerEntityGene * (lg.transform.position.y - transform.position.y);
        }

        // Apply target position movement
        Vector3 move = new Vector3(targetX, targetY);
        move = move.normalized * speed;
        transform.position += move;

        // Adjust Scaling
        transform.localScale = new Vector3(1 + (size - 5)/scaleFactor, 1 + (size - 5)/scaleFactor);

        // Adjust Speed
        speed = baseSpeed - size / 800.0f;
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            size++;
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
            if (obj.GetComponent<EntityBehavior>().size < size - 20)
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
            if (obj.GetComponent<EntityBehavior>().size - 20 > size)
            {
                return true;
            }
        }
        return false;
    }
}

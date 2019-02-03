using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayerState>
{
    public static float maxPickUpDistance = 1;

    private Rigidbody rb;
    private float speed = 10f;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);

    private GameObject currentlyHighlightedInteractiveObject = null;

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);

        //mouse loook
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (aimPlane.Raycast(ray, out distance)) {
            Vector3 target = ray.GetPoint(distance);
            target.y = gameObject.transform.position.y;
            gameObject.transform.LookAt(target);
        }
    }

    void Update()
    {
        UpdateNearestInteractiveObjectHighlight();

        // TODO: Replace with Rewire input
        if (Input.GetKeyDown("z"))
        {
            bool chestOpened = OpenNearestChest();
            if (!chestOpened)
            {
                PickUpNearestItem();
            }
        }
        // TODO: Replace with Rewire input
        if (Input.GetKeyDown("x"))
        {
            ItemManager.Instance.SpawnItem(new Vector3(Random.Range(-1.0f, 1.0f), 0.2f, Random.Range(-1.0f, 1.0f)));
        }
    }

    private bool OpenNearestChest()
    {
        GameObject closestChest = GetClosestChestInReach();
        if (closestChest)
        {
            return closestChest.GetComponent<Chest>().TryOpen();
        }
        return false;
    }

    private bool PickUpNearestItem()
    {
        GameObject closestItem = GetClosestItemInReach();
        if (closestItem)
        {
            return closestItem.GetComponent<Item>().TryPickUp(); ;
        }
        return false;
    }

    private void UpdateNearestInteractiveObjectHighlight()
    {
        if (currentlyHighlightedInteractiveObject)
        {
            currentlyHighlightedInteractiveObject.GetComponent<InteractiveObject>().RemoveHighlight();
        }

        currentlyHighlightedInteractiveObject = GetClosestChestOrItemInReach();

        if (currentlyHighlightedInteractiveObject)
        {
            Chest chest = currentlyHighlightedInteractiveObject.GetComponent<Chest>();
            if (!chest || !chest.IsOpen)
            {
                currentlyHighlightedInteractiveObject.GetComponent<InteractiveObject>().AddHighlight();
            }
        }
    }

    private GameObject GetClosestChestOrItemInReach()
    {
        GameObject closestChest = GetClosestChestInReach();
        GameObject closestItem = GetClosestItemInReach();

        if (!closestItem)
        {
            return closestChest;
        }
        else if (!closestChest)
        {
            return closestItem;
        } 
        else if (!closestChest.GetComponent<Chest>().IsOpen && GetTopDownDistanceBetweenObjects(closestChest, gameObject) < GetTopDownDistanceBetweenObjects(closestItem, gameObject))
        {
            return closestChest;
        }
        else
        {
            return closestItem;
        }
    }

    private GameObject GetClosestChestInReach()
    {
        return GetClosestObjectWithTagInReach("Chest", maxPickUpDistance);
    }

    private GameObject GetClosestItemInReach()
    {
        return GetClosestObjectWithTagInReach("Item", maxPickUpDistance);
    }

    private GameObject GetClosestObjectWithTagInReach(string tag, float maxDistance)
    {
        GameObject closestObject = GetClosestObjectWithTagInReach(tag);

        if (closestObject && GetTopDownDistanceBetweenObjects(gameObject, closestObject) <= maxDistance)
        {
            return closestObject;
        } else
        {
            return null;
        }
    }

    private GameObject GetClosestObjectWithTagInReach(string tag)
    {
        float minDist = float.MaxValue;
        GameObject closestObject = null;

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject taggedObject in taggedObjects)
        {
            float itemDist = GetTopDownDistanceBetweenObjects(gameObject, taggedObject);
            if (itemDist < minDist)
            {
                minDist = itemDist;
                closestObject = taggedObject;
            }
        }

        return closestObject;
    }

    private float GetTopDownDistanceBetweenObjects(GameObject obj1, GameObject obj2)
    {
        return Vector2.Distance(new Vector2(obj1.transform.position.x, obj1.transform.position.z),
                new Vector2(obj2.transform.position.x, obj2.transform.position.z));
    }
}

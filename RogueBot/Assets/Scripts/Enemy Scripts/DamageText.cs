using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Transform enemy;
    Vector3 offset = new Vector3(0, 2f, 0); // To place text above enemy

    private void Start()
    {
        // Gets the transform of the enemy that this gameobject is assigned to
        // .
        enemy = transform.parent.transform.parent.transform;

        // Put the text right above the enemy
        // .
        transform.parent.transform.localPosition += offset;
    }

    private void Update()
    {
        // Assigns enemy transform to text transform so that the local scale
        // of the text is consistent with the enemy transform, as when enemy turns,
        // the localScale.x is multiplied with -1. Thus it would
        // flip the text when the enemy is also flipped and prevents a reverse text
        // .
        Vector3 damageTextScale = enemy.localScale;
        transform.parent.transform.localScale = damageTextScale;
    }
    public void DestroyDamageText()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
    }

}

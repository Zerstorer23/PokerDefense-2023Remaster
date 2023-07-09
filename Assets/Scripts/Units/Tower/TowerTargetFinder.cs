using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargetFinder : MonoBehaviour
{
    //Cache
    [SerializeField] Tower mainTower;
    [SerializeField] Tower_FirePower firepowerManager;
    [SerializeField] SpriteRenderer rangeMarkerSprite;


     float multiFactor = 2f;

    private void Update()
    {
       bool targetLost =  IsTargetValid();
        if (targetLost)
        {
            firepowerManager.SetFocusedTarget(null);
          //  myCollider.enabled = true;
            GetEnemyNear(mainTower.GetFinalAttackRange());
        }
    }


    private bool IsTargetValid()
    {
        GameObject target = firepowerManager.GetFocusedTarget();
        if (!target || !target.activeSelf || !target.activeInHierarchy)
        {
            return true;
        }
        HealthPoint health = target.GetComponentInChildren<HealthPoint>();
        //      if(target)
        //        Debug.Log("target at " + target.transform.position + " active " + target.activeSelf);
        if (mainTower.owner == Owner.KUROI)
        {
            if (health.GetHP() == health.GetFullHP())
            {
                return true;
            }
        }
        else {
            if (health.GetHP() <= 0)
            {
                return true;
            }
        }

        double dist = Vector2.Distance(transform.position, target.transform.position);
        if (dist > mainTower.attackDistance)
        {
            return true;
        }
        return false;
    }
    private void GetEnemyNear_Raycast(float range)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask(ConstantStrings.STRING_LAYER_UNITS));
        // List<GameObject> targets = new List<GameObject>();
        foreach (Collider2D c in hitColliders)
        {
            GameObject target = c.gameObject;
            try
            {
                if (target.CompareTag(ConstantStrings.TAG_ENEMY))
                {
                    HealthPoint enemy_health = target.GetComponentInChildren<HealthPoint>();
                    if (mainTower.owner == Owner.KUROI)
                    {
                        if (enemy_health.GetHP() < enemy_health.GetFullHP())
                        {
                            firepowerManager.SetFocusedTarget(target);
                            return;

                        }
                    }
                    else if (enemy_health.GetHP() > 0)
                    {
                        firepowerManager.SetFocusedTarget(target);
                        return;
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
    private void GetEnemyNear(float range)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask(ConstantStrings.STRING_LAYER_UNITS));
       // List<GameObject> targets = new List<GameObject>();
        foreach (Collider2D c in hitColliders)
        {
            GameObject target = c.gameObject;
            try
            {
                if (target.CompareTag(ConstantStrings.TAG_ENEMY))
                {
                    HealthPoint enemy_health = target.GetComponentInChildren<HealthPoint>();
                    if (mainTower.owner == Owner.KUROI)
                    {
                        if (enemy_health.GetHP() < enemy_health.GetFullHP())
                        {
                            firepowerManager.SetFocusedTarget(target);
                            return;

                        }
                    }
                    else if (enemy_health.GetHP() > 0)
                    {
                        firepowerManager.SetFocusedTarget(target);
                        return;
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
  /*  private void OnTriggerStay2D(Collider2D other)
    {
        //There is target already. Break out.
        if (firepowerManager.GetFocusedTarget()) {

          //  Debug.Log(GameSession.GetDistance(transform.position, mainTower.GetFocusedTarget().transform.position));
            return;
        }


        if (CollisionIsEnemy(other))
        {
       //     Debug.Log("Detected colliusion " + other.name);
            HealthPoint enemy_health = other.GetComponentInChildren<HealthPoint>();
            if (mainTower.owner == Owner.KUROI) {
                if (enemy_health.GetHP() < enemy_health.GetFullHP())
                {
                    firepowerManager.SetFocusedTarget(other.gameObject);
                    myCollider.enabled = false;
                 //    Debug.Log("Set Target to " + other.transform.position);

                }
            }
            else if (enemy_health.GetHP() > 0)
            {
                firepowerManager.SetFocusedTarget(other.gameObject);
                myCollider.enabled = false;
            //  Debug.Log("Set Target to " + other.transform.position);

            }

        }

    }*/

/*    private bool CollisionIsEnemy(Collider2D collision)
    {
        return collision.gameObject.tag.Equals(ConstantStrings.TAG_ENEMY);
    }*/


    internal void SetAttackDistance(float attackDistance)
    {
      // GetComponent<CircleCollider2D>().radius = attackDistance;
       rangeMarkerSprite.transform.localScale = new Vector2(attackDistance * multiFactor, attackDistance * multiFactor);

    }
    internal void SetRangeVisibility(bool visible) {
        rangeMarkerSprite.enabled = visible;
    }
}

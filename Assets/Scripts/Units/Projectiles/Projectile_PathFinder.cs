using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_PathFinder : MonoBehaviour
{
    //Data
    Projectile mainProjectile;
    internal bool isLandTarget = false;
   internal Transform sourceOfFire;

    internal float moveSpeed = 1f;
    internal Vector3 lastPosition;
    internal Vector3 moveVector;
    bool stopMove = false;

    internal ProjectileMoveType moveType = ProjectileMoveType.NORMAL;
    private void Awake()
    {
    //    Debug.Log("projinit srtart");
        mainProjectile = GetComponent<Projectile>();
    //    Debug.Log("projinit end");
    }
    void Update()
    {
        switch (moveType)
        {
            case ProjectileMoveType.INSTA:
                Process_Move_Insta();
                break;
            case ProjectileMoveType.INDIRECT:
                //   Debug.Log("Heading " + targetLocation);
                Process_Move_Normal();
                break;
            case ProjectileMoveType.DIRECTED_VECTOR:
                Process_Move_DirectedVector();
                break;
            case ProjectileMoveType.BEAM:
                Process_Move_Beam();
                break;
            default:
                Process_Move_Normal();
                break;

        }


    }

    private void Process_Move_Insta()
    {
        if (mainProjectile.targetObject || isLandTarget)
        {
            lastPosition = mainProjectile.GetTargetLocation();
            transform.position = lastPosition;
            mainProjectile.DealDamage(true);
        }
    }

    private void Process_Move_Normal()
    {
        if (mainProjectile.targetObject || isLandTarget)
        {
            lastPosition = mainProjectile.GetTargetLocation();
        }

        MoveTowards(lastPosition);

        if (transform.position == lastPosition)
        {
            mainProjectile.DealDamage(true);
        }
    }

    internal void SetMoveType(ProjectileMoveType _moveType)
    {
        moveType = _moveType;

        if (moveType == ProjectileMoveType.INSTA)
        {
            mainProjectile.spriteRenderer.enabled = false;
        }
    }

    private void MoveTowards(Vector2 heading)
    {
        var target = Vector2.MoveTowards(transform.position, heading, moveSpeed * Time.deltaTime);

        transform.position = target;
    }


    public Vector3 AngleToTarget()
    {
        Vector3 diff = lastPosition - sourceOfFire.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        return new Vector3(0f, 0f, rot_z - 90);
    }


    private void Process_Move_Beam()
    {
        if (!stopMove)
        {
            if (mainProjectile.targetObject && mainProjectile.tower != null)
            {
                double length = Vector2.Distance(mainProjectile.tower.GetGunObject().transform.position, mainProjectile.targetObject.transform.position);
                transform.localScale = new Vector3(1, (float)length);
                Vector3 midPoint = GetMidPoint(mainProjectile.tower.GetGunObject().transform, mainProjectile.targetObject.transform);
                transform.position = mainProjectile.tower.GetGunObject().transform.position + midPoint;
                stopMove = true;
                mainProjectile.InstantiateExplosionAt(mainProjectile.targetObject.transform, false);
                StartCoroutine(WaitAndDamage(0.5f));
            }
            else
            {
                mainProjectile.DestroyMyself();
            }
        }
    }
    private IEnumerator WaitAndDamage(float v)
    {
        mainProjectile.DealDamage(false);
        yield return new WaitForSeconds(v);
        mainProjectile.DestroyMyself();
        stopMove = false;
    }

    private Vector3 GetMidPoint(Transform start, Transform end)
    {
        float midX = (end.position.x - start.position.x) / 2;
        float midY = (end.position.y - start.position.y - 0.1f) / 2;
        return new Vector3(midX, midY);

    }

    private void Process_Move_DirectedVector()
    {
        lastPosition = transform.position + moveVector;
        MoveTowards(lastPosition);
    }






}

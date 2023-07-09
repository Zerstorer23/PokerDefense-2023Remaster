using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Azusa_SkillUnit : MonoBehaviour
{
    float effectTime;
    const int blockCountBase = 16;
   public int blockCountMod = 0;
    float range = 4f;
    public SpriteRenderer rangeSprite;
    string objTag;
    public GameObject explosionObj;
    Coroutine azusaRoutine;
    public ProjectileConfig myConfig;
 [SerializeField]  TextMeshProUGUI countText;

    public void SetInformation(string tag, int _count, float _time) {
        rangeSprite.transform.localScale = new Vector2(range * 2f, range * 2f);
        blockCountMod = blockCountBase + _count;
        countText.text = blockCountMod.ToString();
        effectTime = _time;
        objTag = tag;
        azusaRoutine= StartCoroutine(WaitAndDestroy(effectTime));
    }
    IEnumerator WaitAndDestroy(float delay) {
        yield return new WaitForSeconds(delay);
        DestroyMyself();
    }

    private void DestroyMyself()
    {
        if (azusaRoutine != null)
        {
            StopCoroutine(azusaRoutine);
        }
        ObjectPool.SaveObject(objTag, gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        CheckCollision(collision.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        CheckCollision(collision.gameObject);
    }
    private void CheckCollision(GameObject obj) {
        Projectile proj = obj.GetComponent<Projectile>();
       // Debug.Log("Detected collision");
        if (proj == null) return;
       // Debug.Log("=> proj collision");
        if (proj.owner == Owner.KUROI)
        {
         //   Debug.Log("==> kuroi collision");
            InstantiateExplosionAt(proj.transform);
            proj.DestroyMyself();
            blockCountMod--;
            countText.text = blockCountMod.ToString();
            if (blockCountMod <= 0)
            {
                DestroyMyself();
            }
        }
    }

    public void InstantiateExplosionAt(Transform myTransform)
    {
      
        string explosionTag = objTag + "_Explosion";
        GameObject explosion = ObjectPool.PollObject(explosionTag, myTransform.position, Quaternion.identity);
        if (explosion == null)
        {
            explosion = Instantiate(explosionObj, myTransform.position, Quaternion.identity);
            explosion.transform.SetParent(GameSession.GetGameSession().ExplosionHome);
        }
      //  Debug.Log("Explosion at " + explosion.transform.position);
        Explosion exp = explosion.GetComponent<Explosion>();

        exp.SetInformation(myConfig);
        exp.SetTag(explosionTag);
        exp.SetTimer(UnityEngine.Random.Range(0.3f, 0.7f));

    }

}

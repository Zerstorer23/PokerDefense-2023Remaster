
using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] SpriteRenderer explosionImageMain;
    [SerializeField] SpriteRenderer explosionImageSub;
    [SerializeField] Animator animator;
   // ProjectileConfig myConfig;
    string myTag;
    
    public void SetInformation(ProjectileConfig pConfig) {
        explosionImageMain.sprite = pConfig.explosionSprite;
        animator.runtimeAnimatorController = pConfig.explosionAnimatorController;
    }

    public void SetTimer(float delay) {
        StartCoroutine(WaitAndKill(delay));
    }
    public void SetTag(string _tag) {
        myTag = _tag;
    }
    IEnumerator WaitAndKill(float delay) {
        yield return new WaitForSeconds(delay);
        KillMe();
    }

    public void KillMe()
    {
       ObjectPool.SaveObject(myTag, gameObject);
    }

}

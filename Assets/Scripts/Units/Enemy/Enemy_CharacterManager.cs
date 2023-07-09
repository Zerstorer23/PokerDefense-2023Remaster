
using UnityEngine;

public class Enemy_CharacterManager : MonoBehaviour
{
    internal Enemy_Main enemyMain;
    Animator animator;

    private void Awake()
    {
      //  Debug.Log("mineracharacterldisplay init srtart");
        animator = GetComponent<Animator>();
      //  Debug.Log("mineracharacterldisplay init end");
    }
    public void DoDeath() {
        enemyMain.DestroyCharacter();
    }

    internal void SetTrigger(string v)
    {
        animator.SetTrigger(v);
    }
}

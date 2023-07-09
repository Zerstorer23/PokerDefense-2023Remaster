
using System;
using UnityEngine;

public class EnemyUnitConfig
{

    

    public string name;
    public float moveSpeed = 6f;
    public float healthPoint;
    public float defensePoint;
    public EnemyType enemyType;
    public int amount = 0;

    public Sprite spriteImage = null;
   // public RuntimeAnimatorController animatorController = null;
    public GameObject characterBody = null;
    public bool hasAnim = true;
   public EnemyUnitConfig(int _stage, string _name,int _amount, float hp, float defense, float _moveSpeed) {
        healthPoint = hp;
        defensePoint = defense;
        if (_stage % 5 == 0) {
            enemyType = EnemyType.Boss;
        } else if (_amount > 100) {
            enemyType = EnemyType.Small;
        }
        amount = _amount;
        name = _name;
        moveSpeed = _moveSpeed;
    }
    public void SetSpriteAnimation(Sprite _sprite) {
        spriteImage = _sprite;
    
    }
    public void SetSpriteAnimation(GameObject character, Sprite _portrait)
    {
        characterBody = character;
        spriteImage = _portrait;
        // characterBody.GetComponent<Animator>().enabled = false;
    }

    internal int GetEnemyAmount()=>amount;
}

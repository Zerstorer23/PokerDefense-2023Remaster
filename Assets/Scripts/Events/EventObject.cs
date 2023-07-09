
using UnityEngine;


public class EventObject //<T>
{
//    public T myVal;
    public bool boolObj;
    public int intObj;
    public float floatObj;
    public string stringObj;
    public GameObject gameObject;
    public ScreenType screenType;
    public Skill skill;

    PokerHand pokerHand;
    Vector3 vectorObj;
/*    public EventObject(T a){
     myVal = a;
    }*/
    public EventObject(bool a)
    {
        boolObj = a;

    }
    public EventObject(int a)
    {
        intObj = a;

    }
    public EventObject(string s)
    {
        stringObj = s;

    }
    public EventObject(float f)
    {
        floatObj = f;

    }
    public EventObject(GameObject g)
    {
        gameObject = g;

    }
    public EventObject(PokerHand p)
    {
        pokerHand = p;
    }
    public EventObject(Skill sk) {
        skill = sk;
    }
    public EventObject(Vector3 v)
    {
        vectorObj = v;

    }

    public EventObject(ScreenType scr)
    {
        screenType = scr;

    }

    public int GetInt() { return intObj; }
  //  public T GetValue() { return myVal; }

    public string GetString() { return stringObj; }
    public GameObject GetGameObject() { return gameObject; }
    public PokerHand GetPokerHand() { return pokerHand; }
    public float GetFloat() => floatObj;

    public Vector3 GetVector() => vectorObj;
    public Skill GetSkill() => skill;


}

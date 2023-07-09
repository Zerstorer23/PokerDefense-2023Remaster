using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest 
{

    public delegate void voidFuntion();
    //
//    WaitAndDo(1, new voidFuntion(delTest1));

    private void delTest1()
    {

        Debug.Log("Test print");
    }
    private bool delTest2(float a)
    {

        Debug.Log("Test print 2");
        return true;
    }
    public static IEnumerator WaitAndDo(float sec, voidFuntion f1)
    {
        while (true)
        {
            yield return new WaitForSeconds(sec);
            f1();
        }
    }


}

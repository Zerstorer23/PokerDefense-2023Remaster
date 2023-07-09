using UnityEngine;

public class MineralManager : MonoBehaviour
{
    public const int KILL_REWARD = 8;
    public const int KILL_REWARD_SMALL = 2;
    public const int KILL_BOSS_REWARD = 512;
    /*  int vocal_mineral = 0;
      int visual_mineral = 0;
      int dance_mineral = 0 ;*/
   [SerializeField] int[] resources = { 0, 0, 0 };
    [SerializeField] bool cheat =false;
    private void Awake()
    {
     //   Debug.Log("mineral init start");
        if (cheat) {

            resources[0] = 1000000;
            resources[1] = 1000;
            resources[2] = 1000;
        }
      //  Debug.Log("mineral init start");
    }

    public void AddResource(UpgradeType uType, int m) {
        if (m == 0) return;
        resources[(int)uType] += m;
        EventManager.TriggerEvent(MyEvents.EVENT_MINERAL_CHANGED, null);

    }

    public void ResetResources() {
        resources[0] = 0;
        resources[1] = 0;
        resources[2] = 0;
        EventManager.TriggerEvent(MyEvents.EVENT_MINERAL_CHANGED, null);
    }
    public bool SpendResource(UpgradeType uType, int m) {
        if (resources[(int)uType] >= m)
        {
            resources[(int)uType] -= m;
            EventManager.TriggerEvent(MyEvents.EVENT_MINERAL_CHANGED, null);
            return true;
        }
        else {
            
            return false;
        }

    }
    public int GetResource(UpgradeType uType) { 
        
        return resources[(int)uType];
    }
}

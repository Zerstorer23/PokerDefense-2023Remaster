using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;
using Random = UnityEngine.Random;

public class ActiveSkillManager : MonoBehaviour
{
    //Skill Data
    [SerializeField] ProjectileConfig Chiyaha_Truck_Config;
    [SerializeField] ProjectileConfig Yukiho_Lockdown_Config;
    [SerializeField] ProjectileConfig Azusa_Slow_Config;
    [SerializeField] ProjectileConfig Azusa_Kill_Config;
    [SerializeField] ProjectileConfig Iori_Yamato_Config;
    [SerializeField] ProjectileConfig Makoto_Normal_Config;
    [SerializeField] ProjectileConfig Makoto_Burning_Config;

    [SerializeField] UnitConfig[] Hibiki_Underling_Config;
    [SerializeField] GameObject Underling_Base_Prefab;
    [SerializeField] GameObject AzusaUnderlingPrefab;

    MouseUser myMouse = MouseUser.ACTIVE_SKILL;
    // Start is called before the first frame update

    List<Skill>[] skill_slots = new List<Skill>[4];
    int[] skill_loaded = new int[4];
    float[] skill_max = new float[4];
    float[] skill_healPerTurn = new float[4];

    bool slotsInitialised = false;
    [SerializeField] SkillButtonBehaviour[] skill_buttons;
    bool[] buttons_visible = new bool[4];

    [SerializeField] public static bool skillMode = false;
    Skill skillToUse = null;
    int selectedSkillIndex = -1;


    [SerializeField] public Sprite[] buffsprites;
    public static Sprite attBuff, attSpd, damageMod,
     harukaBuff, shieldBreak, slowBuff, stun, yayoiBuff, makotoBuff, attLowBuff, makotoBuff2;

    private void Awake()
    {
       // Debug.Log("activeskill init start");
        EventManager.StartListening(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_REQUESTED, OnSkillActivateRequested);
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, WaitAndUpdate);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnClickTower);
        EventManager.StartListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnClickLand);
        EventManager.StartListening(MyEvents.EVENT_CLICK_ENEMY, OnClickEnemy);
        EventManager.StartListening(MyEvents.EVENT_ADD_ACTIVE_SKILL, AddActiveSkill);
        EventManager.StartListening(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_TOGGLED, OnSkillTogggled);
        EventManager.StartListening(MyEvents.EVENT_SKILL_UPGRADED, UpdateStackInfo);

        attBuff = buffsprites[0];
        attSpd = buffsprites[1];
        damageMod = buffsprites[2];
        harukaBuff = buffsprites[3];
        shieldBreak = buffsprites[4];
        slowBuff = buffsprites[5];
        stun = buffsprites[6];
        yayoiBuff = buffsprites[7];
        makotoBuff = buffsprites[8];
        attLowBuff = buffsprites[9];
        makotoBuff2 = Resources.Load("BuffIcons/slayer_active", typeof(Sprite)) as Sprite;
     //   Debug.Log("activeskill init end");

    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_REQUESTED, OnSkillActivateRequested);
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, WaitAndUpdate);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, OnClickTower);
        EventManager.StopListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnClickLand);
        EventManager.StopListening(MyEvents.EVENT_CLICK_ENEMY, OnClickEnemy);
        EventManager.StopListening(MyEvents.EVENT_ADD_ACTIVE_SKILL, AddActiveSkill);
        EventManager.StopListening(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_TOGGLED, OnSkillTogggled);
        EventManager.StopListening(MyEvents.EVENT_SKILL_UPGRADED, UpdateStackInfo);
    }

    private void OnPanelOpen(EventObject eo)
    {
        if (ScreenType.MAP != eo.screenType)
        {
            RemoveSelection(eo);
        }
    }

    private void WaitAndUpdate(EventObject eo) {
        StartCoroutine(UpdateHelper());
    }
    IEnumerator UpdateHelper() {
        yield return new WaitForFixedUpdate();
        UpdateStackInfo();
    }

    private void UpdateStackInfo(EventObject eo = null)
    {
        if (!slotsInitialised) return;
        for (int i = 0; i < skill_buttons.Length; i++)
        {
            if (skill_slots[i].Count == 0) continue;
            CalculateStacks(i);
            UpdateButtonUI(i);
            if (skill_loaded[i] > 0)
            {
                if (skill_slots[i][0].CanToggleAuto() && skill_buttons[i].GetAutoStatus())
                {
                    FireAutoSkill(i);
                }
            }

        }
    }

    private void CalculateStacks(int i)
    {
        skill_loaded[i] = 0;
        skill_max[i] = 0;
        skill_healPerTurn[i] = 0;
        try
        {
            foreach (Skill skill in skill_slots[i])
            {
                if (!skill.IsCasterActive()) continue;
                int thisStack = (int)skill.Active_CurrentStack();
                skill_loaded[i] += thisStack;
                float thisMax = skill.Active_GetMaxStack();
                skill_max[i] += thisMax;
                float heal = skill.Active_HealPerTurn();
                skill_healPerTurn[i] += heal;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }



    private void OnClickLand(EventObject eo)
    {
        if (!skillMode) return;
       // Debug.Log("Clicked land");
        Vector3 clickedPosition = eo.GetVector();
        FireSkillTo(clickedPosition, null);
    }

    private void OnClickEnemy(EventObject eo)
    {
        if (!skillMode) return;
        GameObject target = eo.GetGameObject();
        Vector3 clickedPosition = target.transform.position;
        FireSkillTo(clickedPosition, target);
    }
    private void OnClickTower(EventObject eo)
    {
        if (!skillMode) return;
        GameObject target = eo.GetGameObject();
      //  Debug.Log("Clicked tower " + target.GetComponent<Tower>().owner);
        if (target.GetComponent<Tower>().owner == Owner.NAMCO || !skillToUse.canTargetTower)
        {
            RemoveSelection(null);
            return;
        }
        Debug.Assert(skillToUse.canTargetTower == true, "Wrong target tower");
        Vector3 clickedPosition = target.transform.position;
        FireSkillTo(clickedPosition, target);
    }
    private void FireAutoSkill(int index)
    {
        bool hasSkillRemain = false;
        for (int i = 0; i < skill_slots[index].Count; i++)
        {

            Skill skill = skill_slots[index][i];
            if (skill.IsCasterActive())
            {
                while (skill.Active_CurrentStack() >= 1 && skill_buttons[index].GetAutoStatus())
                {
                    GameObject target = skill.FindMVPTarget();
                    if (target)
                    {
                        FireSkillTo(target.transform.position, target, skill);
                    }
                    else {
                        hasSkillRemain = true;
                        break;
                    }
                }

            }
        }
        if (hasSkillRemain) {
            StartCoroutine(WaitAndSearch(index, Random.Range(1.5f,3f)));
        }
    }

    IEnumerator WaitAndSearch(int index, float delay) {
        yield return new WaitForSeconds(delay);
        FireAutoSkill(index);
    }

    private void FireSkillTo(Vector3 targetLocation, GameObject targetObject, Skill skill = null)
    {
        bool findNextskill = false;
        if (skill == null) {
            skill = skillToUse;
            findNextskill = true;
        }
        if (skill.isTargetLand)
        {
            skill.SetSkillTarget(targetLocation);
        }
        else
        {
            GameObject target = (targetObject != null) ? targetObject : FindNearestUnit(targetLocation, skill.canTargetTower); ;
            if (target == null)
            {
                EventManager.TriggerEvent(MyEvents.EVENT_MESSAGE_TRIGGERED, new EventObject("==잘못된 대상==") { floatObj = 3f });
                return;
            }
            skill.SetSkillTarget(target);
        }
        skill.Activate();

        StartCoroutine(DoEndOfSkill(findNextskill));
    }

    IEnumerator DoEndOfSkill(bool findNextSkill) {

        yield return new WaitForFixedUpdate();
        if (findNextSkill)
        {
            CheckNextSkillAvailable();
        }
        UpdateStackInfo(null);
    }

    private void CheckNextSkillAvailable()
    {
        if (HasStack(selectedSkillIndex))
        {
            bool success = PrepareSkill(selectedSkillIndex);
            if (success)
            {
                skill_buttons[selectedSkillIndex].SetFocused(true);
            }
        }
        else
        {

            RemoveSelection_Delay();
        }
    }

    public void OnSkillActivateRequested(EventObject eo)
    {
        if (!ClickManager.IsNone() && ClickManager.GetCurrentUser() != myMouse) return;
        int requestedIndex = eo.GetInt();
        if (requestedIndex == selectedSkillIndex)
        {
            RemoveSelection(null);
        }
        else
        {
            bool success = PrepareSkill(requestedIndex);
            if (success)
            {
                skill_buttons[requestedIndex].SetFocused(true);
            }
        }
    }

    private bool PrepareSkill(int requestedIndex)
    {
        RemoveSelection(null);
        //    if (!valid) return false;
        for (int i = 0; i < skill_slots[requestedIndex].Count; i++)
        {
            if (skill_slots[requestedIndex][i].IsCasterActive() &&
                skill_slots[requestedIndex][i].Active_CurrentStack() >= 1)
            {
  
                SelectSkill(requestedIndex, i);
                ClickManager.ToggleUser(myMouse, true);
                return true;
            }
        }
        return false;
    }
    void SelectSkill(int skill_index, int unit_index)
    {
        skillToUse = skill_slots[skill_index][unit_index];
        selectedSkillIndex = skill_index;
        skillMode = true;
        if (skillToUse.GetSkillEffectRadius() > 0f)
        {
            EventManager.TriggerEvent(MyEvents.EVENT_CURSOR_RADIUS_REQUESTED, new EventObject(skillToUse.GetSkillEffectRadius()));
        }
    }
    void ResetFocus()
    {
        foreach (SkillButtonBehaviour obj in skill_buttons)
        {
            obj.SetFocused(false);
        }
    }

    private GameObject FindNearestUnit(Vector3 clickedPosition, bool includeKuroi)
    {
       // Debug.Log("Find nearest unit");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(clickedPosition, 2f, LayerMask.GetMask(STRING_LAYER_UNITS));
        double lowestDist = double.MaxValue;
        GameObject lowestObject = null;
        foreach (Collider2D c in hitColliders)
        {
            if (CheckValidTarget(c.gameObject, includeKuroi))
            {
                double dist = Vector2.Distance(c.gameObject.transform.position, clickedPosition);
                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    lowestObject = c.gameObject;
                }
           //     Debug.Log(c.gameObject.transform.position);
            }
        }

        return lowestObject;
    }
    bool CheckValidTarget(GameObject obj, bool includeKuroi)
    {
        if (obj.CompareTag(TAG_ENEMY)) return true;
        if (includeKuroi && obj.CompareTag(TAG_MINE))
        {
            if (obj.GetComponent<Tower>().owner == Owner.KUROI)
            {
                return true;
            };
        }
        return false;
    }
    void RemoveSelection_Delay()
    {
        StartCoroutine(WaitAndDelegate(RemoveSelection, null));
    }
    public delegate void eventFunction(EventObject eo);
    void RemoveSelection(EventObject eo)
    {
        if (ClickManager.GetCurrentUser() == myMouse)
        {
            ClickManager.ToggleUser(myMouse, false);
        }
        if (selectedSkillIndex < 0 || skillToUse == null) return;
        skillMode = false;
        if (skillToUse.GetSkillEffectRadius() > 0f)
        {
            EventManager.TriggerEvent(MyEvents.EVENT_CURSOR_RADIUS_REQUESTED, new EventObject(0f));
        }
        skillToUse.SetSkillTarget(null);
        skillToUse = null;
        selectedSkillIndex = -1;
        ResetFocus();
    }
    IEnumerator WaitAndDelegate(eventFunction func, EventObject eo)
    {
        yield return new WaitForFixedUpdate();
     //   Debug.Log("Wait and remove selection");
        func(eo);
    }
    void OnSkillTogggled(EventObject eo) {
        bool enabled = eo.boolObj;
     //   Debug.Log("Skil toggle" + enabled);
        if (enabled)
        {
            RemoveSelection(null);
            UpdateStackInfo();
        }
        else
        {
            RemoveSelection(null);
        }
    
    }


    private bool HasStack(int index)
    {
     //   Debug.Log("Search stack  ... " );
        try
        {
            foreach (Skill skill in skill_slots[index])
            {
                Debug.Log("skill caster" + skill.GetCaster());
                if (!skill.IsCasterActive()) continue;
            //    Debug.Log("Finding next ... " + skill.Active_CurrentStack());
                if (skill.Active_CurrentStack() >= 1)
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return false;
    }
    private void UpdateButtonUI(int button_index)
    {
        int curr = skill_loaded[button_index];
        int max = (int)skill_max[button_index];
        float heal = skill_healPerTurn[button_index];
        skill_buttons[button_index].SetInformation(skill_slots[button_index][0].txt_skill_name, curr, max, heal);

    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < skill_slots.Length; i++)
        {
            skill_slots[i] = new List<Skill>();
        }
        slotsInitialised = true;
    }

    public void AddActiveSkill(EventObject eo)
    {
        Skill skill = eo.GetSkill();

        if (skill.towerComponent.owner != Owner.NAMCO) return;
        if (skill.isLocked || skill.activeAdded) return;
        if (!slotsInitialised) InitialiseSlots();
        int index = skill.GetSkillSlot();
        skill_slots[index].Add(skill);
        skill.activeAdded = true;

        if (buttons_visible[index] == false)
        {
            SetSkillButtonVisibility(index, true);
        }
        UpdateStackInfo();
    }

    public void RemoveActiveSkill(GameObject removedObject, List<Skill> skills)
    {

        foreach (Skill sk in skills)
        {
            int index = sk.GetSkillSlot();
            if (index < 0 || !slotsInitialised) continue;
            for (int i = 0; i < skill_slots[index].Count; i++)
            {
                if (skill_slots[index][i].GetCaster().transform.position == removedObject.transform.position)
                {
                    skill_slots[index].RemoveAt(i);
                    sk.activeAdded = false;
                    if (!CheckHasActiveSkill(index) && buttons_visible[index])
                    {
                        SetSkillButtonVisibility(index, false);
                    };
                    break;
                }
            }
        }
        UpdateStackInfo();
    }

    private void SetSkillButtonVisibility(int index, bool enable)
    {
        if (enable)
        {
            EventManager.TriggerEvent(MyEvents.EVENT_SHOW_ACTIVESKILL, new EventObject(index));
        }
        else
        {

            EventManager.TriggerEvent(MyEvents.EVENT_HIDE_ACTIVESKILL, new EventObject(index));
        }
        buttons_visible[index] = enable;

    }

    private bool CheckHasActiveSkill(int index)
    {

        for (int i = 0; i < skill_slots[index].Count; i++)
        {
            if (skill_slots[index][i].IsCasterActive())
            {
                return true;
            }
        }
        return false;
    }

    public UnitConfig GetHibikiUnderlingConfig(int id)
    {
        return Hibiki_Underling_Config[id];
    }
    public GameObject GetUnderlingPrefab()
    {
        return Underling_Base_Prefab;
    }
    public GameObject GetAzusaUnderlingPrefab()
    {
        return AzusaUnderlingPrefab;
    }
}

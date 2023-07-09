using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class CharacterBodyManager : MonoBehaviour
{
    [SerializeField] GameObject wholeBody;
    [SerializeField] SpriteRenderer[] mainSprites;
    [SerializeField] SpriteRenderer[] subSprites;
    string category_hairFront = "HairFront";
    string category_hairRear = "HairRear";
    string category_eye = "NewEye";
    string category_mouth = "Mouth";
    string uid ="";

    [SerializeField] SpriteResolver hairFront;
    [SerializeField] SpriteResolver hairRear;
    [SerializeField] SpriteResolver eye;
    [SerializeField] SpriteResolver mouth;

    CharacterConfig myConfig;
    SpriteLibrary spriteLibrary;
    
    Animator characterAnimator;

    Tower tower;
    int[] random_eye_set = { 1, 2, 3, 4, 5};
    float lastChangeTime = 0f;
    float changeDelay = 7f;


    private void Awake()
    {
       // Debug.Log("charbody srtart");
        characterAnimator = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, OnWaveFinish);
      //  Debug.Log("charbody end");
    }
    private void Update()
    {
        if (tower == null || tower.IsInKnockBack()) return;

        if (Time.time >= lastChangeTime + changeDelay)
            {
            if (tower.owner == Owner.NAMCO)
            {
                int eyeIndex = Random.Range(0, random_eye_set.Length);
                SetEyeSkin(random_eye_set[eyeIndex] + "");
                changeDelay = Random.Range(4f, 11f);
            }
            else if (tower.owner == Owner.KUROI)
            {
                SetEyeSkin("6");
            }
            lastChangeTime = Time.time;
        }
        
        
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, OnWaveFinish);
      
    }
    private void OnWaveFinish(EventObject eo)
    {
        if (uid.Equals(ConstantStrings.MIKI)) {
            characterAnimator.SetTrigger("MIKI_DoNormalMode");
        }
    }

    internal void SetUID(CharacterConfig config, Tower t, string _uid) {
        myConfig = config;
        uid = _uid;
        tower = t;
    }
    internal void SetHairSkins(string key) {
        hairFront.GetComponent<SpriteResolver>().SetCategoryAndLabel(category_hairFront,key);
        hairRear.GetComponent<SpriteResolver>().SetCategoryAndLabel(category_hairRear, key);

        hairFront.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
        hairRear.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
    }
    internal void SetMouthSkin(string key)
    {
        SpriteResolver rs = mouth.GetComponent<SpriteResolver>();
        rs.SetCategoryAndLabel(category_mouth, key);
        rs.ResolveSpriteToSpriteRenderer();
    }
    internal void SetOriginalMouth() {
        if (myConfig == null) return;
        SpriteResolver rs = mouth.GetComponent<SpriteResolver>();
        rs.SetCategoryAndLabel(category_mouth, myConfig.mouthID);
        rs.ResolveSpriteToSpriteRenderer();
    }
    internal void SetEyeSkin(string key)
    {
       // Debug.Log("Set eye " + key);
        eye.GetComponent<SpriteResolver>().SetCategoryAndLabel(category_eye, key);
        eye.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
    }

    internal void SetColors(string main, string sub) {
        foreach (SpriteRenderer sprite in mainSprites) {
                Color c = ConstantStrings.GetColorByHex(main);
                if (c == null) return;
                sprite.color = c;
        }
        foreach (SpriteRenderer sprite in subSprites)
        {
            Color c = ConstantStrings.GetColorByHex(sub);
            if (c == null) return;
            sprite.color = c;
        }

    }
    private void OnEnable()
    {
        SetAnimationBool("IsKnockBack", false);
    }



    internal void SetAnimationTrigger(string key)
    {
        bool hasCustomAnimation = HasCustomAnimationTrigger(key);
        if (!hasCustomAnimation)
        {
            characterAnimator.SetTrigger(key);
        }
    }
    internal void FlipYAxis() {
        float x = wholeBody.transform.localScale.x;
        Debug.Log(wholeBody.transform.localScale);
        wholeBody.transform.localScale = new Vector3(-x, x, 1) ;

    }

    internal void SetAnimationBool(string key, bool value)
    {
        bool hasCustomAnimation = HasCustomAnimationBool(key, value);
        if (!hasCustomAnimation)
        {
            characterAnimator.SetBool(key, value);
        }
    }

    private bool HasCustomAnimationTrigger( string key) {
        bool hasCustomAnimation = false;
        if (uid.Equals(ConstantStrings.MIKI))
        {
             if (key.Equals("DoAttack"))
            {
                hasCustomAnimation = true;
            }
        }else if (uid.Equals(ConstantStrings.TAKANE))
        {
            if (key.Equals("DoAttack"))
            {
                hasCustomAnimation = true;
            }
        }

        if (hasCustomAnimation)
        {
            characterAnimator.SetTrigger(uid + "_" + key);
        }
        return hasCustomAnimation;
    }

    private bool HasCustomAnimationBool( string key, bool enable)
    {
        bool hasCustomAnimation = false;
        switch (uid)
        {
            case ConstantStrings.MIKI:
                if (key.Equals("IsKnockBack"))
                {
                    hasCustomAnimation = true;
                }
            break;
            case ConstantStrings.HARUKA:
                if (key.Equals("IsKnockBack"))
                {
                    hasCustomAnimation = true;
                }
                break;
            case ConstantStrings.CHIHAYA:
                if (key.Equals("IsKnockBack"))
                {
                    hasCustomAnimation = true;
                }
                break;
        }

        if (hasCustomAnimation)
        {
            characterAnimator.SetBool(uid + "_" + key,enable);
        }
        return hasCustomAnimation;
    }
}

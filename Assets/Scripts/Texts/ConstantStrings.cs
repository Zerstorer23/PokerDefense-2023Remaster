using UnityEngine;

public class ConstantStrings
{
    //Layers
    public const string STRING_LAYER_BOUNDARY = "Boundary";
    public const string STRING_LAYER_UNITS = "Units";
    public const string STRING_LAYER_PLACING_AREA = "PlacingArea";
    public const string STRING_LAYER_MOVE_AREA = "MoveArea";

    public const string PROJ_PARENT_NAME = "Projectiles";
    public const string TAG_ENEMY = "EnemyUnit";
    public const string TAG_MINE = "MyUnit";

    //Units
    public const string YUKIHO = "YUKIHO";
    public const string HARUKA = "HARUKA";
    public const string MIKI = "MIKI";
    public const string MAKOTO = "MAKOTO";
    public const string AMI = "AMI";
    public const string MAMI = "MAMI";
    public const string FUTAMI = "FUTAMI";
    public const string IORI = "IORI";
    public const string YAYOI = "YAYOI";
    public const string TAKANE = "TAKANE";
    public const string AZUSA = "AZUSA";
    public const string CHIHAYA = "CHIHAYA";
    public const string HIBIKI = "HIBIKI";
    public static string[] UID_List = { YUKIHO, HARUKA, MIKI, CHIHAYA, AMI, MAMI, TAKANE, AZUSA, HIBIKI, IORI, YAYOI, MAKOTO };

    //Panels
    public const string STRING_PANEL_LESSON = "LESSON";
    public const string STRING_PANEL_MAP = "MAP";
    public const string STRING_PANEL_DRAW = "DRAW";


    //Object pool tags

    public const string OBJ_EXPLOSION = "ObjectExplosion";
    public const string OBJ_EXPLOSION_HEAL = "ObjectExplosionHeal";
    public const string OBJ_DAMAGE_SIGN = "ObjectDamageSign";
    public const string OBJ_PROJECTILE = "ObjectProjectile";
    public const string OBJ_ENEMY_MOB = "ObjectEnemyMob";


    //Stat tags
    public const string STAT_TOTAL_TOWER_SPAWNED = "StatTotalTowerSpawned";
    public const string STAT_LESSON_OPENED = "StatLessonOpened";
/*    public const string STAT_KILLS = "TowerKill_";
    public const string STAT_DAMAGE = "TowerDamage_";
    public const string STAT_TOWER_NUMBERS = "TowerNumbers_";*/

    public static Color GetColorByHex(string hex)
    {
        Color newCol;
        ColorUtility.TryParseHtmlString(hex, out newCol);
        return newCol;
    }
}

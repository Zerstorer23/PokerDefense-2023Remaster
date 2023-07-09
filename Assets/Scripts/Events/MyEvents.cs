using UnityEngine.Events;


public class MyEvents {
    //HUD Manager
    public const string EVENT_ENEMY_DEAD = "EnemyUnitKilled";
    public const string EVENT_SHOW_PANEL = "ShowPanel"; //Only Listen by hud manager
    public const string EVENT_SHOW_IDOL_RANK = "ShowIdolRankPanel"; //Only Listen by hud manager
    public const string EVENT_SHOW_TOWER_OPTIONS = "ShowTowerOptions"; //Only Listen by hud manager

    //HUD Reactions
    public const string EVENT_OPEN_TOWER_SPAWN_SELECTOR_HUD = "OpenTowerSpawnSelectorHUD";
    public const string EVENT_TOWER_TO_SPAWN_SELECTED = "TowerToSpawnSelected";
    public const string EVENT_CANVAS_TRANSITION_FINISHED = "CanvasTransitionFinished";
    public const string EVENT_BACKGROUND_CLICKED = "BackgroundClicked";

    public const string EVENT_PLACE_AREA_CLICKED = "PlaceAreaClicked";
    public const string EVENT_SHOW_PURCHASE_OPTION = "ConstructionAreaClicked";
    public const string EVENT_MOUSE_USER_CHANGED = "MouseUserChanged";

    //Skills
    public const string EVENT_SHOW_ACTIVESKILL = "ActiveSkillShow";
    public const string EVENT_HIDE_ACTIVESKILL = "ActiveSkillHide";
    public const string EVENT_ACTIVE_SKILL_ACTIVATE_REQUESTED = "ActiveSkillActivateToggled";
    public const string EVENT_ACTIVE_SKILL_ACTIVATE_TOGGLED = "ActiveSkillActivateRequested";
    public const string EVENT_CLICK_TOWER = "TowerClicked";
    public const string EVENT_CLICK_ENEMY = "EnemyClicked";
    public const string EVENT_POKERHAND_FINALISED = "PokerHandFinalised";
    public const string EVENT_POKERHAND_FINALISED_AI = "PokerHandFinalisedAI";
    public const string EVENT_CARD_CHANGE_REQUESTED = "CardChangeRequested";
    public const string EVENT_MINERAL_CHANGED = "MineralChanged";
    public const string EVENT_SKILL_UPGRADED = "SkillUpgraded";
    public const string EVENT_ADD_ACTIVE_SKILL = "AddActiveSkill";
    public const string EVENT_ACTIVESKILL_TOGGLE_POSSIBLE = "ToggleAutoActiveSkill";
  //  public const string EVENT_ACTIVESKILL_AUTO_ON_OFF = "ActiveskillAutoOnOff";

    //Mouse Actions
    public const string EVENT_CURSOR_RADIUS_REQUESTED = "CursorRadiusRequested";

    //Waves


    public const string EVENT_GAMESESSION_WAVE_STARTED = "WaveStarted";
    public const string EVENT_GAMESESSION_WAVE_FINISHED = "GameSessionWaveFinished"; //Only triggered by game session

    public const string EVENT_WAVE_ALL_DEAD = "WaveAllDead";
    public const string EVENT_WAVE_ALL_SPAWNED = "WaveAllSpawned";
    public const string EVENT_WAVE_TIMEOUT = "WaveTimeOut";

    //Towers
    public const string EVENT_TOWER_RELOCATION_REQUESTED = "TowerRelocationRequested";
    public const string EVENT_TOWER_SELL_REQUESTED = "TowerSellRequested";
    public const string EVENT_TOWER_PLACED = "TowerPlaced";
    public const string EVENT_TOWER_REMOVED = "TowerRemoved";
    public const string EVENT_TOWER_DISABLED = "TowerDisabled";
    public const string EVENT_WAVE_START_REQUESTED = "RequestWaveStart"; //Called by towerspawner on spawn finish
 //   public const string EVENT_TOWER_REMOVE_ALL_OF = "TowerAllRemoveOfRequested";
    public const string EVENT_RESERVE_PLACE_RESULT = "ReservePlaceResult";
  //  public const string EVENT_UNITOPTION_SHOW_REQUEST = "OpenUnitOptions";
    public const string EVENT_UNITOPTION_HIDE_REQUEST = "HideUnitOptions";


    public const string EVENT_MESSAGE_TRIGGERED = "MessageTriggered";
    public const string EVENT_LOCALIZATION_LOADED = "LocalizationLoaded";
}

public class EventOneArg : UnityEvent<EventObject>
{


}


using KKSG;
using Koakuma.Game.Characters;
using Koakuma.Game.Items;
using Koakuma.Game.Scenes;
using Koakuma.Proto;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{


    public struct CreateCharacterView
    {
        public CharacterEntity character;
        public CharacterData characterData;
    }

    public struct CharacterVitalized
    {
        public long entityID;
    }

    public struct GameSceneCreating
    {
        public CreateSceneData data;
    }

    public struct GameSceneCreated
    {
        public CreateSceneData data;
    }

    public struct CharacterAttributeChanged
    {
        public long entityID;
        public CharacterAttributeType type;
        public long oldValue;
        public long newValue;
    }

    public struct Login
    {
        public string serverName;
        public int serverID;
        public int serverId;
    }

    public struct LoginResp
    {
        public GameInfo gameInfo;
    }
    public struct EnterServer
    {
       public QueryGateRes res;
    }
    public struct ChangeLobbyProcedure
    {

    }
    public struct EnterLobby
    {
        
    }

    public struct EnterDungeon
    {
    }

    public struct ExitDungeon
    {

    }

    public struct EnterNextDungeon
    {
    }

    public struct PickupItem
    {
        public long entityID;
    }

    public struct SkillCastFinished
    {
        public long entityID;
        public bool isCancel;
    }

    public struct SetInputEnableState
    {
        public bool enabled;
    }

    public struct GameSceneLoadingStateChanged
    {
        public bool isLoading;
    }

    public struct DropEquipment
    {
        public Vector3 position;
        public Koakuma.Proto.EquipmentInfo equipmentInfo;
    }

    public struct PlayerDead
    {
    }

    public struct ActiveMonsterSpawner
    {
    }

    public struct WearEqupmentRequest
    {
        public long itemID;
    }

    public struct TakeOffEquipmentRequest
    {
        public Config.Parts parts;
    }

    public struct AddResource
    {
        public ResourceType type;
        public long value;
    }

    public struct ResourceChanged
    {
        public ResourceType type;
        public long oldValue;
        public long newValue;
    }
}

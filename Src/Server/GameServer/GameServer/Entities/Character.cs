using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase
    {
        public TCharacter Tcharacter;
        public Character(CharacterType type, TCharacter Tdata) : base(new Vector3Int(Tdata.MapPosX, Tdata.MapPosY, Tdata.MapPosZ), new Vector3Int(100,0,0))
        {
            this.Tcharacter = Tdata;

            this.Ncharacterinfo = new NCharacterInfo();
            this.Ncharacterinfo.Id = Tdata.ID;
            this.Ncharacterinfo.Tid = Tdata.TID;
            this.Ncharacterinfo.mapId = Tdata.MapID;
            this.Ncharacterinfo.Type = type;
            this.Ncharacterinfo.Name = Tdata.Name;
            this.Ncharacterinfo.Class = (CharacterClass)Tdata.Class;
            this.Ncharacterinfo.Level = 1;
            this.Ncharacterinfo.Entity = this.Nentity;

            this.characterdefine = DataManager.Instance.Characters[Tdata.TID];
        }
    }
}

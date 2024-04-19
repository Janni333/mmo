using Common.Data;
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
    class CharacterBase : Entity
    {
        #region Property & Fields
        public int ID
        {
            get 
            {
                return this.EntityId;
            }
        }
        public NCharacterInfo Ncharacterinfo;
        public CharacterDefine characterdefine;
        #endregion


        #region Function
        public CharacterBase(Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {
        }

        public CharacterBase(CharacterType type, int Tid, int level, Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {
            this.Ncharacterinfo = new NCharacterInfo();
            this.Ncharacterinfo.Tid = Tid;
            this.Ncharacterinfo.Type = type;
            this.Ncharacterinfo.Level = level;
            this.Ncharacterinfo.Entity = this.Nentity;
            this.characterdefine = DataManager.Instance.Characters[Tid];
        }
        #endregion
    }
}

using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public UnityAction<Character> OnAddCharacter;
        //管理器字典
        public Dictionary<int, Character> ClientCharacters = new Dictionary<int, Character>();

        //构造 析构 初始化
        public CharacterManager()
        { 
        }
        public void Dispose()
        {
        }
        public void Init()
        {
        }

        public void Clear()
        {
            this.ClientCharacters.Clear();
        }

        public Character Add(NCharacterInfo NInfo)
        {
            Character newCha = new Character(NInfo);
            this.ClientCharacters[NInfo.Id] = newCha;
            if(this.OnAddCharacter != null)
            {
                this.OnAddCharacter(newCha);
            }
            return newCha;
        }
    }
}

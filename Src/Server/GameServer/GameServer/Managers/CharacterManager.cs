using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 240407
 *  创建和基本逻辑
 */
namespace GameServer.Managers
{
    class CharacterManager: Singleton<CharacterManager>
    {
        //管理器字典
        Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        //构造
        //初始化
        //析构
        public CharacterManager()
        { }
        public void Init()
        { }
        public void Despose()
        { }

        //清除管理器字典
        public void Clear()
        {
            this.Characters.Clear();
        }

        //创建新实体并加入管理器字典
        public Character Add(CharacterType type, TCharacter Tcha)
        {
            Character newcha = new Character(type, Tcha);
            this.Characters[newcha.ID] = newcha;
            return newcha;
        }

        //删除管理器字典元素
        public void Remove(int charid)
        {
            this.Characters.Remove(charid);
        }
    }
}

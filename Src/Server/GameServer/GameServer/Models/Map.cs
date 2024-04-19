using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 240407
 *  创建与基础逻辑
 */

/*
 * 增删改
 *  1 MapCharacer类
 *  2 CharacterEnterMap character加入mapcharacters顺序调整，防止重复发送给自己
 */
namespace GameServer.Models
{
    class Map
    {
        //Map
        internal MapDefine mapDefine;
        public int ID
        {
            get
            {
                return this.mapDefine.ID;
            }
        }

        internal Map(MapDefine define)
        {
            this.mapDefine = define;
        }

        //1 MapCharacter
        internal class MapCharacter
        {
            internal Character Character;
            internal NetConnection<NetSession> chaConnection;
            public MapCharacter(NetConnection<NetSession> sender, Character cha)
            {
                this.Character = cha;
                this.chaConnection = sender;
            }
        }
        Dictionary<int, MapCharacter> mapCharacters = new Dictionary<int, MapCharacter>();

        public void Update()
        { }

        //角色进入地图
        public void CharacterEnterMap(NetConnection<NetSession> conn, Character character)
        {
            //日志
            Log.InfoFormat("接收角色进入地图请求：character:{0} map:{1} {2}", character.Tcharacter.Name, this.ID, this.mapDefine.Name);

            //角色加入本地图角色管理器
            //this.mapCharacters.Add(character.Info.Id, new MapCharacter(conn, character));
            //this.mapCharacters[character.Info.Id] = new MapCharacter(conn, character);

            //响应消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Ncharacterinfo);

            //向其他角色发送角色进入地图消息
            foreach (var mapcharacter in mapCharacters.Values)
            {
                message.Response.mapCharacterEnter.Characters.Add(mapcharacter.Character.Ncharacterinfo);
                this.SendCharacterEnterMap(mapcharacter.chaConnection, character);
            }

            //修改2 
            this.mapCharacters[character.Ncharacterinfo.Id] = new MapCharacter(conn, character);

            //向自己客户端发送角色进入消息
            Log.InfoFormat("发送角色进入地图响应：character:{0} map:{1} {2}", character.Tcharacter.Name, this.ID, this.mapDefine.Name);
            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        void SendCharacterEnterMap(NetConnection<NetSession> sender, Character character)
        {
            //日志
            Log.InfoFormat("向{0}发送角色{1}进入地图{2}{3}响应", sender.Session.Character.Tcharacter.Name, character.Tcharacter.Name,this.ID, this.mapDefine.Name);

            //消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Ncharacterinfo);

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
    }
}

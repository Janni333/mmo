using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;


/*
 * 240401
 *  注册逻辑
 *  登陆逻辑
 * 240406
 *  角色创建逻辑
 */

/*
 * 增删改
 *  1 从数据库中拉取数据
 *  2 向数据库中增加条目
 *  3 登录拉取填充数据逻辑
 *  4 角色创建数据库增加将TCharacter条目加入Character表
 *  5 OnGameEnter：EnterGame 响应填充character
 */

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            //订阅
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCharacterCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
        }

        public void Init()
        {
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            //日志
            Log.InfoFormat("接收注册请求：username:{0} password:{1}", request.User, request.Passward);

            //消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            //校验
            //  重复返回
            //  成功：数据库添加新用户和玩家；完成消息

            //修改1：从数据库拉取数据
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {
                //修改2：向数据库中添加新的条目，从最底层条目开始创建
                /*原写法
                TUser newuser = new TUser();
                newuser.Username = request.User;
                newuser.Password = request.Passward;
                newuser.Player = new TPlayer();
                DBService.Instance.Entities.Users.Add(newuser);
                 */
                TPlayer newplayer = DBService.Instance.Entities.Players.Add(new TPlayer());     //Add方法返回TEntity
                DBService.Instance.Entities.Users.Add(new TUser {Username = request.User, Password = request.Passward, Player = newplayer});
                DBService.Instance.Entities.SaveChanges();


                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "注册成功";
            }

            //发送消息
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            //日志
            Log.InfoFormat("接收登录请求：username:{0} password:{1}", request.User, request.Passward);

            //准备消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            //信息校验
            //  用户存在
            //  密码正确
            //登陆成功
            //  填充消息
            //  数据库数据 ---> 网络数据
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户名错误";
            }
            else if (user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else 
            {
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "登录成功";

                //修改3 拉取填充数据
                /*
                NUserInfo -> NPlayerInfo -> NCharacterInfo
                List<NCharacterInfo> characters = new List<NCharacterInfo>();
                foreach (TCharacter cha in user.Player.Characters)
                {
                    NCharacterInfo character = new NCharacterInfo();
                    character.Id = cha.ID;
                    character.Tid = cha.TID;
                    character.Name = cha.Name;
                    character.Type = CharacterType.Player;
                    character.Class = (CharacterClass)cha.Class;
                    character.mapId = cha.MapID;
                    character.Entity = new NEntity
                }
                */
                sender.Session.User = user;
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = 1;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (TCharacter cha in user.Player.Characters)
                {
                    NCharacterInfo character = new NCharacterInfo
                    {
                        Id = cha.ID,
                        Name = cha.Name,
                        Class = (CharacterClass)cha.Class
                    };
                    message.Response.userLogin.Userinfo.Player.Characters.Add(character);
                }
            }

            //发送消息
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        private void OnCharacterCreate(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            //日志
            Log.InfoFormat("接收角色创建请求：username:{0} password:{1}", request.Name, request.Class);

            //DB操作
            //  新建数据库角色对象
            //  Session
            //  保存
            TCharacter newTChar = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };
            //修改4
            DBService.Instance.Entities.Characters.Add(newTChar);
            sender.Session.User.Player.Characters.Add(newTChar);
            DBService.Instance.Entities.SaveChanges();

            //消息与发送
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "角色创建成功";
            foreach (var cha in sender.Session.User.Player.Characters)
            {
                NCharacterInfo Char = new NCharacterInfo()
                {
                    Tid = cha.TID,
                    Name = cha.Name,
                    Type = CharacterType.Player,
                    Class = (CharacterClass)cha.Class,
                    mapId = cha.MapID,

                };
                message.Response.createChar.Characters.Add(Char);
            }
            
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            //拉取角色
            //ICollection集合不可以用[]索引访问元素
            //TCharacter enterChar = sender.Session.User.Player.Characters[request.characterIdx];
            TCharacter enterTChar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);

            //日志
            Log.InfoFormat("接收进入游戏请求：Character:{0}", enterTChar.Name);

            //创建逻辑角色
            Character enterChar = CharacterManager.Instance.Add(CharacterType.Player, enterTChar);

            //消息和发送
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Result = Result.Success;
            message.Response.gameEnter.Errormsg = "进入游戏成功";
            //修改5 
            //message.Response.gameEnter.

            Log.InfoFormat("发送进入游戏响应：Character:{0}", enterTChar.Name);
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);

            //填充本地角色
            sender.Session.Character = enterChar;

            //调用MapService角色进入地图 
            MapManager.Instance[enterChar.Tcharacter.MapID].CharacterEnterMap(sender, enterChar);
        }
    }
}

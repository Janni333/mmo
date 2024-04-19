using Entities;
using Managers;
using Assets.Scripts.Model;
using Common.Data;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*
 * 240409
 *  OnCharacterEnter逻辑
 *  EnterMap逻辑
 */

/*
 * 修改
 *  1 加载场景时需要校验地图是否存在
 *  2 可以利用响应中的角色列表更新当前本地角色
 */

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        /*
         *信息
         *  当前地图
         */

        int CurrentMap;

        //构造
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
        }

        //析构
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
        }
        //初始化
        public void Init()
        { 
        }


        private void OnCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            //日志
            Debug.LogFormat("接收角色进入地图响应：Character:{0} Map:{1}{2}", response.Characters[0], response.mapId, DataManager.Instance.Maps[response.mapId]);

            //将响应中角色列表拉入客户端角色管理器
            this.AddCharacter(response.Characters);

            //加载场景（或不加载）
            this.LoadMap(response.mapId);

            //调用GameObjectManager创建实体

        }

        private void AddCharacter(List<NCharacterInfo> chars)
        {
            foreach (var cha in chars)
            {
                //2 刷新本地角色
                if (User.Instance.currentCharacter == null || User.Instance.currentCharacter.Id == cha.Id)
                {
                    User.Instance.currentCharacter = cha;
                }
                Character newCha = CharacterManager.Instance.Add(cha);
            }
        }

        private void LoadMap(int mapId)
        {
            //校验并切换场景
            if (CurrentMap != mapId)
            {
                //修改
                if (DataManager.Instance.Maps.ContainsKey(mapId))
                {
                    MapDefine map = DataManager.Instance.Maps[mapId];
                    SceneManager.Instance.LoadScene(map.Resource);
                    CurrentMap = mapId;
                    User.Instance.currentMapdefine = map;
                    Debug.LogFormat("角色进入新地图{0}", DataManager.Instance.Maps[mapId].Name);
                }
            }
            else
            {
                Debug.LogFormat("角色进入当前地图");
            }
        }
    }
}

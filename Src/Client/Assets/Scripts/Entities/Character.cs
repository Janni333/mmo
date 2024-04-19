using Assets.Scripts.Model;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        /*
         *信息
         *  网络信息 / 配置信息
         *  姓名
         *  是否玩家
         */
        public NCharacterInfo Ncharacterinfo;
        public CharacterDefine characterdefine;
        public string name
        {
            get 
            {
                return this.Ncharacterinfo.Name;
            }
        }
        public bool isPlayer
        {
            get
            {
                /*白痴写法
                if (Ncharacterinfo.Id == Model.User.Instance.currentCharacter.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
                return (this.Ncharacterinfo.Id == User.Instance.currentCharacter.Id);
            }
        }

        //构造
        public Character(NCharacterInfo NInfo) : base(NInfo.Entity)
        {
            this.Ncharacterinfo = NInfo;
            this.characterdefine = DataManager.Instance.Characters[NInfo.Tid];
        }

        //移动方法
        //  向前/后 停止 设置方向 设置位置
        public void MoveForward()
        {
            Debug.LogFormat("{0}向前移动", this.name);
            this.speed = this.characterdefine.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("{0}向前移动", this.name);
            this.speed = -this.characterdefine.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("{0}停止移动", this.name);
            this.speed = 0;
        }

        public void SetPosition(Vector3Int pos)
        {
            Debug.LogFormat("更新位置：{0}", pos);
            this.position = pos;
        }

        public void SetDirection(Vector3Int dir)
        {
            Debug.LogFormat("更新方向：{0}", dir);
            this.direction = dir;
        }
    }
}

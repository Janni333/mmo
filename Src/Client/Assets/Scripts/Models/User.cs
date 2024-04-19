using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 增删改
 *  1 UserInfo属性
 */
namespace Assets.Scripts.Model
{
    class User: Singleton<User>
    {
        private NUserInfo userinfo;

        //修改1
        /*
        public NUserInfo UserInfo
        {
            get 
            {
                return userinfo;
            }
            set 
            {
                UserInfo = value;
                SetInfo(UserInfo);
            }
        }
        */
        public NUserInfo UserInfo
        {
            get 
            { return userinfo; }
        }
        public void SetInfo(NUserInfo userInfo)
        {
            this.userinfo = userInfo;
        }

        public NCharacterInfo currentCharacter;

        public GameObject currentGameObject;

        public MapDefine currentMapdefine;
    }
}

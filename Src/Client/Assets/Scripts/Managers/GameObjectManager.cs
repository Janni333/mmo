using Assets.Scripts.Model;
using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

/*
 *修改
 *  1 创建角色时需要校验角色是否已存在
 */

namespace Managers
{
    class GameObjectManager : MonoSingleton<GameObjectManager>
    {
        //管理器字典
        Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

        //启动
        protected override void OnStart()
        {
            //协程创建当前游戏所有角色
            StartCoroutine(InitGameObject());

            //注册角色进入事件
            CharacterManager.Instance.OnAddCharacter += OnCharacterEnter;
        }

        //销毁
        void OnDestroy()
        {
            //取消注册
            CharacterManager.Instance.OnAddCharacter -= OnCharacterEnter;
        }

        //协程
        IEnumerator InitGameObject()
        {
            foreach(var kv in CharacterManager.Instance.ClientCharacters)
            {
                this.CreateCharacterObject(kv.Value);
                yield return null;
            }
        }

        //角色进入事件响应
        void OnCharacterEnter(Character enterCha)
        {
            //调用创建角色
            this.CreateCharacterObject(enterCha);
        }

        void CreateCharacterObject(Character cha)
        {
            //修改1 校验
            if (!Characters.ContainsKey(cha.entityId) || Characters[cha.entityId] == null)
            {
                // 不存在的角色
                if (!DataManager.Instance.Characters.ContainsKey(cha.characterdefine.TID))
                {
                    Debug.Log("角色不存在");
                    return;
                }

                //角色存在
                //  在管理器根节点下初始化GameObject
                Object obj = Resloader.Load<Object>(cha.characterdefine.Resource);
                GameObject CharacterObject = (GameObject)Instantiate(obj, this.transform);
                this.Characters[cha.entityId] = CharacterObject;
                //  初始化GameObject
                this.InitCharacterObject(Characters[cha.entityId], cha);
            }
        }

        private void InitCharacterObject(GameObject chaObject, Character cha)
        {
            chaObject.name = "Character_" + cha.characterdefine.Name + cha.Ncharacterinfo.Id + cha.name;
            if (cha.Ncharacterinfo.Id == User.Instance.currentCharacter.Id)
            {
                User.Instance.currentGameObject = chaObject;
                MainPlayerCamera.Instance.player = chaObject;
            }

            //实体
            chaObject.transform.position = GameObjectTool.LogicToWorld(cha.position);
            chaObject.transform.forward = GameObjectTool.LogicToWorld(cha.direction);

            //ec & pc
            PlayerInputController pc = chaObject.GetComponent<PlayerInputController>();
            EntityController ec = chaObject.GetComponent<EntityController>();

            if (ec != null)
            {
                ec.curEntity = cha;
                ec.isPlayer = cha.isPlayer;
            }

            if (pc != null)
            {
                if (cha.Ncharacterinfo.Id == User.Instance.currentCharacter.Id)
                {
                    pc.enabled = true;
                    pc.curCharacter = cha;
                    pc.ec = ec;
                }
                else 
                {
                    pc.enabled = false;
                }
            }
            
        }
    }
}

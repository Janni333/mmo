using Assets.Scripts.Model;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MinimapManager: MonoSingleton<MinimapManager>
    {
        //当前场景小地图和小地图更新所有需要信息
        public UIMinimap minimap;
        private Transform playerTransform;
        public Transform PlayerTransform
        {
            get
            {
                if (this.playerTransform == null)
                {
                    this.playerTransform = User.Instance.currentGameObject.transform;
                }
                return this.playerTransform;
            }
        }

        private MapDefine minidefine;
        public MapDefine miniDefine
        {
            get
            {
                if (this.minidefine == null)
                {
                    this.minidefine = User.Instance.currentMapdefine;
                }
                return this.minidefine;
            }
        }


        protected override void OnStart()
        {
            Sprite ima = this.LoadSprite();
            this.minimap.InitMap(this.miniDefine.Name, ima);
        }

        Sprite LoadSprite()
        {
            Sprite mapSprite = Resloader.Load<Sprite>("UI/Minimap/" + miniDefine.Minimap);
            return mapSprite;
        }

    }
}

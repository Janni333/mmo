using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EntityController : MonoBehaviour
{
    /*信息
     *   
     */
    public Animator ani;
    public Rigidbody rb;
    public Collider co;
    public Entity curEntity;

    public float Speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;
    public bool isPlayer = false;

    public Vector3 Position;
    public Vector3 Direction;
    public Quaternion Rotation;
    public Vector3 lasPosition;
    public Quaternion lasRotation;

    void Start()
    {
        if (this.curEntity != null)
        {
            this.UpdateTransform();
        }

        if (!this.isPlayer)
        {
            this.rb.useGravity = false;
        }
    }
    void OnDestroy()
    {
        if (this.curEntity != null)
        {
            Debug.LogFormat("{0}Destroy", this.curEntity.entityId);
        }
    }

    //每帧移动
    void FixedUpdate()
    {
        if (this.curEntity == null)
        {
            return;
        }

        this.curEntity.OnUpdate(Time.deltaTime);

        if (! isPlayer)
        {
            this.UpdateTransform();
        }
    }

    //从逻辑更新对象
    void UpdateTransform()
    {
        this.Position = GameObjectTool.LogicToWorld(this.curEntity.position);
        this.Direction = GameObjectTool.LogicToWorld(this.curEntity.direction);

        this.rb.MovePosition(this.Position);
        this.transform.forward = this.Direction;
        this.lasPosition = this.Position;
        this.lasRotation = this.Rotation;
    }

    //动画切换
    public void OnEntityEvent(EntityEvent eve)
    {
        switch (eve)
        {
            case EntityEvent.Idle:
                ani.SetBool("isMove", false);
                ani.SetTrigger("isIdle");
                break;
            case EntityEvent.MoveBack:
                ani.SetBool("isMove", true);
                break;
            case EntityEvent.MoveFwd:
                ani.SetBool("isMove", true);
                break;
            case EntityEvent.Jump:
                ani.SetBool("isMove", false);
                ani.SetTrigger("isJump");
                break;
        }
    }
}

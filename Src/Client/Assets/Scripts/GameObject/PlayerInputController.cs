using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class PlayerInputController : MonoBehaviour
{
    /*
     *信息
     *  
     */
    
    CharacterState curState = CharacterState.Idle;
    public Character curCharacter;
    public EntityController ec;
    public Rigidbody rb;
    public bool onAir = false;

    public int Speed;
    public float rotateSpeed = 2.0f;
    public float turnAngle = 10;

    void FixedUpdate()
    {
        //确定是否绑定角色
        if (curCharacter != null)
        {
            //移动
            //  获取Vertical输入
            //  判断阈值向前/向后/静止
            //      判断并设置State
            //      调用移动方法
            //      发送事件
            //      计算速度
            float v = Input.GetAxis("Vertical");
            if (v > 0.01f)
            {
                //状态改变
                if (this.curState != CharacterState.Move)
                {
                    this.curState = CharacterState.Move;
                    curCharacter.MoveForward();
                    this.SendEntityEvent(EntityEvent.MoveFwd);
                }
                //rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
                rb.velocity = GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
            }
            else if (v < -0.01f)
            {
                if (this.curState != CharacterState.Move)
                {
                    this.curState = CharacterState.Move;
                    curCharacter.MoveBack();
                    this.SendEntityEvent(EntityEvent.MoveBack);
                }
                //rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
                rb.velocity = GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
            }
            else 
            {
                if (this.curState != CharacterState.Idle)
                {
                    this.curState = CharacterState.Idle;
                    curCharacter.Stop();
                    this.SendEntityEvent(EntityEvent.Idle);
                }
            }

            //跳跃
            //  发送事件
            if (Input.GetButtonDown("Jump"))
            {
                this.SendEntityEvent(EntityEvent.Jump);
            }


            //旋转
            //  获取Horizontal输入
            //  判断阈值
            float h = Input.GetAxis("Horizontal");
            if (h < 0.1 || h > 0.1)
            {
                this.transform.Rotate(0, h * rotateSpeed, 0);

                Vector3 charDir = GameObjectTool.LogicToWorld(this.curCharacter.direction);
                Quaternion rot = new Quaternion();
                rot.SetFromToRotation(this.transform.forward, charDir);

                if (rot.eulerAngles.y > this.turnAngle || rot.eulerAngles.y < (360 - this.turnAngle))
                {
                    this.curCharacter.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                    rb.transform.forward = this.transform.forward;
                }
            }
        }
    }

    Vector3 lastpos;
    void LateUpdate()
    {
        if (this.curCharacter == null) return;

        Vector3 offset = this.rb.transform.position - this.lastpos;
        this.Speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastpos = this.rb.transform.position;

        //更新transform position
        this.transform.position = this.lastpos;
        //更新logical position
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.curCharacter.position).magnitude > 50f)
        {
            this.curCharacter.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    void SendEntityEvent(EntityEvent eve)
    {
        if (this.ec != null)
        {
            this.ec.OnEntityEvent(eve);
        }
    }
}

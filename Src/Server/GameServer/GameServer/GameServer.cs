using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    /*
     * 240326
     *  初始化网络服务
     *  启动网络服务
     *  停止网络服务
     *  初始化测试服务
     *  
     *240401
     *  初始化UserService
     */
    class GameServer
    {
        NetService net = new NetService();
        Thread thread;
        bool running = false;
        public bool Init()
        {
            net.Init(8089);
            DBService.Instance.Init();
            TestService.Instance.Init();
            UserService.Instance.Init();

            DataManager.Instance.Load();
            MapManager.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            net.Start();
            TestService.Instance.Start();

            running = true;
            thread.Start();
        }


        public void Stop()
        {
            net.Stop();
            running = false;
            thread.Join();
        }

        public void Update()
        {
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
            }
        }
    }
}

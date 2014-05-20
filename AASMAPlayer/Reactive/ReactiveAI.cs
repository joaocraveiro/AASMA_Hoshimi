using System;
using System.Collections.Generic;
using System.Text;
using AASMAHoshimi;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Reactive
{
    public class ReactiveAI : AASMAAI
    {
        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }


        private int shooterAlive(Type shooterType)
        {            
            NanoBotCollection bots = getAASMAFramework().NanoBots;
            int numBots = 0;
            foreach(NanoBot bot in bots)
            {
                if (bot.GetType() == shooterType)
                {
                    numBots++;
                }
            }
            return numBots;
        }
        
        // Build needles, protectors, containers, explorers. By this order.        
        public override void DoActions()
        {
            // NEEDLE CREATION
            if (getAASMAFramework().overHoshimiPoint(this._nanoAI) && !getAASMAFramework().overNeedle(this._nanoAI))
            {
                this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
            }

            // PROTECTORS CREATION
          
            if(shooterAlive(typeof(ReactiveSpawnProtector)) <= 3)
            {
                this._nanoAI.Build(typeof(ReactiveSpawnProtector), "P" + this._protectorNumber++);
            }
            if (shooterAlive(typeof(ReactiveBodyGuardProtector)) <= 3)
            {
                this._nanoAI.Build(typeof(ReactiveBodyGuardProtector), "P" + this._protectorNumber++);
            }
            if (shooterAlive(typeof(ReactiveProtector)) <= 5)
            {
                this._nanoAI.Build(typeof(ReactiveProtector), "P" + this._protectorNumber++);
            }


            // CONTAINERS CREATION
            else if (getAASMAFramework().containersAlive() <= 2)
            {
                this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);
            }


            // EXPLORERS CREATION
            else if (getAASMAFramework().explorersAlive() <= 2)
            {
                this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
            }
            
            // MOVING
            else
            {
                List<Point> hoshimis = getAASMAFramework().visibleHoshimies(this._nanoAI);
                List<Point> needles = getAASMAFramework().visibleEmptyNeedles(this._nanoAI);
                needles.AddRange(getAASMAFramework().visibleFullNeedles(this._nanoAI));
                foreach (Point p in needles) { hoshimis.Remove(p); }
                if (hoshimis.Count > 0)
                {
                    this._nanoAI.MoveTo(hoshimis[0]);                     
                }

                else
                {
                    if (frontClear())
                    {
                        this.MoveForward();
                    }
                    else
                    {
                        this.RandomTurn();
                    }
                }
            }
        }

        public override void receiveMessage(AASMAMessage msg)
        {
        }

    }
}

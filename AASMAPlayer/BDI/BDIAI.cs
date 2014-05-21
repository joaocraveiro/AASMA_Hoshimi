using System;
using System.Collections.Generic;
using System.Text;
using AASMAHoshimi;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
    public class BDIAI : AASMAAI
    {
        int deliberationDelta = Utils.deliberationDelta;
        const int SPAWNGUARDS = 2;
        const int FORMATIONDISTANCE = 4;
        const int AIBODYGUARDS = 4;
        const int AIEXPLORERS = 1;

        List<BDI.Knowledge.Beliefs> beliefs = new List<Knowledge.Beliefs>();
        List<BDI.Knowledge.Desires> desires = new List<Knowledge.Desires>();
        Stack<BDI.Knowledge.Desires> intentions = new Stack<Knowledge.Desires>();

        List<Point> HoshimisAvailable = new List<Point>();
        List<Point> AZNPoints = new List<Point>();

        int deliberationWaitPeriods=0;

        public BDIAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }


        # region BELIEFS
        private void updateBeliefs()
        {
            beliefs.Clear();
            // UPDATE BELIEFS: Percept + Belief = Belief            

            //1. x protectors near spawn = safe spawn
            if (countSpawnProtectors() >= 2) { beliefs.Add(Knowledge.Beliefs.SafeSpawn); }            

            if (bodyGuardOK()) { beliefs.Add(Knowledge.Beliefs.SecureGuard); }

            if (bodySightOK()) { beliefs.Add(Knowledge.Beliefs.SecureSight); }

            if (getAASMAFramework().explorersAlive() > 5) { beliefs.Add(Knowledge.Beliefs.EnoughExploreEffort); }

            if (HoshimisAvailable.Count > 0) { beliefs.Add(Knowledge.Beliefs.AvailableHoshimi); }    

        }
        #endregion

        #region DELIBERATE
        private void deliberate()
        {
            desires.Clear();
            //   options -> Beliefs x Intentions = Desires            
            if (!beliefs.Contains(Knowledge.Beliefs.SecureGuard))
            {
                desires.Add(Knowledge.Desires.Guard);
            }
            if (!beliefs.Contains(Knowledge.Beliefs.SecureSight))
            {
                desires.Add(Knowledge.Desires.Sight);
            }
            if (HoshimisAvailable.Count > 0 && beliefs.Contains(Knowledge.Beliefs.SecureSight) && beliefs.Contains(Knowledge.Beliefs.SecureGuard))
            {
                desires.Add(Knowledge.Desires.BuildNeedle);
            }
            if (getAASMAFramework().needlesAlive() > 0 && beliefs.Contains(Knowledge.Beliefs.SafeSpawn))
            {
                desires.Add(Knowledge.Desires.BuildCollector);
            }
            if (beliefs.Contains(Knowledge.Beliefs.SafeSpawn) && !beliefs.Contains(Knowledge.Beliefs.EnoughExploreEffort))
            {
                desires.Add(Knowledge.Desires.BuildSoloExplorer);
            }
            if(beliefs.Contains(Knowledge.Beliefs.SecureGuard) && beliefs.Contains(Knowledge.Beliefs.SecureSight) && beliefs.Contains(Knowledge.Beliefs.AvailableHoshimi))
            {
                desires.Add(Knowledge.Desires.MoveToHoshimi);
            }            

            //   filter -> Beliefs x Desires x Intentions = Intentions
            intentions.Clear();
            if(desires.Contains(Knowledge.Desires.Guard)){
                intentions.Push(Knowledge.Desires.Guard);
            }
            if(desires.Contains(Knowledge.Desires.Sight)){
                intentions.Push(Knowledge.Desires.Sight);
            }
            if (desires.Contains(Knowledge.Desires.BuildSoloExplorer))
            {
                intentions.Push(Knowledge.Desires.BuildSoloExplorer);
            }

            // Go to needle or build if already there
            if (desires.Contains(Knowledge.Desires.MoveToHoshimi) && this._nanoAI.Location.Equals(HoshimisAvailable[0]))
            {
                intentions.Push(Knowledge.Desires.BuildNeedle);
            }
            else if(desires.Contains(Knowledge.Desires.MoveToHoshimi))
            {
                intentions.Push(Knowledge.Desires.MoveToHoshimi);
            }           

        }
        #endregion

        #region PLAN
        // REASONING:    Beliefs x Intentions x Actions = Plan
        // In this simple case we use beliefs to sort the intentions list which will
        // be executed by it's order.
        #endregion

        public override void DoActions()
        {
            // update beliefs and deliberate only each X rounds if AI still has intentions
            if (intentions.Count == 0 || deliberationWaitPeriods > deliberationDelta)
            {
                //getAASMAFramework().logData(this._nanoAI, "AI Deliberating...");
                updateBeliefs();
                deliberate();
                deliberationWaitPeriods = 0;
                HoshimisAvailable.Sort((x, y) => Utils.SquareDistance(this._nanoAI.Location, x).CompareTo(Utils.SquareDistance(this._nanoAI.Location, y)));
                this._nanoAI.StopMoving();
            }
            else
            {
                deliberationWaitPeriods++;
            }

            // Execute intentions
            if (intentions.Count > 0)
            {
                Knowledge.Desires intent = intentions.Pop();

                if (intent.Equals(Knowledge.Desires.Guard))
                {
                    this._nanoAI.Build(typeof(BDIBodyGuardProtector), "P" + this._protectorNumber++);
                }
                if (intent.Equals(Knowledge.Desires.Sight))
                {
                    this._nanoAI.Build(typeof(BDIBodyGuardExplorer), "E" + this._explorerNumber++);
                }
                if (intent.Equals(Knowledge.Desires.BuildSoloExplorer))
                {
                    this._nanoAI.Build(typeof(BDISoloExplorer), "E" + this._explorerNumber++);
                }
                if (intent.Equals(Knowledge.Desires.MoveToHoshimi))
                {
                    this._nanoAI.MoveTo(HoshimisAvailable[0]);
                }
                if (intent.Equals(Knowledge.Desires.BuildNeedle))
                {
                    this.HoshimisAvailable.Remove(this._nanoAI.Location);
                    this._nanoAI.Build(typeof(BDINeedle), "N" + this._needleNumber++);                   
                }
            }

        }

        public override void receiveMessage(AASMAMessage msg)
        {
            // MESSAGES ARE TREATED AS BELIEFS UPDATES
            // message about hoshimi found
            if (msg.Content.Contains("hoshimi"))
            {
                bool empty = true;
                Point location = (Point)msg.Tag;
                foreach (NanoBot bot in getAASMAFramework().NanoBots)
                {
                    if(bot.GetType().Equals(typeof(NanoNeedle)) && bot.Location.Equals(location))
                    {
                        empty = false;
                    }
                }
                if(empty && !HoshimisAvailable.Contains(location)){
                    this.HoshimisAvailable.Add((Point)msg.Tag);                    
                    getAASMAFramework().logData(this._nanoAI, "New hoshimi point at: " + location.X + "," + location.Y);
                }
            }

            //message about hoshimi unavailable = remove hoshimi
        }

        # region private functions

        private int countAliveBots(Type botType)
        {
            NanoBotCollection bots = getAASMAFramework().NanoBots;
            int numBots = 0;
            foreach (NanoBot bot in bots)
            {
                if (bot.GetType() == botType)
                {
                    numBots++;
                }
            }
            return numBots;
        }

        private int countSpawnProtectors()
        {
            int spawnprotectors = 0;
            foreach (NanoDefender defender in getAASMAFramework().NanoBots)
            {
                if (Utils.SquareDistance(getAASMAFramework().InjectionPoint, defender.Location) < FORMATIONDISTANCE)
                {
                    spawnprotectors++;
                }
            }
            return spawnprotectors;
        }

        private bool bodyGuardOK()
        {            
            int count = 0;
            foreach (NanoBot defender in getAASMAFramework().NanoBots)
            {
                if (defender.GetType() == typeof(BDIBodyGuardProtector))
                {                    
                        count++;
                }
            }
            if (count >= AIBODYGUARDS) { return true; }
            else { return false; }
        }

        private bool bodySightOK()
        {
            int count = 0;
            foreach (NanoBot explorer in getAASMAFramework().NanoBots)
            {
                if (explorer.GetType() == typeof(BDIBodyGuardExplorer))
                {                  
                        count++;                    
                }
            }
            if (count >= AIEXPLORERS) { return true; }
            else { return false; }
        }
        #endregion

    }
}

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
        const int deliberationDelta = 10;
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
            // UPDATE BELIEFS: Percept + Belief = Belief            

            //1. x protectors near spawn = safe spawn
            if (countSpawnProtectors() >= 2) { beliefs.Add(Knowledge.Beliefs.SafeSpawn); }            

            if (bodyGuardOK()) { beliefs.Add(Knowledge.Beliefs.SecureGuard); }

            if (bodySightOK()) { beliefs.Add(Knowledge.Beliefs.SecureSight); }

            if (getAASMAFramework().explorersAlive() > 5) { beliefs.Add(Knowledge.Beliefs.EnoughExploreEffort); }             

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
                     
            //   filter -> Beliefs x Desires x Intentions = Intentions
            if(desires.Contains(Knowledge.Desires.Guard)){
                intentions.Push(Knowledge.Desires.Guard);
            }
            if(desires.Contains(Knowledge.Desires.Sight)){
                intentions.Push(Knowledge.Desires.Sight);
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
                getAASMAFramework().logData(this._nanoAI, "AI Deliberating...");
                updateBeliefs();
                deliberate();
                deliberationWaitPeriods = 0;
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
                    this._nanoAI.Build(typeof(BDIExplorer), "E" + this._explorerNumber++);
                }
            }

        }

        public override void receiveMessage(AASMAMessage msg)
        {
            // MESSAGES ARE TREATED AS BELIEFS UPDATES
            // message about hoshimi = hoshimi


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
                    if (Utils.SquareDistance(getAASMAFramework().AI.Location, defender.Location) < FORMATIONDISTANCE)
                    {
                        count++;
                    }
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
                if (explorer.GetType() == typeof(BDIExplorer))
                {
                    if (Utils.SquareDistance(getAASMAFramework().AI.Location, explorer.Location) < FORMATIONDISTANCE)
                    {
                        count++;
                    }
                }
            }
            if (count >= AIEXPLORERS) { return true; }
            else { return false; }
        }
        #endregion

    }
}

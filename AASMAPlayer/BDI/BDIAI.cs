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
        const int SPAWNGUARDS = 2;
        const int FORMATIONDISTANCE = 4;
        const int AIBODYGUARDS = 4;
        const int AIEXPLORERS = 1;

        Dictionary<BDI.Knowledge.Beliefs, List<int>> beliefs = new Dictionary<Knowledge.Beliefs, List<int>>();
        List<BDI.Knowledge.Desires> desires = new List<Knowledge.Desires>();

        public BDIAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }


        # region BELIEFS
        private void updateBeliefs()
        {
            // UPDATE BELIEFS: Percept + Belief = Belief

            //1. x protectors near spawn = safe spawn
            if (countSpawnProtectors() >= 2) { beliefs[Knowledge.Beliefs.SafeSpawn] = new List<int> { 1 }; }
            else { beliefs[Knowledge.Beliefs.SafeSpawn] = new List<int> { 0 }; }

            if (bodyGuardOK()) { beliefs[Knowledge.Beliefs.SecureGuard] = new List<int> { 1 }; }
            else { beliefs[Knowledge.Beliefs.SecureGuard] = new List<int> { 0 }; }

            if (bodySightOK()) { beliefs[Knowledge.Beliefs.SecureSight] = new List<int> { 1 }; }
            else { beliefs[Knowledge.Beliefs.SecureSight] = new List<int> { 0 }; }

            if (getAASMAFramework().explorersAlive() > 5) { beliefs[Knowledge.Beliefs.EnoughExploreEffort] = new List<int> { 1 }; }
            else { beliefs[Knowledge.Beliefs.EnoughExploreEffort] = new List<int> { 0 }; }

        }
        #endregion

        #region DELIBERATE
        private void deliberate()
        {
            desires.Clear();
            //   options -> Beliefs x Intentions = Desires            
            if (beliefs[Knowledge.Beliefs.SecureGuard][0] == 1)
            {
                desires.Add(Knowledge.Desires.Guard);
            }
            if (beliefs[Knowledge.Beliefs.SecureSight][0] == 1)
            {
                desires.Add(Knowledge.Desires.Sight);
            }
            if (beliefs[Knowledge.Beliefs.AvailableHoshimiAt].Count > 0)
            {
                desires.Add(Knowledge.Desires.BuildNeedle);
            }
            if (beliefs[Knowledge.Beliefs.AvailableNeedleAt].Count > 0)
            {
                desires.Add(Knowledge.Desires.BuildCollector);
            }
            

         
            //   filter -> Beliefs x Desires x Intentions = Intentions


        }
        #endregion

        #region PLAN
        // REASONING:    Beliefs x Intentions x Actions = Plan
        #endregion

        public override void DoActions()
        {
            updateBeliefs();

            deliberate();


        }

        public override void receiveMessage(AASMAMessage msg)
        {
            // MESSAGES ARE TREATED AS BELIEFS UPDATES
            //message about hoshimi = hoshimi


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
            foreach (NanoDefender defender in getAASMAFramework().NanoBots)
            {
                if (Utils.SquareDistance(getAASMAFramework().AI.Location, defender.Location) < FORMATIONDISTANCE){
                    count++;
                }
            }
            if (count >= AIBODYGUARDS) { return true; }
            else { return false; }
        }

        private bool bodySightOK()
        {
            int count = 0;
            foreach (NanoExplorer explorer in getAASMAFramework().NanoBots)
            {
                if (Utils.SquareDistance(getAASMAFramework().AI.Location, explorer.Location) < FORMATIONDISTANCE)
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

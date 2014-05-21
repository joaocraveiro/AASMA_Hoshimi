using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class BDITeamExplorer : AASMAExplorer
    {
        int deliberationDelta = Utils.deliberationDelta;

        List<BDI.Knowledge.Beliefs> beliefs = new List<Knowledge.Beliefs>();
        List<BDI.Knowledge.Desires> desires = new List<Knowledge.Desires>();
        Stack<BDI.Knowledge.Desires> intentions = new Stack<Knowledge.Desires>();
        
        int deliberationWaitPeriods = 0;

        # region BELIEFS
        private void updateBeliefs()
        {           

        }
        #endregion

        #region DELIBERATE
        private void deliberate()
        {
            desires.Clear();
            //   options -> Beliefs x Intentions = Desires                        
        }
        #endregion

        public override void DoActions()
        {
            if (intentions.Count == 0 || deliberationWaitPeriods > deliberationDelta)
            {
                getAASMAFramework().logData(this, "Team Explorer Deliberating...");
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
                // EXECUTE               
            }
   
        }                    

        public override void receiveMessage(AASMAMessage msg)
        {
        }

    }
}

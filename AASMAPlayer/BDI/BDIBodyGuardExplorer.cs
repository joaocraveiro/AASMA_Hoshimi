using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class BDIBodyGuardExplorer: AASMAExplorer
    {
        public override void  DoActions()
        {
           
            // MOVE WITH THE TEAM
                this.MoveTo(getAASMAFramework().AI.Location);            
        }

        public override void receiveMessage(AASMAMessage msg)
        {
            // SEND MESSAGES ABOUT DANGER AND SHOOTING TARGETS
        }
        
    }
}

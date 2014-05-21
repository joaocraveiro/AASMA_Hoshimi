using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class BDIExplorer : AASMAExplorer
    {
        public override void DoActions()
        {            
            List<Point> points = getAASMAFramework().visibleNavigationPoints(this);
            if(points.Count > 0){
                this.MoveTo(points[Utils.randomValue(points.Count-1)]);                   
            }

                else if (frontClear())
                {
                    if (Utils.randomValue(100) < 80)
                    {
                        this.MoveForward();
                    }
                    else
                    {
                        this.RandomTurn();
                    }
                }
                else
                {
                    this.RandomTurn();
                }

            }                    

        public override void receiveMessage(AASMAMessage msg)
        {
        }

    }
}

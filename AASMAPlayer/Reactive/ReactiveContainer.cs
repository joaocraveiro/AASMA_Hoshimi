using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.Reactive
{
    //this is an example of a stupid collector that does not move, however if he is under an AZN point he will try to collect
    //something
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class ReactiveContainer : AASMAContainer
    {
        public override void DoActions()
        {
            List<Point> visibleAZN = getAASMAFramework().visibleAznPoints(this);
            List<Point> visibleEmptyNeedle = getAASMAFramework().visibleEmptyNeedles(this);

            if (Stock < ContainerCapacity && this.getAASMAFramework().overAZN(this))
            {
                this.collectAZN();
            }
            else if (Stock > 0 && this.getAASMAFramework().overEmptyNeedle(this))
            {
                this.transferAZN();
            }
            else if (Stock < ContainerCapacity && visibleAZN.Count > 0)
            {
                Point target = visibleAZN[0];
                this.MoveTo(target);
            }
            else if (Stock > 0 && visibleEmptyNeedle.Count > 0)
            {
                Point target = visibleEmptyNeedle[0];
                this.MoveTo(target);
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

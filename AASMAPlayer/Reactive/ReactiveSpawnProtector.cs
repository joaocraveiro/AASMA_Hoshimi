using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Reactive
{

    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 28)]
    public class ReactiveSpawnProtector : AASMAProtector
    {
        public override void  DoActions()
        {
            List<Point> enemies = getAASMAFramework().visiblePierres(this);
            Point enemyPosition;

            // SHOOT
            if (enemies.Count > 0)
            {

                enemyPosition = enemies[0];
                int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
                int sqrDistanceToEnemy = Utils.SquareDistance(this.Location,enemyPosition);
                //we need to test if the enemy is within firing distance.
                if (sqrDistanceToEnemy <= sqrDefenceDistance)
                {
                    //the defendTo commands fires to the specified position for a number of specified turns. 1 is the recommended number of turns.

                    this.DefendTo(enemyPosition, 1);
                }
            }    
        }

        public override void receiveMessage(AASMAMessage msg)
        {
        }

    }
}

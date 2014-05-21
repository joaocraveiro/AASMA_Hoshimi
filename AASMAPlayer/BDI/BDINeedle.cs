using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.BDI
{
    //a needle can't do much since it cannot move. However, it does have vision (so it can see enemies and other stuff),
    //can attack Pierre Bots and can send messages. 
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class BDINeedle : AASMANeedle
    {
        public override void DoActions()
        {
            List<Point> enemies = getAASMAFramework().visiblePierres(this);
            Point enemyPosition;

            // SHOOT
            if (enemies.Count > 0)
            {

                enemyPosition = enemies[0];
                int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
                int sqrDistanceToEnemy = Utils.SquareDistance(this.Location, enemyPosition);
                //we need to test if the enemy is within firing distance.
                if (sqrDistanceToEnemy <= sqrDefenceDistance)
                {
                    //the defendTo commands fires to the specified position for a number of specified turns. 1 is the recommended number of turns.
                    this.DefendTo(enemyPosition, 1);
                }
            }

            // SEND INFO
            /*if (getAASMAFramework().visibleHoshimies(this).Count > 0)
            {
                foreach (Point hoshimi in getAASMAFramework().visibleHoshimies(this))
                {
                    AASMAMessage msg = new AASMAMessage(this.InternalName, "hoshimi");
                    msg.Tag = hoshimi;
                    getAASMAFramework().sendMessage(msg, "AI");
                }
            }*/
        }

        public override void receiveMessage(AASMAMessage msg)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{    
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class BDISoloExplorer : AASMAExplorer
    {
        int deliberationDelta = Utils.deliberationDelta;

        List<BDI.Knowledge.Beliefs> beliefs = new List<Knowledge.Beliefs>();
        List<BDI.Knowledge.Desires> desires = new List<Knowledge.Desires>();
        Stack<BDI.Knowledge.Desires> intentions = new Stack<Knowledge.Desires>();        

        List<Point> visitedGoals = new List<Point>();
        List<Point> goals = new List<Point>();
        
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
            if (goals.Count > 0)
            {
                desires.Add(Knowledge.Desires.GoNavigationPoint);                
            }

            // FILTER
            intentions.Clear();
            if (desires.Contains(Knowledge.Desires.GoNavigationPoint))
            {                
                intentions.Push(Knowledge.Desires.GoNavigationPoint);                
            }
            //intentions.Push(Knowledge.Desires.Wander);
        }
        #endregion

        public override void DoActions()
        {
            updateNavigationPoints();

            if (intentions.Count == 0 || deliberationWaitPeriods > deliberationDelta)
            {                              
                updateBeliefs();
                deliberate();
                deliberationWaitPeriods = 0;
                this.StopMoving();
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
                if (intent == Knowledge.Desires.GoNavigationPoint)
                {
                    this.MoveTo(goals[0]);
                }              
            }
            else
            {
                wander();
            }

            // message handling
            if (getAASMAFramework().visibleHoshimies(this).Count > 0)
            {
                foreach (Point hoshimi in getAASMAFramework().visibleHoshimies(this))
                {
                    AASMAMessage msg = new AASMAMessage(this.InternalName, "hoshimi");
                    msg.Tag = hoshimi;
                    getAASMAFramework().sendMessage(msg, "AI");
                }
            }
   
        }                    

        public override void receiveMessage(AASMAMessage msg)
        {
        }

        #region private methods
        private void wander()
        {
            if (this.frontClear())
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

        private void updateNavigationPoints()
        {            
            List<Point> navPoints = getAASMAFramework().visibleNavigationPoints(this);            
            foreach (Point point in navPoints)
            {                
                if (!visitedGoals.Contains(point))
                {
                    goals.Add(point);
                }
            }
            //visited just now
            foreach (Point goal in goals)
            {
                if (goal.Equals(this.Location))
                {                    
                    visitedGoals.Add(goal);
                }
            }
            // remove visited ones
            foreach (Point goal in visitedGoals)
            {
                if(goals.Contains(goal)){
                goals.Remove(goal);
                }
            }
            goals.Sort((x, y) => Utils.SquareDistance(this.Location, x).CompareTo(Utils.SquareDistance(this.Location, y)));       
        }

        #endregion

    }
}

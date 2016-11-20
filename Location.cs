using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace locationSpace
{
     class Location
    {
       public int locId;
       public string locName;
       public int xcord, ycord;
       public Location() { xcord = ycord = 0; }

        public Location(int locid, string lname, int xcord, int ycord)//constructor
        {
            locId = locid;
            locName = lname;
            this.xcord = xcord;
            this.ycord = ycord;
        }
        Point getPoint()
        {
            Point p = new Point(xcord, ycord);
            return p;
        }
        public string toString()
        {
            return locId + "  " + locName + " " + xcord + " " + ycord;
        }
        public string display(Location []locArray,int count)
        {
            string str=" ";
            for(int i=0;i<count;i++)
                str=str+locArray[i].toString()+"\n";
            return str;
 
        }
        public double getWeightBetweenNode(Location l1, Location l2)
        {
            return Math.Sqrt(Math.Pow(l1.xcord - l2.xcord, 2) + Math.Pow(l1.ycord - l2.ycord, 2));
        }
         
    }

}

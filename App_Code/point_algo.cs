/** 
Written by: Nathan Del Carmen, Jonathan Yang
Tested by: Nathan Del Carmen, Jonathan Yang
Debugged by: Nathan Del Carmen, Jonathan Yang
Purpose:  Apply point algorithm in search bar.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Creddit
{
    public class point_algo
    {
        public float point_worth(float times_voted)
        {
            float pointworth;
            if (times_voted <= 10)
            {
                return pointworth = (float)1.0;
            }
            if (times_voted > 55)
            {
                return pointworth = (float).15;
            }
            else
            {
                pointworth = (float)Math.Exp( -(times_voted/100));
                return pointworth;
            }
        }
    }
}
/** 
Written by: Nathan Del Carmen, Ka Wai Chu
Tested by: Nathan Del Carmen, Ka Wai Chu
Debugged by: Nathan Del Carmen, Ka Wai Chu
Purpose:  Check for duplicates in search bar.
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Creddit
{
    public class Duplicates
    {
        public void Duplicate(List<string> forum)
        {
            for (int i = 0; i < forum.Count; ++i)
            {
                for (int j = i + 1; j < forum.Count; ++j)
                {
                    if (forum[i] == forum[j])
                    {
                        forum.RemoveAt(j);
                        j--;
                    }
                }
            }
            return;
        }

    }
}
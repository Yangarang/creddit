/** 
Written by: Jonathan Yang
Tested by: Jonathan Yang
Debugged by: Jonathan Yang
Purpose: To display newsfeed of forum.
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Creddit
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if user chooses a specific post, redirect to thread.aspx with forum id
            int index = GridView1.SelectedIndex;
            String forumId = GridView1.DataKeys[index].Value.ToString();
            String redirect = "Thread.aspx?t=" + forumId;
            Response.Redirect(redirect);
        }
    }
}
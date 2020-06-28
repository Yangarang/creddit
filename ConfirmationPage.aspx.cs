/** 
Written by: Kyle Clark, Elizabeth Chao
Tested by: Kyle Clark, Elizabeth Chao
Debugged by: Kyle Clark, Elizabeth Chao
Purpose: To redirect when confirming registering for tutoring.
**/

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using Creddit.Models;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Creddit
{
    public partial class ConfirmationPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accountID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx?m=LoginError");
            }
            int varindex = 0;
            int index = 0;

            Uri myUrl = Request.Url;
            string query = myUrl.PathAndQuery;
            varindex = query.IndexOf("?");
            string parse = query.Substring(varindex + 1);
            index = parse.IndexOf("&");
            string tutor = parse.Substring(0, index);
            string course = parse.Substring(index + 1);
            if (course.IndexOf("%") >= 0)
            {
                course = course.Replace("%20", " ");
            }
            if (course.IndexOf("+") >= 0)
            {
                course = course.Replace("+", " ");
            }
            if (tutor.IndexOf("%") >= 0)
            {
                tutor = tutor.Replace("%20", " ");
            }
            if (tutor.IndexOf("+") >= 0)
            {
                tutor = tutor.Replace("+", " ");
            }


            Label2.Text = "You are now registered for a tutoring session for " + course + " with " + tutor + ".";


            MySql.Data.MySqlClient.MySqlConnection conn;
            MySql.Data.MySqlClient.MySqlCommand cmd;
            String queryStr;
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);

            queryStr = "UPDATE account SET registeredTutor = '" + tutor + "', registeredCourse = '" + course + "' WHERE accountId =" + Convert.ToInt32(Session["accountID"]);
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);

            conn.Open();
            cmd.ExecuteReader();
            conn.Close();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String redirect = "Tutor.aspx";
            Response.Redirect(redirect);
        }
    }
}


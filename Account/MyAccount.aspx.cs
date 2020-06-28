/** 
Written by: Jonathan Yang
Tested by: Jonathan Yang
Debugged by: Jonathan Yang
Purpose:  Allow user to access own account information.
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Creddit
{
    public partial class MyAccount : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accountID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx?m=LoginError");
            }
            updateMyPoints();
            NameLabel.Text = (String)Session["name"];
            AccountIDLabel.Text = (String)Session["accountID"];
            EmailLabel.Text = (String)Session["email"];
            TypeLabel.Text = (String)Session["accountType"];
            PointLabel.Text = Session["point"].ToString();
            DegreeLabel.Text = (String)Session["degree"];
            SchoolLabel.Text = (String)Session["school"];
        }
        private void updateMyPoints()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            MySql.Data.MySqlClient.MySqlCommand cmd;
            MySql.Data.MySqlClient.MySqlDataReader reader;
            String queryStr;
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "";
            queryStr = "SELECT * FROM account WHERE accountId='" + (String)Session["accountID"] + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            int point = 0;
            while (reader.HasRows && reader.Read())
            {
                point = Int32.Parse(reader.GetString(reader.GetOrdinal("point")));
            }
            if (reader.HasRows)
            {
                Session["point"] = point;
            }
            reader.Close();
            conn.Close();
        }
    }
     
}
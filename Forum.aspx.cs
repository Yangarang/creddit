/** 
Written by: Jonathan Yang
Tested by: Jonathan Yang
Debugged by: Jonathan Yang
Purpose: To display recently posted topics in forum.
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

    public partial class Forum : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if user is logged in, allow them to post in forum. Else do not let them post in forum.
            if (Session["accountID"] == null)
            {
                PostButton.Visible = false;
                PostText.Visible = false;
                ForumLabel.Text = "You must be logged in to post in forum!";
            }
            else
            {
                PostButton.Visible = true;
                PostText.Visible = true;
                ForumLabel.Text = "Posting as " + (String)Session["name"] + "(" + (String)Session["accountType"] + "):";
            }
            GridView1.DataBind();
        }
        
        protected void PostButton_Click1(object sender, EventArgs e)
        {
            //post into forum by adding the post to the forum table of database
            try
            {
                ErrorLabel.Visible = false;
                // get the session states of post, name, account ID, account type, date time and input to strings
                string post = PostText.Text;
                string name = (String)Session["name"];
                string accountID = (String)Session["accountID"];
                string accountType = (String)Session["accountType"];
                DateTime datetime = DateTime.Now;
                string sqldatetime = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                
                // connect to the MySql Database
                MySql.Data.MySqlClient.MySqlConnection conn;
                String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();

                // declare the MySql command that will run which inserts the post and the above informations to the post
                /*fixed for SQL injection */
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO forum(post, name, dateTime, accountId, accountType) VALUES(?post,?name,?dateTime,?accountId,?accountType)";
                cmd.Parameters.Add("?post", MySqlDbType.VarChar).Value = post;
                cmd.Parameters.Add("?name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("?dateTime", MySqlDbType.VarChar).Value = sqldatetime;
                cmd.Parameters.Add("?accountId", MySqlDbType.VarChar).Value = accountID;
                cmd.Parameters.Add("?accountType", MySqlDbType.VarChar).Value = accountType;
                
                // execute the MySql command
                cmd.ExecuteNonQuery();

                // close the connection and tell user of success
                conn.Close();
                successLabel.Text = "Post was successful!";
                successLabel.Visible = true;
                ErrorLabel.Visible = false;
            }
            catch
            {
                // if post cannot be inserted to the forum, let user know of error
                ErrorLabel.Visible = true;
                successLabel.Visible = false;
                ErrorLabel.Text = "Error posting in forum, please try again...";
            }
            // update the gridview
            PostText.Text = "";
            GridView1.DataBind();
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
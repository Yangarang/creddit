/** 
Written by: Jonathan Yang, Nathan Del Carmen
Tested by: Jonathan Yang
Debugged by: Jonathan Yang
Purpose: To display comments of post in forum. Also allows upvote and downvote capabilities.
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
    public partial class Thread : System.Web.UI.Page
    {
        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        String queryStr;
        String forumId;
        String threadId;
        protected void Page_Load(object sender, EventArgs e)
        {
               
            // if user is not logged in, do not allow them to post a comment and upvote/downvote. 
            if (Session["accountID"] == null)
            {
                PostButton.Visible = false;
                PostText.Visible = false;
                ForumLabel.Text = "You must be logged in to comment and vote in forum!";
                UpvoteForumButton.Visible = false;
                DownvoteForumButton.Visible = false;
                GridView1.Columns[0].Visible = false;
            }
            //else if user is logged in,  let them post a comment and upvote/downvote.
            else
            {
                PostButton.Visible = true;
                PostText.Visible = true;
                ForumLabel.Text = "Posting as " + (String)Session["name"] + "(" + (String)Session["accountType"] + "):";
            }

            // get forum id from the session state passed in from forum.aspx page
            forumId = (String)Request.QueryString["t"];
            Session["forumId"] = forumId;
            // if forum id is not set, go back to forum page
            if (forumId == null)
            {
                Response.Redirect("Forum.aspx");
            }
            // else print out the forum post, the comments, and points.
            else
            {
                printForumPost();
                getPointCount();
                checkVoted();
            }
        }
        private void printForumPost()
        {
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "";
            queryStr = "SELECT * FROM forum WHERE forumId='" + forumId + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            String post = "";
            String name = "";
            int accountID = 0;
            String dateTime = "";
            String accountType = "";
            while (reader.HasRows && reader.Read())
            {
                post = reader.GetString(reader.GetOrdinal("post"));
                name = reader.GetString(reader.GetOrdinal("name"));
                dateTime = reader.GetString(reader.GetOrdinal("dateTime"));
                accountID = Int32.Parse(reader.GetString(reader.GetOrdinal("accountId")));
                accountType = reader.GetString(reader.GetOrdinal("accountType"));
            }
            PostLabel.Text = post;
            NameLabel.Text = name;
            datetimeLabel.Text = dateTime;
            TypeLabel.Text = accountType;
            reader.Close();
            conn.Close();
        }

        protected void PostButton_Click1(object sender, EventArgs e)
        {
            try
            {
                ErrorLabel.Visible = false;
                string comment = PostText.Text;
                string name = (String)Session["name"];
                string accountID = (String)Session["accountID"];
                string accountType = (String)Session["accountType"];
                DateTime datetime = DateTime.Now;
                string sqldatetime = datetime.ToString("yyyy-MM-dd HH:mm:ss");

                MySql.Data.MySqlClient.MySqlConnection conn;

                String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();

                /*fixed for SQL injection */
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO thread(comment, name, dateTime, accountId, accountType, forumId) VALUES(?comment,?name,?dateTime,?accountId,?accountType,?forumId)";
                cmd.Parameters.Add("?comment", MySqlDbType.VarChar).Value = comment;
                cmd.Parameters.Add("?name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("?dateTime", MySqlDbType.VarChar).Value = sqldatetime;
                cmd.Parameters.Add("?accountId", MySqlDbType.VarChar).Value = accountID;
                cmd.Parameters.Add("?accountType", MySqlDbType.VarChar).Value = accountType;
                cmd.Parameters.Add("?forumId", MySqlDbType.VarChar).Value = (String)Session["forumId"];

                cmd.ExecuteNonQuery();

                conn.Close();
                successLabel.Text = "Posting comment was successful!";
                successLabel.Visible = true;
                ErrorLabel.Visible = false;
            }
            catch
            {
                ErrorLabel.Visible = true;
                successLabel.Visible = false;
                ErrorLabel.Text = "Error posting comment, please try again...";
            }
            PostText.Text = "";
            int intforumId = Convert.ToInt32(forumId);
            GridView1.DataBind();
            checkVoted();
        }

        protected void returnForumButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        private void getPointCount()
        {
            int points = 0;
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "SELECT * FROM forum WHERE forumId = '" + forumId + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                points = Int32.Parse(reader.GetString(reader.GetOrdinal("point")));
            }
            conn.Close();
            PointCount.Text = points.ToString();
        }
        protected void UpvoteForumButton_Click(object sender, ImageClickEventArgs e)
        {
            ErrorLabel.Visible = false;
            votePost("+1", "forum", forumId);
            votesuccess.Text = "Upvote successful!";
            votesuccess.Visible = true;
            UpvoteForumButton.Visible = false;
            DownvoteForumButton.Visible = false;

        }

        protected void DownvoteForumButton_Click(object sender, ImageClickEventArgs e)
        {     
            ErrorLabel.Visible = false;
            votePost("-1", "forum", forumId);
            votesuccess.Text = "Downvote successful!";
            votesuccess.Visible = true;
            UpvoteForumButton.Visible = false;
            DownvoteForumButton.Visible = false;
        }
        private void votePost(String vote, String TypeID, String ID)
        {
            /*looks for accountID*/
            String accountID = "";
            String myaccountID = (String)Session["accountID"];
            MySql.Data.MySqlClient.MySqlConnection conn;
            MySql.Data.MySqlClient.MySqlCommand cmd;
            String queryStr;
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "SELECT * FROM " + TypeID + " WHERE " + TypeID + "Id = '" + ID + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                accountID = reader.GetString(reader.GetOrdinal("accountId"));
            }
            conn.Close();

            conn.Open();
            queryStr = "";
            queryStr = "UPDATE account SET point = point" + vote + " WHERE accountId = " + accountID;
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            cmd.ExecuteReader();
            conn.Close();

            conn.Open();
            queryStr = "";
            queryStr = "UPDATE " + TypeID +" SET point = point" + vote + " WHERE " + TypeID + "Id = " + ID;
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            cmd.ExecuteReader();
            conn.Close();

            updatePointWell(vote, accountID);
            getPointCount();
            updateMyPoints();
            addVote(TypeID, ID);
        }

        //nathan's point stuff
        private void updatePointWell(String vote, String accountID)
        {
            string myaccountID = (String)Session["accountID"];
            int votecount = 0;
            conn.Open();
            //CHECK THIS
            queryStr = "SELECT votecount FROM account where accountId = '" + myaccountID + "'";
            using (cmd = new MySqlCommand(queryStr, conn))
            {
                votecount = Convert.ToInt32(cmd.ExecuteScalar());

            }
            conn.Close();

            //INCREASE THE VOTECOUNT
            conn.Open();
            queryStr = "";
            queryStr = "UPDATE account SET votecount = votecount+1 WHERE accountId = " + myaccountID;
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            cmd.ExecuteReader();
            conn.Close();

            point_algo pa = new point_algo();
            float pointworth = pa.point_worth(votecount);
            /*fixed for SQL injection */
            //ADD POINT WORTH INTO THE POINT WELL (well drys up if it isnt used, the decay... pretty good right? :D
            if (vote == "+1")
            {
                //CHECK  THIS (upvote)
                conn.Open();
                MySqlCommand cmddd = conn.CreateCommand();
                queryStr = "";
                queryStr = "UPDATE account SET pointwell = pointwell +'" + pointworth + "' WHERE accountId = " + accountID;
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                cmd.ExecuteReader();
                conn.Close();
            }
            else
            {
                //DOWNVOTE
                conn.Open();
                MySqlCommand cmddd = conn.CreateCommand();
                queryStr = "";
                queryStr = "UPDATE account SET pointwell = pointwell -'" + pointworth + "' WHERE accountId = " + accountID;
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                cmd.ExecuteReader();
                conn.Close();
            }

        }

        private void updateMyPoints()
        {
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "";
            queryStr = "SELECT * FROM account WHERE accountId='" + (String)Session["accountId"] + "'";
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
 
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = GridView1.SelectedIndex;
            threadId = GridView1.DataKeys[index].Value.ToString();
            //threadId = "7";
            if ((String)Session["choose"] == "upvote")
            {
                votePost("+1", "thread", threadId);
                GridViewRow row = GridView1.SelectedRow;
                int update = Convert.ToInt32(row.Cells[2].Text);
                update++;
                row.Cells[2].Text = update.ToString();
                row.Cells[0].Text = "Already Voted";
            }
            else if ((String)Session["choose"] == "downvote")
            {
                votePost("-1", "thread", threadId);
                GridViewRow row = GridView1.SelectedRow;
                int update = Convert.ToInt32(row.Cells[2].Text);
                update--;
                row.Cells[2].Text = update.ToString();
                row.Cells[0].Text = "Already Voted";
            }       
        }

        protected void UpvoteCommentButton_Click(object sender, ImageClickEventArgs e)
        {
            votesuccess.Text = "Upvote comment successful!";
            Session["choose"] = "upvote";
            votesuccess.Visible = true;
        }

        protected void DownvoteCommentButton_Click(object sender, ImageClickEventArgs e)
        {
            votesuccess.Text = "Downvote comment successful!";
            Session["choose"] = "downvote";
            votesuccess.Visible = true;
        }

        //check in initiate if user already voted any posts
        private void checkVoted()
        {
            String accountID = (String)Session["accountID"];
            //check forum post voted already
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "";
            queryStr = "SELECT * FROM forum WHERE forumId='" + forumId + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            String vote = "";
            while (reader.HasRows && reader.Read())
            {
                vote = reader.GetString(reader.GetOrdinal("vote"));
            }
            reader.Close();
            conn.Close();
            // if user already voted on this forum, hide upvote/downvote
            if (vote.Contains("|" + accountID + "|"))
            {
                UpvoteForumButton.Visible = false;
                DownvoteForumButton.Visible = false;
            }
                
            //check comment post voted already
            foreach (GridViewRow row in GridView1.Rows)
            {
                threadId = row.Cells[7].Text;
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();
                queryStr = "";
                queryStr = "SELECT * FROM thread WHERE threadId='" + threadId + "'";
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                reader = cmd.ExecuteReader();
                vote = "";
                while (reader.HasRows && reader.Read())
                {
                    vote = reader.GetString(reader.GetOrdinal("vote"));
                }
                reader.Close();
                conn.Close();
                // if user already voted on this comment, hide upvote/downvote
                if (vote.Contains("|" + accountID + "|"))
                {
                    row.Cells[0].Text = "Already Voted";
                }
            }
        }
        private void addVote(String TypeID, String ID)
        {
            /*looks for accountID*/
            String myaccountID = (String)Session["accountID"];
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "SELECT * FROM " + TypeID + " WHERE " + TypeID + "Id = '" + ID + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            String vote = "";
            while (reader.HasRows && reader.Read())
            {
                vote = reader.GetString(reader.GetOrdinal("vote"));
            }
            String updatevote = vote + "|" + myaccountID + "|";
            conn.Close();

            conn.Open();
            queryStr = "";
            queryStr = "UPDATE " + TypeID + " SET vote = '" + updatevote + "' WHERE " + TypeID + "Id = '" + ID + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            cmd.ExecuteReader();
            conn.Close();
        }
    }
}
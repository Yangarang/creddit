/** 
Written by: Kyle Clark, Elizabeth Chao
Tested by: Kyle Clark, Elizabeth Chao
Debugged by: Kyle Clark, Elizabeth Chao
Purpose: Page which displays tutor information, reviews, and registeration times/dates.
**/

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
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
    public partial class TutorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accountID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx?m=LoginError");
            }
            int varindex = 0;
            int index = 0;
            string firstname = "";
            string lastname = "";
            double rating = 0;
            int rowcount = 0;
            String registeredTutor = (String)Session["registeredTutor"];
            String registeredCourse = (String)Session["registeredCourse"];

            Uri myUrl = Request.Url;
            string query = myUrl.PathAndQuery;
            varindex = query.IndexOf("=");
            string parse = query.Substring(varindex+1);
            if (parse.IndexOf("%") >= 0)
            {
                index = parse.IndexOf("%");
                firstname = parse.Substring(0, index);
                lastname = parse.Substring(index + 3);
            }
            if (parse.IndexOf("+") >= 0)
            {
                index = parse.IndexOf("+");
                firstname = parse.Substring(0, index);
                lastname = parse.Substring(index + 1);
            }

            string tutor_name = firstname + " " + lastname;
            Session["tutor_name"] = tutor_name;
            if (tutor_name.Equals(Session["name"]))
            {
                Response.Redirect("~/Account/MyAccount.aspx");
            }

            MySql.Data.MySqlClient.MySqlConnection conn;
            MySql.Data.MySqlClient.MySqlCommand cmd1;
            MySql.Data.MySqlClient.MySqlCommand cmd2;
            MySql.Data.MySqlClient.MySqlCommand cmd3;
            MySql.Data.MySqlClient.MySqlDataReader reader1;
            MySql.Data.MySqlClient.MySqlDataReader reader2;
            MySql.Data.MySqlClient.MySqlDataReader reader3;
            String queryStr1;
            String queryStr2;
            String queryStr3;
            DateTime now = DateTime.Now;
            String sqlnow = now.ToString("yyyy-MM-dd HH:mm:ss");
            Session["now"] = sqlnow;

            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);

            queryStr1 = "SELECT accountId, course_name, course_id FROM tutor WHERE name LIKE '%" + tutor_name + "%' ";
            cmd1 = new MySql.Data.MySqlClient.MySqlCommand(queryStr1, conn);

            conn.Open();
            reader1 = cmd1.ExecuteReader();
            String course_name = "";
            int course_id = 0;
            int accountId = 0;
            while (reader1.HasRows && reader1.Read())
            {
                course_name = reader1.GetString(reader1.GetOrdinal("course_name"));
                accountId = Int32.Parse(reader1.GetString(reader1.GetOrdinal("accountId")));
                course_id = Int32.Parse(reader1.GetString(reader1.GetOrdinal("course_id")));
            }
            if (reader1.HasRows)
            {
                Session["course_name"] = course_name;
                Session["course_id"] = course_id;
                Session["tutorId"] = accountId;
            }
            reader1.Close();

            queryStr2 = "SELECT * FROM reviews WHERE accountId LIKE '%" + accountId + "%' ";
            cmd2 = new MySql.Data.MySqlClient.MySqlCommand(queryStr2, conn);
            reader2 = cmd2.ExecuteReader();
            while (reader2.HasRows && reader2.Read())
            {
                rating = rating + Int32.Parse(reader2.GetString(reader2.GetOrdinal("rating")));
                rowcount++;
            }
            if (tutor_name.Equals(registeredTutor) && course_name.Equals(registeredCourse))
            {
                Label8.Visible = true;
                Label10.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Button3.Visible = true;
            }
            reader2.Close();
            queryStr3 = String.Format("SELECT * FROM time WHERE time > '{0}' AND course_id LIKE '{1}'", sqlnow, course_id);
            cmd3 = new MySql.Data.MySqlClient.MySqlCommand(queryStr3, conn);
            reader3 = cmd3.ExecuteReader();
            if (reader3.HasRows == true)
            {
                Button2.Visible = true;
                Label7.Visible = true;
                Label6.Visible = true;
                Label9.Visible = false;
            }
            reader3.Close();
            conn.Close();

            rating = rating / rowcount;
            rating = Math.Round(rating, 1);
            Label2.Text = tutor_name;
            Label5.Text = rating.ToString();
            Label4.Text = course_name;
            GridView1.DataBind();
            RadioButtonList1.DataBind();

         
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String redirect = "Tutor.aspx";
            Response.Redirect(redirect);
        }


        protected void Button2_Click(object sender, EventArgs e)
        {
            String redirect = String.Format("ConfirmationPage.aspx?{0}&{1}", Session["tutor_name"], Session["course_name"]);
            Response.Redirect(redirect);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            int rating = 0;
            if (Convert.ToInt32(TextBox2.Text) <= 5 && Convert.ToInt32(TextBox2.Text) >= 1)
            {
                try
                {
                    string newreview = TextBox1.Text;
                    int accountId = (Int32)Session["tutorId"];
                    rating = Convert.ToInt32(TextBox2.Text);

                    MySql.Data.MySqlClient.MySqlConnection conn;

                    String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                    conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                    conn.Open();

                    /*fixed for SQL injection */
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO reviews(reviews, accountId, rating) VALUES(?reviews,?accountId,?rating)";
                    cmd.Parameters.Add("?reviews", MySqlDbType.VarChar).Value = newreview;
                    cmd.Parameters.Add("?accountId", MySqlDbType.VarChar).Value = accountId;
                    cmd.Parameters.Add("?rating", MySqlDbType.VarChar).Value = rating;

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    SuccessLabel.Text = "Review post was successful!";
                    SuccessLabel.Visible = true;
                    ErrorLabel.Visible = false;
                }
                catch
                {
                    ErrorLabel.Visible = true;
                    SuccessLabel.Visible = false;
                    ErrorLabel.Text = "Error posting the review, please try again...";
                }
                GridView1.DataBind();
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                ErrorLabel.Visible = true;
                SuccessLabel.Visible = false;
                ErrorLabel.Text = "That is an invalid rating, please try again...";
            }
        }

    }
}
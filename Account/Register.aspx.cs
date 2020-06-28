/** 
Written by: Kyujin Kim, Daniel Lee
Tested by: Kyujin Kim, Daniel Lee
Debugged by: Kyujin Kim, Daniel Lee
Purpose:  Allow user to register an account.
**/

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Creddit.Models;
using System.Net.Mail;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace Creddit.Account
{
    public partial class Register : Page
    {
        MySql.Data.MySqlClient.MySqlConnection conn;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accountID"] != null)
            {
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            /** 
             username: creddituser
             password: creddit
             **/
            if (IsValidEmail(Email.Text) != true)
            {
                ErrorMessage.Text = "Not valid email!";
            }
            else
            {
                try
                {
                Random rnd = new Random();
                int code = rnd.Next(1000000);
                String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();

                // declare the MySql command that will run which inserts the post and the above informations to the post
                string name = Name.Text;
                string email = Email.Text;
                string school = School.Text;
                string degree = Degree.Text;
                string password = Password.Text;
                string accounttype = accountType.Text;

                /*fixed for SQL injection */
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO account(name, email, school, degree, password, accountType, code) VALUES(?name,?email,?school,?degree,?password,?accountType,?code)";
                cmd.Parameters.Add("?name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("?school", MySqlDbType.VarChar).Value = school;
                cmd.Parameters.Add("?degree", MySqlDbType.VarChar).Value = degree;
                cmd.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;
                cmd.Parameters.Add("?accountType", MySqlDbType.VarChar).Value = accounttype;
                cmd.Parameters.Add("?code", MySqlDbType.Int32).Value = code;
                // execute the MySql command
                cmd.ExecuteNonQuery();

                conn.Close();
                if (accounttype == "Student")
                {
                    sendEmail(code);
                    Response.Redirect("~/Account/Login.aspx?m=RegSuccess");
                }
                else if (accounttype == "Faculty")
                {
                    sendFaculty();
                    Response.Redirect("~/Account/Login.aspx");
                }
                }
                catch
                {
                    ErrorMessage.Text = "Email already in use.";
                }
            }

        }
        private void sendEmail(int code)
        {
            MailMessage mail = new MailMessage("credditSE@gmail.com", Email.Text, "CREDDIT Confirmation Email", "Your Verification Code for CREDDIT is: " + code.ToString());
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new System.Net.NetworkCredential("credditSE@gmail.com", "takeaguess");
            client.EnableSsl = true;
            client.Send(mail);
        }
        private void sendFaculty()
        {
            MailMessage mail = new MailMessage("credditSE@gmail.com", Email.Text, "CREDDIT Faculty Verification Email", "Please send a proof of employment at your University (i.e. Paystub, Certificate, Faculty Identification Card, etc.) to credditSE@gmail.com with the Subject 'Faculty Verification'. Your account will be reviewed by a CREDDIT administrator.");
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new System.Net.NetworkCredential("credditSE@gmail.com", "takeaguess");
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}
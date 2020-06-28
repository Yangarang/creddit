/** 
Written by: Kyujin Kim, Daniel Lee
Tested by: Kyujin Kim, Daniel Lee
Debugged by: Kyujin Kim, Daniel Lee
Purpose:  Allow user to log in.
**/

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using Creddit.Models;
using System.Net.Mail;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Creddit.Account
{
    public partial class Login : Page
    {

        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        String queryStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
            var message = Request.QueryString["m"];
            if (message == "RegSuccess")
            {
                successMessage.Text = "Account successfully created! Welcome to Creddit! Please Log In.";
                successMessage.Visible = true;
                //go to the verify page
                EmailVerifier.Visible = true;
                EmailVerifierLabel.Visible = true;
            }
            if (message == "LoginError")
            {
                FailureText.Text = "You must be logged in to access that page!";
                ErrorMessage.Visible = true;
            }
            if (Session["accountID"] != null)
            {
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {

                String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();
                queryStr = "";
                queryStr = "SELECT * FROM account WHERE email='" + Email.Text + "' AND password='" + Password.Text + "'";
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                reader = cmd.ExecuteReader();
                String name = "";
                String accountID = "";
                String email = "";
                String accountType = "";
                int code = 0;
                int emailVerified = 3;
                int point = 0;
                String registeredTutor = "";
                String registeredCourse = "";
                String faculty = "";
                while (reader.HasRows && reader.Read())
                {
                    accountID = reader.GetString(reader.GetOrdinal("accountId"));
                    code = Int32.Parse(reader.GetString(reader.GetOrdinal("code")));
                    accountType = reader.GetString(reader.GetOrdinal("accountType"));
                    emailVerified = Int32.Parse(reader.GetString(reader.GetOrdinal("EmailVerified")));
                    faculty = reader.GetString(reader.GetOrdinal("faculty"));
                }
                if (accountType == "Faculty" && faculty != "verified")
                {                        
                        FailureText.Text = "Your Faculty account is under review. Please check your email and wait 2~3 business days.";
                        ErrorMessage.Visible = true;
                        return;
                }
                conn.Close();
                if (emailVerified == 0 && accountType == "Student")
                {
                    //go to the verify page
                    EmailVerifier.Visible = true;
                    EmailVerifierLabel.Visible = true;

                    if (EmailVerifier.Text == code.ToString())
                    {
                        emailVerified = 1;
                        conn.Open();
                        queryStr = "UPDATE account SET EmailVerified = EmailVerified+1 WHERE accountId = " + accountID;
                        cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                        reader = cmd.ExecuteReader();
                        FailureText.Text = "";
                        EmailVerifier.Visible = false;
                        EmailVerifierLabel.Visible = false;
                        conn.Close();
                    }
                    else
                    {
                        ErrorMessage.Visible = true;
                        FailureText.Text = "Please enter correct verification code.";
                        return;
                    }
                }
                try
                {
                    connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                    conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                    conn.Open();
                    queryStr = "";
                    queryStr = "SELECT * FROM account WHERE email='" + Email.Text + "' AND password='" + Password.Text + "'";
                    cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
                    reader = cmd.ExecuteReader();
                    name = "";
                    accountID = "";
                    email = "";
                    accountType = "";
                    String degree = "";
                    String school = "";
                    String becomeTutor = "";
                    faculty = "";
                    float pointWell = 0;
                    point = 0;
                    registeredTutor = "";
                    registeredCourse = "";
                    while (reader.HasRows && reader.Read())
                    {
                        name = reader.GetString(reader.GetOrdinal("name"));
                        accountID = reader.GetString(reader.GetOrdinal("accountId"));
                        email = reader.GetString(reader.GetOrdinal("email"));
                        accountType = reader.GetString(reader.GetOrdinal("accountType"));
                        degree = reader.GetString(reader.GetOrdinal("degree"));
                        school = reader.GetString(reader.GetOrdinal("school"));
                        point = Int32.Parse(reader.GetString(reader.GetOrdinal("point")));
                        becomeTutor = reader.GetString(reader.GetOrdinal("becometutor"));
                        pointWell = float.Parse(reader.GetString(reader.GetOrdinal("pointwell")));
                        faculty = reader.GetString(reader.GetOrdinal("faculty"));
                        registeredTutor = reader.GetString(reader.GetOrdinal("registeredTutor"));
                        registeredCourse = reader.GetString(reader.GetOrdinal("registeredCourse"));
                    }

                    if (reader.HasRows)
                    {
                        Session["name"] = name;
                        Session["accountID"] = accountID;
                        Session["email"] = email;
                        Session["accountType"] = accountType;
                        Session["point"] = point;
                        Session["degree"] = degree;
                        Session["school"] = school;
                        Session["registeredTutor"] = registeredTutor;
                        Session["registeredCourse"] = registeredCourse;
                        Response.BufferOutput = true;
                        reader.Close();
                        conn.Close();

                        if (pointWell > 50 & becomeTutor != "sent")
                        {
                            sendEmail(email);
                        }

                        FormsAuthentication.RedirectFromLoginPage(accountID, true);
                        FormsAuthentication.SetAuthCookie(accountID, true);
                        var manager = new UserManager();
                        var user = new ApplicationUser();
                        IdentityHelper.SignIn(manager, user, isPersistent: false);
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        return;
                    }
                    else
                    {
                        FailureText.Text = "Incorrect username or password.";
                        ErrorMessage.Visible = true;
                        reader.Close();
                        conn.Close();
                    }
                }
                catch
                {
                    FailureText.Text = "Error occured - please try again.";
                    ErrorMessage.Visible = true;
                }
            }
        }
        private void sendEmail(String email)
        {
            MailMessage mail = new MailMessage("credditSE@gmail.com", email, "Become a CREDDIT Tutor TODAY!", "Congratualations! You can sign up to become a tutor in CREDDIT and continue to help the community! Please visit the tutor page at www.creddi.com");
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new System.Net.NetworkCredential("credditSE@gmail.com", "takeaguess");
            client.EnableSsl = true;
            client.Send(mail);
            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            queryStr = "UPDATE account SET becometutor = 'sent' WHERE email = '" + email + "'";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);
            reader = cmd.ExecuteReader();
            conn.Close();
        }
    }
}
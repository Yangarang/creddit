/** 
Written by: Nathan Del Carmen, Ka Wai Chu
Tested by: Nathan Del Carmen, Ka Wai Chu
Debugged by: Nathan Del Carmen, Ka Wai Chu
Purpose: The search bar which searches through forum posts and tutor and returns keywords inputted.
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

    public class Article
    {
        public string Content { get; set; }
    }

    public partial class Search : Page
    {
        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        String queryStr;

        protected void Page_Load(object sender, EventArgs e)
        {

        }









        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string[] keywords = SearchWord.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            ErrorMessage.Visible = false;


            try
            {
                String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();
                conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                conn.Open();

                search sc = new search();


                List<string> searchwords = sc.parser((SearchWord.Text));

                sc.stopwords(searchwords);

                int count = 0;
                string datastring = "";
                queryStr = "SELECT COUNT(*) FROM forum";

                using (MySqlCommand cmd = new MySqlCommand(queryStr, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }

                conn.Close();

                //Finding a Post
                algo le = new algo();
                int closeness;
                bool found = false;
                List<string> forumstringresult = new List<string>();
                List<int> intforumresult = new List<int>();
                for (int loopvar = 1; loopvar <= count; ++loopvar)
                {
                    conn.Open();
                    queryStr = "SELECT post FROM forum WHERE forumId = '" + loopvar + "' ";
                    cmd = new MySqlCommand(queryStr, conn);
                    reader = cmd.ExecuteReader();
                    datastring = "";
                    found = false;
                    while (reader.HasRows && reader.Read())
                    {
                        datastring = reader.GetString(reader.GetOrdinal("post"));
                    }
                    List<string> datawords = sc.parser(datastring);
                    sc.stopwords(datawords);
                    conn.Close();

                    for (int i = 0; i < searchwords.Count; ++i)
                    {
                        for (int j = 0; j < datawords.Count; ++j)
                        {
                            closeness = le.levenshtein(searchwords[i], datawords[j]);
                            if (closeness <= 2)
                            {
                                forumstringresult.Add(datastring);
                                intforumresult.Add(loopvar);

                                found = true;
                                ErrorMessage.Visible = true;
                                FailureText.Text = "Results found";
                            }
                        }

                    }


                }

                conn.Open();
                queryStr = "SELECT COUNT(*) FROM thread";

                using (MySqlCommand cmd = new MySqlCommand(queryStr, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }

                conn.Close();

                List<string> commentstringresult = new List<string>();
                List<int> intcommentresult = new List<int>();
                int commentid;

                //Finding a Comment
                for (int loopvar = 1; loopvar <= count; ++loopvar)
                {
                    conn.Open();
                    queryStr = "SELECT comment FROM thread WHERE threadId = '" + loopvar + "' ";
                    cmd = new MySqlCommand(queryStr, conn);
                    reader = cmd.ExecuteReader();
                    datastring = "";

                    while (reader.HasRows && reader.Read())
                    {
                        datastring = reader.GetString(reader.GetOrdinal("comment"));
                    }
                    List<string> commentwords = sc.parser(datastring);
                    sc.stopwords(commentwords);
                    conn.Close();

                    for (int i = 0; i < searchwords.Count; ++i)
                    {
                        for (int j = 0; j < commentwords.Count; ++j)
                        {
                            closeness = le.levenshtein(searchwords[i], commentwords[j]);
                            if (closeness <= 2)
                            {
                                found = true;
                                commentstringresult.Add(datastring);
                                conn.Open();
                                queryStr = "SELECT forumId FROM thread WHERE threadId = '" + loopvar + "'";
                                using (MySqlCommand cmdd = new MySqlCommand(queryStr, conn))
                                {
                                    commentid = Convert.ToInt32(cmdd.ExecuteScalar());
                                }
                                intcommentresult.Add(commentid);
                                conn.Close();

                            }
                        }

                    }







                }

                //Finding a tutor
                List<string> tutornamesresult = new List<string>();
                List<int> tutorid = new List<int>();
                conn.Open();
                queryStr = "SELECT COUNT(*) FROM tutor";

                using (MySqlCommand cmd = new MySqlCommand(queryStr, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }

                conn.Close();

                for (int loopvar = 1; loopvar <= count; ++loopvar)
                {
                    conn.Open();
                    queryStr = "SELECT name FROM tutor WHERE accountId = '" + loopvar + "' ";
                    cmd = new MySqlCommand(queryStr, conn);
                    reader = cmd.ExecuteReader();
                    datastring = "";

                    while (reader.HasRows && reader.Read())
                    {
                        datastring = reader.GetString(reader.GetOrdinal("name"));
                    }
                    List<string> tutors = sc.parser(datastring);
                    sc.stopwords(tutors);
                    conn.Close();

                    for (int i = 0; i < searchwords.Count; ++i)
                    {
                        for (int j = 0; j < tutors.Count; ++j)
                        {
                            closeness = le.levenshtein(searchwords[i], tutors[j]);
                            if (closeness <= 1)
                            {
                                found = true;
                                tutornamesresult.Add(datastring);

                                tutorid.Add(loopvar);


                            }
                        }

                    }
                }

                //Finding a tutor course
                List<string> coursename = new List<string>();
                List<int> courseid = new List<int>();
                conn.Open();
                queryStr = "SELECT COUNT(*) FROM tutorcourse";

                using (MySqlCommand cmd = new MySqlCommand(queryStr, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }

                conn.Close();

                for (int loopvar = 1; loopvar <= count; ++loopvar)
                {
                    conn.Open();
                    queryStr = "SELECT course_name FROM tutorcourse WHERE subject_id = '" + loopvar + "' ";
                    cmd = new MySqlCommand(queryStr, conn);
                    reader = cmd.ExecuteReader();
                    datastring = "";

                    while (reader.HasRows && reader.Read())
                    {
                        datastring = reader.GetString(reader.GetOrdinal("course_name"));
                    }
                    List<string> tutors = sc.parser(datastring);
                    sc.stopwords(tutors);
                    conn.Close();

                    for (int i = 0; i < searchwords.Count; ++i)
                    {
                        for (int j = 0; j < tutors.Count; ++j)
                        {
                            closeness = le.levenshtein(searchwords[i], tutors[j]);
                            if (closeness <= 2)
                            {
                                found = true;
                                coursename.Add(datastring);
                                courseid.Add(loopvar);
                            }
                        }

                    }
                }

                if (found == true)
                {
                    ErrorMessage.Visible = true;
                    FailureText.Text = "Results found";

                    //display result
                }
                if (found == false)
                {
                    ErrorMessage.Visible = true;
                    FailureText.Text = "Results were not found";
                    Label1.Visible = false;
                }

                //Display

                Duplicates dup = new Duplicates();
                dup.Duplicate(forumstringresult);
                dup.Duplicate(commentstringresult);
                dup.Duplicate(tutornamesresult);
                dup.Duplicate(coursename);
                //Displays how many results found
                if (found == true)
                {
                    Label1.Visible = true;
                    Label1.Text = "Found " + (forumstringresult.Count + commentstringresult.Count + tutornamesresult.Count + coursename.Count) + " Results";
                }
                int secondloop = 0;
                int thirdloop = 0;
                int fourloop = 0;
                HyperLink[] links = new HyperLink[forumstringresult.Count + commentstringresult.Count + tutornamesresult.Count + coursename.Count];
                for (int n = 0; n < forumstringresult.Count + commentstringresult.Count + tutornamesresult.Count + coursename.Count; ++n)
                {
                    Label lbl = new Label();
                    if (n < forumstringresult.Count)
                    {
                        lbl.Text = "Post:&nbsp;&nbsp;";
                        links[n] = new HyperLink();
                        links[n].ID = "link" + n;

                        links[n].NavigateUrl = "Thread.aspx?t=" + intforumresult[n];



                        links[n].Text = forumstringresult[n] + "<br /><br />";
                    }
                    else if (n >= forumstringresult.Count && n < (forumstringresult.Count + commentstringresult.Count))
                    {
                        lbl.Text = "Comment:&nbsp;&nbsp;";
                        links[n] = new HyperLink();
                        links[n].ID = "link" + n;
                        links[n].NavigateUrl = "Thread.aspx?t=" + intcommentresult[secondloop];
                        links[n].Text = commentstringresult[secondloop] + "<br /><br />";
                        ++secondloop;
                    }
                    else if (n >= (forumstringresult.Count + commentstringresult.Count) && n < (forumstringresult.Count + commentstringresult.Count + tutornamesresult.Count))
                    {
                        lbl.Text = "Tutor:&nbsp;&nbsp;";
                        links[n] = new HyperLink();
                        links[n].ID = "link" + n;
                        links[n].NavigateUrl = "TutorPage.aspx?t=" + tutornamesresult[thirdloop];
                        links[n].Text = tutornamesresult[thirdloop] + "<br /><br />";
                        ++thirdloop;
                    }

                    else
                    {
                        lbl.Text = "Tutor Course:&nbsp;&nbsp;";
                        links[n] = new HyperLink();
                        links[n].ID = "link" + n;
                        links[n].NavigateUrl = "Tutor.aspx";
                        links[n].Text = coursename[fourloop] + "<br /><br />";
                        ++fourloop;
                    }


                    ph.Controls.Add(lbl);
                    ph.Controls.Add(links[n]);
                }

            }
            catch
            {

            }
        }
    }

}
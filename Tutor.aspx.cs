/** 
Written by: Kyle Clark, Elizabeth Chao
Tested by: Kyle Clark, Elizabeth Chao
Debugged by: Kyle Clark, Elizabeth Chao
Purpose: Display tree of what tutors are available for each class/subject.
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
    public partial class Tutor : Page
    {  
        String connString = System.Configuration.ConfigurationManager.ConnectionStrings["CredditConnString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["accountID"] == null)
            {
                Response.Redirect("~/Account/Login.aspx?m=LoginError");
            }
            DataSet ds = RunQuery("SELECT subject_name FROM tutorcourse GROUP BY subject_name");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                TreeNode root = new TreeNode(ds.Tables[0].Rows[i][0].ToString());
                root.SelectAction = TreeNodeSelectAction.Expand;
                CreateNode(root);
                TreeView1.Nodes.Add(root);
            }
        }

        void CreateNode(TreeNode node)
        {
        DataSet ds = RunQuery("SELECT course_name FROM tutorcourse WHERE subject_name ='" + node.Value + "'");
            if (ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                TreeNode tnode = new TreeNode(ds.Tables[0].Rows[i][0].ToString());
                tnode.SelectAction = TreeNodeSelectAction.Expand;
                node.ChildNodes.Add(tnode);
                CreateNode(tnode);
                DataSet ds2 = RunQuery("SELECT name FROM tutorcourse WHERE course_name ='" + tnode.Value + "'");

                if (ds2.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                {
                    TreeNode pnode = new TreeNode(ds2.Tables[0].Rows[j][0].ToString());
                    pnode.SelectAction = TreeNodeSelectAction.Expand;
                    tnode.ChildNodes.Add(pnode);
                    CreateNode(pnode);
                    string tutor_name = pnode.Text;
                    pnode.NavigateUrl = String.Format("TutorPage.aspx?tutor_name={0}", tutor_name);
                }
            }
        }

        DataSet RunQuery(String Query)
        {
            DataSet ds = new DataSet();
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand objCommand = new MySqlCommand(Query, conn);
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(objCommand);
                da.Fill(ds);
                da.Dispose();
                conn.Close();
            }
            return ds;
        }


        protected void TreeView1_SelectedNodeChanged1(object sender, EventArgs e)
        {
   
        }
    }
}

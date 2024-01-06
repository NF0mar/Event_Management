using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EventManagement
{
    public partial class Users : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            conn.Open();
            string sqlquery = "insert into Users values (@fullname,@Email,@Password,@UserType,@RegistrationDate)";
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            cmd.Parameters.AddWithValue("@fullname", txtfullname.Text);
            cmd.Parameters.AddWithValue("@Email", txtemail.Text);
            cmd.Parameters.AddWithValue("@Password", txtpassword.Text);
            cmd.Parameters.AddWithValue("@UserType", ddlutype.Text);
            cmd.Parameters.AddWithValue("@RegistrationDate", txtdate.Text);

            //cmd.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            lblinfo.Text = "User is saved";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            conn.Open();
            String edit = "update users set fullname=@fullname, Email=@Email, Password=@Password, UserType=@Usertype, RegistrationDate=@RegistrationDate where UserID= '" + txtid.Text + "' ";
            SqlCommand cmd = new SqlCommand(edit, conn);
            cmd.Parameters.AddWithValue("@fullname", txtfullname.Text);
            cmd.Parameters.AddWithValue("@Email", txtemail.Text);
            cmd.Parameters.AddWithValue("@Password",txtpassword.Text);
            cmd.Parameters.AddWithValue("@UserType", ddlutype.Text);
            cmd.Parameters.AddWithValue("@RegistrationDate", txtdate.Text);

            //cmd.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            lblinfo.Text = "User has been updated";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Users where UserID  = '" + txtid.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "User is deleted";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string userID = txtid.Text.Trim();

            if (!string.IsNullOrEmpty(userID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Users WHERE UserID = @userID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@userID", userID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        txtfullname.Text = reader["FullName"].ToString();
                        txtemail.Text = reader["Email"].ToString();
                        txtpassword.Text = reader["Password"].ToString();
                        ddlutype.SelectedValue = reader["UserType"].ToString();
                        txtdate.Text = reader["RegistrationDate"].ToString();

                        lblinfo.Text = "User found and form filled.";
                    }
                    else
                    {
                        lblinfo.Text = "User not found.";
                    }
                }
                catch (Exception ex)
                {
                    lblinfo.Text = "Error: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                lblinfo.Text = "Please enter a valid User ID.";
            }
        
        }
    }
}
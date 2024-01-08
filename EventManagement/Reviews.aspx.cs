using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace EventManagement
{
    public partial class Reviews : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Define your SQL query with a JOIN clause
                string query = @"

                    SELECT Reviews.ReviewID, Users.fullname AS FullName, Events.Title AS EventTitle, Reviews.Comment,Reviews.SubmissionDate 
                    FROM Reviews
                    INNER JOIN Events ON Reviews.EventID = Events.EventID
                    INNER JOIN Users ON Reviews.UserID = Users.UserID


                ";

                // Create a SqlConnection and SqlDataAdapter to execute the query

                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                // Create a DataSet to hold the result
                DataSet dataSet = new DataSet();

                // Fill the DataSet with data from the query
                adapter.Fill(dataSet);

                // Bind the result to your GridView
                GridView1.DataSource = dataSet.Tables[0]; // Replace GridView1 with your actual GridView ID
                GridView1.DataBind(); // Replace GridView1 with your actual GridView ID
                conn.Close();

            }

            if (!IsPostBack)
            {
                // Fetch the data from the "Users" table
                string query = "SELECT UserID, fullname FROM Users"; // Query to fetch user data

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Bind the data to the DropDownList
                    ddltuserid.DataSource = reader;
                    ddltuserid.DataTextField = "fullname"; // Display this field's values
                    ddltuserid.DataValueField = "UserID"; // Use this field's values as the selected value
                    ddltuserid.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddltuserid.Items.Insert(0, new ListItem("Select Your Name", ""));
            }
            if (!IsPostBack)
            {
                // Fetch the data from the "Users" table
                string query = "SELECT EventID, Title FROM Events"; // Query to fetch user data

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Bind the data to the DropDownList
                    ddlteventid.DataSource = reader;
                    ddlteventid.DataTextField = "Title"; // Display this field's values
                    ddlteventid.DataValueField = "EventID"; // Use this field's values as the selected value
                    ddlteventid.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddlteventid.Items.Insert(0, new ListItem("Select Event", ""));
            }
        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            conn.Open();

            // Insert the main data into the Events table
            string sqlquery = "INSERT INTO Reviews (UserID, EventID, Comment, SubmissionDate)" + "VALUES (@UserID, @EventID, @Comment, @SubmissionDate)";
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            cmd.Parameters.AddWithValue("@EventID", ddlteventid.Text);
            cmd.Parameters.AddWithValue("@UserID", ddltuserid.Text);
            cmd.Parameters.AddWithValue("@SubmissionDate", txtdate.Text);



            // Get the content of the textarea
            string comment = txtcomment.Value;

            if (string.IsNullOrEmpty(comment))
            {
                comment = "";
            }
            cmd.Parameters.AddWithValue("@Comment", comment);
            cmd.ExecuteNonQuery();

            lblinfo.Text = "Comment Submitted";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string ReviewID = txtReviewID.Text.Trim();

            if (!string.IsNullOrEmpty(ReviewID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Reviews WHERE ReviewID = @ReviewID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@ReviewID", ReviewID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        ddltuserid.SelectedValue = reader["UserID"].ToString();
                        ddlteventid.SelectedValue = reader["EventID"].ToString();
                        txtcomment.Value = reader["Comment"].ToString();
                        txtdate.Text = reader["SubmissionDate"].ToString();

                        lblinfo.Text = "Review Comment found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Review Comment not found.";
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
                lblinfo.Text = "Please enter a valid Review ID.";


            }

            GridView1.DataBind();
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtReviewID.Text, out int ReviewID))
                {

                    conn.Open();                 
                    string sqlquery = "UPDATE Reviews SET UserID = @UserID, EventID = @EventID, Comment = @Comment, SubmissionDate = @SubmissionDate";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@UserID", ddltuserid.Text);
                    cmd.Parameters.AddWithValue("@EventID", ddlteventid.Text);
                    cmd.Parameters.AddWithValue("@Comment", txtcomment.Value);
                    cmd.Parameters.AddWithValue("@SubmissionDate", txtdate.Text);
                    

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Review Comment updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Review Comment not found or update failed";
                    }

                    conn.Close();

                }
                else
                {
                    lblinfo.Text = "Invalid ReviewID";
                }
            }
            catch (Exception ex)
            {
                lblinfo.Text = "Error: " + ex.Message;
            }

            GridView1.DataBind();
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Reviews where ReviewID  = '" + txtReviewID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "Review Comment is deleted";
            conn.Close();

            GridView1.DataBind();
        }
    }
}
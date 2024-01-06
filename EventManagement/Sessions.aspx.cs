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
    public partial class Sessions : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Define your SQL query with a JOIN clause
                string query = @"

                    SELECT Sessions.SessionID, Sessions.Title AS SessionTitle, Events.Title AS EventTitle, Speakers.Name AS SpeakerName, Sessions.Description, Sessions.StartTime, Sessions.EndTime
                    FROM Sessions
                    INNER JOIN Events ON Sessions.EventID = Events.EventID
                    INNER JOIN Speakers ON Sessions.SpeakerID = Speakers.SpeakerID


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
                string query = "SELECT SpeakerID, Name FROM Speakers"; // Query to fetch user data

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Bind the data to the DropDownList
                    ddlspeaker.DataSource = reader;
                    ddlspeaker.DataTextField = "Name"; // Display this field's values
                    ddlspeaker.DataValueField = "SpeakerID"; // Use this field's values as the selected value
                    ddlspeaker.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddlspeaker.Items.Insert(0, new ListItem("Select Speaker", ""));
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
                    ddlevent.DataSource = reader;
                    ddlevent.DataTextField = "Title"; // Display this field's values
                    ddlevent.DataValueField = "EventID"; // Use this field's values as the selected value
                    ddlevent.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddlevent.Items.Insert(0, new ListItem("Select Event", ""));
            }
        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            conn.Open();

            // Insert the main data into the Events table
            string sqlquery = "INSERT INTO Sessions (EventID, SpeakerID, Title, Description, StartTime, EndTime)" + "VALUES (@EventID, @SpeakerID, @Title, @Description, @StartTime, @EndTime)";
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            cmd.Parameters.AddWithValue("@EventID", ddlevent.Text);
            cmd.Parameters.AddWithValue("@SpeakerID", ddlspeaker.Text);
            cmd.Parameters.AddWithValue("@Title", txtEventTitle.Text);
            cmd.Parameters.AddWithValue("@StartTime", txtStartTime.Text);
            cmd.Parameters.AddWithValue("@EndTime", txtEndTime.Text);
            


            // Get the content of the textarea
            string description = txtdescription.Value;

            if (string.IsNullOrEmpty(description))
            {
                description = "";
            }
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.ExecuteNonQuery();

            lblinfo.Text = "Session is Created";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtSessionID.Text , out int SessionID))
                {

                    conn.Open();
                    string sqlquery = "UPDATE Sessions SET EventID = @EventID, SpeakerID = @SpeakerID, Title = @Title, Description = @Description, StartTime = @StartTime, EndTime = @EndTime ";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@EventID", ddlevent.Text);
                    cmd.Parameters.AddWithValue("@SpeakerID", ddlspeaker.Text);
                    cmd.Parameters.AddWithValue("@Title", txtEventTitle.Text);
                    cmd.Parameters.AddWithValue("@Description", txtdescription.Value);
                    cmd.Parameters.AddWithValue("@StartTime", txtStartTime.Text);
                    cmd.Parameters.AddWithValue("@EndTime", txtEndTime.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Session updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Session not found or update failed";
                    }

                    conn.Close();

                }
                else
                {
                    lblinfo.Text = "Invalid SessionID";
                }
            }
            catch (Exception ex)
            {
                lblinfo.Text = "Error: " + ex.Message;
            }

            GridView1.DataBind();
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string SessionID = txtSessionID.Text.Trim();

            if (!string.IsNullOrEmpty(SessionID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Sessions WHERE SessionID = @SessionID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@SessionID", SessionID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        ddlevent.SelectedValue = reader["EventID"].ToString();
                        ddlspeaker.SelectedValue = reader["SpeakerID"].ToString();
                        txtEventTitle.Text = reader["Title"].ToString();
                        txtdescription.Value = reader["Description"].ToString();
                        txtStartTime.Text = reader["StartTime"].ToString();
                        txtEndTime.Text = reader["EndTime"].ToString();

                        lblinfo.Text = "Session found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Session not found.";
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
                lblinfo.Text = "Please enter a valid Session ID.";


            }

            GridView1.DataBind();
        }
    }
}
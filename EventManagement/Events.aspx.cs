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
    public partial class Events : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Define your SQL query with a JOIN clause
                string query = @"

                    SELECT Events.EventID, Users.fullname AS FullName, Events.Description, Events.Date, Events.Time, Events.Location, Events.Capacity
                    FROM Events
                    INNER JOIN Users ON Events.OrganizerID = Users.UserID


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
                    ddlorganizer.DataSource = reader;
                    ddlorganizer.DataTextField = "fullname"; // Display this field's values
                    ddlorganizer.DataValueField = "UserID"; // Use this field's values as the selected value
                    ddlorganizer.DataBind();

                    conn.Close();
                }

        // Add an initial item (optional)
        ddlorganizer.Items.Insert(0, new ListItem("Select Organizer", ""));
            }

        }

        protected void btnregister_Click(object sender, EventArgs e)    
        {

            string capacityValue = txtcapacity.Text.Trim();
            int capacity;

            if (string.IsNullOrEmpty(capacityValue) || !int.TryParse(capacityValue, out capacity) || capacity <= 0)
            {
                lblinfo.Text = "Please enter a valid capacity.";
                return; // Exit the event handler
            }

            conn.Open();

                // Insert the main data into the Events table
                string sqlquery = "INSERT INTO Events (Title, Date, Time, Location, Capacity, OrganizerID,Description)" + "VALUES (@Title, @Date, @Time, @Location, @Capacity, @OrganizerID,@Description)";
                SqlCommand cmd = new SqlCommand(sqlquery, conn);
                cmd.Parameters.AddWithValue("@Title", txtEventTitle.Text);
                cmd.Parameters.AddWithValue("@Date", txtdate.Text);
                cmd.Parameters.AddWithValue("@Time", ddlEventTime.Text);
                cmd.Parameters.AddWithValue("@Location", txtlocation.Text);
                cmd.Parameters.AddWithValue("@Capacity", txtcapacity.Text);
                cmd.Parameters.AddWithValue("@OrganizerID", ddlorganizer.Text);


                // Get the content of the textarea
                string description = txtdescription.Value;

                if (string.IsNullOrEmpty(description))
                {
                    description = "";
                }
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.ExecuteNonQuery();

                lblinfo.Text = "Event is saved";
                conn.Close();

                GridView1.DataBind();
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
           
            string eventID = txtEventID.Text.Trim();

            if (!string.IsNullOrEmpty(eventID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Events WHERE EventID = @eventID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@eventID", eventID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        txtEventTitle.Text = reader["Title"].ToString();
                        txtdescription.Value = reader["Description"].ToString();
                        txtdate.Text = reader["Date"].ToString();
                        ddlEventTime.SelectedValue = reader["Time"].ToString();
                        txtlocation.Text = reader["Location"].ToString();
                        txtcapacity.Text = reader["Capacity"].ToString();
                        ddlorganizer.SelectedValue = reader["OrganizerID"].ToString();

                        lblinfo.Text = "Event found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Event not found.";
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
                lblinfo.Text = "Please enter a valid Event ID.";
                
                
            }
            

        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure you have a valid event ID from txtEventID TextBox
                if (int.TryParse(txtEventID.Text, out int eventID))
                {
                    conn.Open();
                    string sqlquery = "UPDATE Events SET Title = @Title, Description = @Description, Date = @Date, Time = @Time, Location = @Location, Capacity = @Capacity WHERE EventID = @EventID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@EventID", eventID);
                    cmd.Parameters.AddWithValue("@Title", txtEventTitle.Text);
                    cmd.Parameters.AddWithValue("@Description", txtdescription.Value); // Assuming txtdescription is the ID of your TextArea
                    cmd.Parameters.AddWithValue("@Date", txtdate.Text);
                    cmd.Parameters.AddWithValue("@Time", ddlEventTime.SelectedValue); // Assuming ddlEventTime is the ID of your DropDownList
                    cmd.Parameters.AddWithValue("@Location", txtlocation.Text);
                    cmd.Parameters.AddWithValue("@Capacity", txtcapacity.Text);
                    cmd.Parameters.AddWithValue("@OrganizerID", ddlorganizer.SelectedValue); // Assuming ddlorganizer is the ID of your DropDownList for Organizer

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Event updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Event not found or update failed";
                    }

                    conn.Close();
                }
                else
                {
                    lblinfo.Text = "Invalid Event ID";
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
            String del = "delete from Events where EventID  = '" + txtEventID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "Event is deleted";
            conn.Close();

            GridView1.DataBind();
        }
    }
}
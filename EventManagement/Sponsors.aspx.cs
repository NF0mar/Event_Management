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
    public partial class Sponsors : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Define your SQL query with a JOIN clause
                string query = @"

                    SELECT Sponsors.SponsorID,  Events.Title AS EventTitle, Sponsors.Name, Sponsors.LogoURL, Sponsors.WebsiteURL, Sponsors.ContactName, Sponsors.ContactEmail, Sponsors.ContactPhone
                    FROM Sponsors
                    INNER JOIN Events ON Sponsors.EventID = Events.EventID


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
                string query = "SELECT EventID, Title FROM Events"; // Query to fetch user data

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Bind the data to the DropDownList
                    ddlEventID.DataSource = reader;
                    ddlEventID.DataTextField = "Title"; // Display this field's values
                    ddlEventID.DataValueField = "EventID"; // Use this field's values as the selected value
                    ddlEventID.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddlEventID.Items.Insert(0, new ListItem("Select Event", ""));
            }

        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            conn.Open();
            string sqlquery = "insert into Sponsors values (@Name,@LogoURL,@WebsiteURL,@ContactName,@ContactEmail,@ContactPhone,@EventID)";
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            cmd.Parameters.AddWithValue("@Name", txtSponsorName.Text);
            cmd.Parameters.AddWithValue("@LogoURL", txtLogoURL.Text);
            cmd.Parameters.AddWithValue("@WebsiteURL", txtWebsiteURL.Text);
            cmd.Parameters.AddWithValue("@ContactName", txtContactName.Text);
            cmd.Parameters.AddWithValue("@ContactEmail", txtContactEmail.Text);
            cmd.Parameters.AddWithValue("@ContactPhone", txtContactPhone.Text);
            cmd.Parameters.AddWithValue("@EventID", ddlEventID.Text);


            cmd.ExecuteNonQuery();
            lblinfo.Text = "Sponsor saved";
            conn.Close();

        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string sponsorID = txtSponsorID.Text.Trim();

            if (!string.IsNullOrEmpty(sponsorID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Sponsors WHERE SponsorID = @sponsorID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@sponsorID", sponsorID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        txtSponsorName.Text = reader["Name"].ToString();
                        txtLogoURL.Text = reader["LogoURL"].ToString();
                        txtWebsiteURL.Text = reader["WebsiteURL"].ToString();
                        ddlEventID.SelectedValue = reader["EventID"].ToString();
                        txtContactName.Text = reader["ContactName"].ToString();
                        txtContactEmail.Text = reader["ContactEmail"].ToString();
                        txtContactPhone.Text = reader["ContactPhone"].ToString();


                        lblinfo.Text = "Sponsor found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Sponsor not found.";
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
                lblinfo.Text = "Please enter a valid Sponsor ID.";


            }
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure you have a valid event ID from txtEventID TextBox
                if (int.TryParse(txtSponsorID.Text, out int sponsorID))
                {
                    conn.Open(); 
                    string sqlquery = "UPDATE Sponsors SET Name = @Name, LogoURL = @LogoURL, WebsiteURL = @WebsiteURL, ContactName = @ContactName, ContactEmail = @ContactEmail, ContactPhone = @ContactPhone, EventID = @EventID WHERE SponsorID = @SponsorID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@SponsorID", sponsorID);
                    cmd.Parameters.AddWithValue("@Name", txtSponsorName.Text);
                    cmd.Parameters.AddWithValue("@LogoURL", txtLogoURL.Text);
                    cmd.Parameters.AddWithValue("@WebsiteURL", txtWebsiteURL.Text); 
                    cmd.Parameters.AddWithValue("@ContactName", txtContactName.Text);
                    cmd.Parameters.AddWithValue("@ContactEmail", txtContactEmail.Text);
                    cmd.Parameters.AddWithValue("@ContactPhone", txtContactPhone.Text);
                    cmd.Parameters.AddWithValue("@EventID", ddlEventID.SelectedValue);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Sponsor updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Sponsor not found or update failed";
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


        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Sponsors where SponsorID  = '" + txtSponsorID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "Sponsor is deleted";
            conn.Close();
        }
    }
}
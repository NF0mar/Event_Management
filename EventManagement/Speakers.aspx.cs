using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EventManagement
{
    public partial class Speakers : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            conn.Open();

            // Insert the main data into the Events table
            string sqlquery = "INSERT INTO Speakers (Name, Bio, SocialMediaLinks)" + "VALUES (@Name, @Bio, @SocialMediaLinks)";
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            cmd.Parameters.AddWithValue("@name", txtname.Text);
            


            // Get the content of the textarea
            String bio = txtbio.Value;
            string socialMediaLinks = txtsocialmedialinks.Value;

            if (string.IsNullOrEmpty(bio) || string.IsNullOrEmpty(socialMediaLinks))
            {
                bio = "";
                socialMediaLinks = "";
            }
            cmd.Parameters.AddWithValue("@Bio", bio);
            cmd.Parameters.AddWithValue("@SocialMediaLinks", socialMediaLinks);
            cmd.ExecuteNonQuery();

            lblinfo.Text = "Event is saved";
            conn.Close();

            GridView1.DataBind();
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse( txtSpeakerID.Text , out int SpeakerID))
                {

                    conn.Open();
                    string sqlquery = "UPDATE Speakers SET Name = @Name, Bio = @Bio, SocialMediaLinks = @SocialMediaLinks";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@Name", txtname.Text);
                    cmd.Parameters.AddWithValue("@Bio", txtbio.Value);
                    cmd.Parameters.AddWithValue("@SocialMediaLinks", txtsocialmedialinks.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Speaker updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Speaker not found or update failed";
                    }

                    conn.Close();

                }
                else
                {
                    lblinfo.Text = "Invalid SpeakerID";
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
            string SpeakerID = txtSpeakerID.Text.Trim();

            if (!string.IsNullOrEmpty(SpeakerID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Speakers WHERE SpeakerID = @SpeakerID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@SpeakerID", SpeakerID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        txtname.Text = reader["Name"].ToString();
                        txtbio.Value = reader["Bio"].ToString();
                        txtsocialmedialinks.Value = reader["SocialMediaLinks"].ToString();

                        lblinfo.Text = "Speaker found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Speaker not found.";
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
                lblinfo.Text = "Please enter a valid Speaker ID.";


            }
        }
    }
}
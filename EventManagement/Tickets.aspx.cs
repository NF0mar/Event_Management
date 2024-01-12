using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Diagnostics;

namespace EventManagement
{
    public partial class Tickets : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");

        // Define ticket prices
        private const decimal GeneralAdmissionPrice = 50m; // Example price
        private const decimal VipTicketPrice = 100m; // Example price

        protected void Page_Load(object sender, EventArgs e)
        {
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
                ddlteventid.Items[0].Attributes["disabled"] = "disabled";
                ddlteventid.Items[0].Attributes["selected"] = "selected";
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
                ddltuserid.Items.Insert(0, new ListItem("Select User", ""));
                ddltuserid.Items[0].Attributes["disabled"] = "disabled";
                ddltuserid.Items[0].Attributes["selected"] = "selected";
            }

            if (!IsPostBack)
            {
                lblPrice.Text = "$0.00"; // Initial placeholder value
                                         // Other initialization code...
            }

        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            string ticketType = ddlTicketType.SelectedValue; // Get the selected ticket type
            int quantity = Convert.ToInt32(txtQuantity.Text); // Get the entered quantity

            decimal price = CalculatePrice(ticketType, quantity); // Calculate the price with correct parameters

            string query = "INSERT INTO Tickets (EventID, UserID, Type, Quantity, Price, PurchaseDate) VALUES (@EventID, @UserID, @Type, @Quantity, @Price, @PurchaseDate)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EventID", ddlteventid.SelectedValue);
                cmd.Parameters.AddWithValue("@UserID", ddltuserid.SelectedValue);
                cmd.Parameters.AddWithValue("@Type", ticketType);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Price", price); // Use the calculated price
                cmd.Parameters.AddWithValue("@PurchaseDate", Convert.ToDateTime(txtdate.Text));

                conn.Open(); // Open the connection
                cmd.ExecuteNonQuery(); // Execute the INSERT query
                lblinfo.Text = "Ticket is purcashed";
                conn.Close(); // Close the connection
            }
        }

        protected void ddlTicketType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePrice();
        }

        protected void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            UpdatePrice();
        }

        private decimal CalculatePrice(string ticketType, int quantity)
        {
            decimal pricePerTicket = ticketType == "General Admission" ? GeneralAdmissionPrice : VipTicketPrice;
            return pricePerTicket * quantity;
        }

        private void UpdatePrice()
        {

            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                // If the quantity is not entered or is invalid, don't update the price.
                lblPrice.Text = "Enter quantity"; // You can change this message if needed.
                return;
            }

            string ticketType = ddlTicketType.SelectedValue;
            decimal price = CalculatePrice(ticketType, quantity);
            lblPrice.Text = price.ToString("C", new CultureInfo("en-US"));

            
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Tickets where TicketID  = '" + txtTicketID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "Ticket is deleted";
            conn.Close();

        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string ticketID = txtTicketID.Text.Trim();

            if (!string.IsNullOrEmpty(ticketID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Tickets WHERE TicketID = @TicketID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@ticketID", ticketID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        ddlteventid.SelectedValue = reader["EventID"].ToString();
                        ddltuserid.SelectedValue = reader["UserID"].ToString();
                        ddlTicketType.SelectedValue = reader["Type"].ToString();
                        txtQuantity.Text = reader["Quantity"].ToString();
                        lblPrice.Text = reader["Price"].ToString();
                        txtdate.Text = reader["PurchaseDate"].ToString();

                        lblinfo.Text = "Ticket info found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Ticket info not found";
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
                lblinfo.Text = "Please enter a valid Ticket ID.";


            }
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure you have a valid Ticket ID from txtTicketID TextBox
                if (int.TryParse(txtTicketID.Text, out int ticketID))
                {
                    conn.Open();
                    string sqlquery = "UPDATE Tickets SET EventID = @EventID, UserID = @UserID, Type = @Type, Quantity = @Quantity, Price = @Price, PurchaseDate = @PurchaseDate WHERE TicketID = @TicketID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@TicketID", ticketID);
                    cmd.Parameters.AddWithValue("@EventID", ddlteventid.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserID", ddltuserid.SelectedValue);
                    cmd.Parameters.AddWithValue("@Type", ddlTicketType.SelectedValue);
                    cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text);
                    cmd.Parameters.AddWithValue("@Price", lblPrice.Text);
                    cmd.Parameters.AddWithValue("@PurchaseDate", txtdate.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Ticket updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Ticket info not found or update failed";
                    }

                    conn.Close();
                }
                else
                {
                    lblinfo.Text = "Invalid Ticket ID";
                }
            }
            catch (Exception ex)
            {
                lblinfo.Text = "Error: " + ex.Message;
            }
        }
    }
}
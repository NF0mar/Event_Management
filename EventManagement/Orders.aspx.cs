using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EventManagement
{
    public partial class Orders : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateUsersDropdown();
                PopulatePaymentMethodsDropdown();
            }
        }

        private void PopulateUsersDropdown()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT UserID, fullname FROM Users", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            ddlUserID.DataSource = reader;
            ddlUserID.DataTextField = "fullname";
            ddlUserID.DataValueField = "UserID";
            ddlUserID.DataBind();

            conn.Close();
        }

        private void PopulatePaymentMethodsDropdown()
        {
            // Assuming you have a PaymentMethods table with ID and MethodName
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT PaymentID, Method FROM Payments", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            ddlPaymentID.DataSource = reader;
            ddlPaymentID.DataTextField = "Method";
            ddlPaymentID.DataValueField = "PaymentID";
            ddlPaymentID.DataBind();

            conn.Close();
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Orders where OrderID = '" + txtOrderID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "Order is deleted";
            conn.Close();
        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string insertQuery = "INSERT INTO Orders (UserID, TotalPrice, PaymentID) VALUES (@UserID, @TotalPrice, @PaymentID)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@UserID", ddlUserID.SelectedValue);
                cmd.Parameters.AddWithValue("@TotalPrice", lblTotalPrice.Text); // You need to set this value from your application logic
                cmd.Parameters.AddWithValue("@PaymentID", ddlPaymentID.SelectedValue);

                cmd.ExecuteNonQuery();
                lblinfo.Text = "Order registered successfully.";
            }
            catch (Exception ex)
            {
                lblinfo.Text = "Error while registering order: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        private void DisplayTotalPriceForUser(string userId)
        {
            string query = "SELECT SUM(Price) FROM Tickets WHERE UserID = @UserID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            conn.Open();
            object result = cmd.ExecuteScalar();
            conn.Close();

            if (result != null)
            {
                lblTotalPrice.Text = $"Total Price: ${result.ToString()}";
            }
            else
            {
                lblTotalPrice.Text = "No purchases found for this user.";
            }
        }

        protected void ddlUserID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string userId = ddlUserID.SelectedValue;
            DisplayTotalPriceForUser(userId);
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            string orderID = txtOrderID.Text.Trim();

            if (!string.IsNullOrEmpty(orderID))
            {
                try
                {
                    conn.Open();
                    string sqlquery = "SELECT * FROM Orders WHERE OrderID = @orderID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);
                    cmd.Parameters.AddWithValue("@orderID", orderID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Populate the form fields with the retrieved data
                        ddlUserID.SelectedValue = reader["UserID"].ToString();
                        lblTotalPrice.Text = reader["TotalPrice"].ToString();
                        ddlPaymentID.SelectedValue = reader["PaymentID"].ToString();

                        lblinfo.Text = "Order found and form filled.";

                    }
                    else
                    {
                        lblinfo.Text = "Order not found.";
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
                lblinfo.Text = "Please enter a valid Order ID.";


            }
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure you have a valid event ID from txtEventID TextBox
                if (int.TryParse(txtOrderID.Text, out int orderID))
                {
                    conn.Open(); 
                    string sqlquery = "UPDATE Orders SET UserID = @UserID, TotalPrice = @TotalPrice, PaymentID = @PaymentID WHERE OrderID = @OrderID";
                    SqlCommand cmd = new SqlCommand(sqlquery, conn);

                    cmd.Parameters.AddWithValue("@OrderID", orderID);
                    cmd.Parameters.AddWithValue("@UserID", ddlUserID.SelectedValue);
                    cmd.Parameters.AddWithValue("@TotalPrice", lblTotalPrice.Text);
                    cmd.Parameters.AddWithValue("@PaymentID", ddlPaymentID.SelectedValue);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblinfo.Text = "Order updated successfully";
                    }
                    else
                    {
                        lblinfo.Text = "Order not found or update failed";
                    }

                    conn.Close();
                }
                else
                {
                    lblinfo.Text = "Invalid Order ID";
                }
            }
            catch (Exception ex)
            {
                lblinfo.Text = "Error: " + ex.Message;
            }

        }
    }
}
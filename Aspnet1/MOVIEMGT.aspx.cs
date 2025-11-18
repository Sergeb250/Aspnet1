using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Aspnet1
{
    public partial class MOVIEMGT : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["movieConnString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMovies();
            }
        }

        protected void saveBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text) || string.IsNullOrWhiteSpace(ratingBox.Text))
                {
                    errorlbl.Visible = true;
                    errorlbl.Text = "Name and Rating are required fields.";
                    return;
                }

                string sqlQuery = "INSERT INTO Movies (name, director, description, rating) VALUES (@name, @director, @description, @rating)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", nameBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@director", directorBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", descriptionBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@rating", ratingBox.Text.Trim());

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        messagelbl.Visible = true;
                        messagelbl.Text = "Movie saved successfully!";
                        errorlbl.Visible = false;
                        LoadMovies();
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Error: " + ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        protected void importBtn_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension == ".csv")
                    {
                        ImportFromCSV(fileUpload.FileContent);
                    }
                    else
                    {
                        ShowError("Please select a CSV file only.");
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Error importing CSV file: " + ex.Message);
                }
            }
            else
            {
                ShowError("Please select a CSV file to import.");
            }
        }

        private void ImportFromCSV(Stream fileStream)
        {
            int importedCount = 0;
            int errorCount = 0;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line;
                bool isFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    string[] values = line.Split(',');

                    if (values.Length >= 4)
                    {
                        try
                        {
                            string name = values[0].Trim();
                            string director = values[1].Trim();
                            string description = values[2].Trim();
                            string ratingStr = values[3].Trim();

                            if (!string.IsNullOrEmpty(name))
                            {
                                string sqlQuery = @"INSERT INTO Movies (name, director, description, rating) 
                                                  VALUES (@name, @director, @description, @rating)";

                                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@name", name);
                                    cmd.Parameters.AddWithValue("@director", director);
                                    cmd.Parameters.AddWithValue("@description", description);

                                    if (decimal.TryParse(ratingStr, out decimal rating))
                                        cmd.Parameters.AddWithValue("@rating", rating);
                                    else
                                        cmd.Parameters.AddWithValue("@rating", DBNull.Value);

                                    if (conn.State == ConnectionState.Closed)
                                        conn.Open();

                                    cmd.ExecuteNonQuery();
                                    importedCount++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                        }
                    }
                }
            }

            if (errorCount == 0)
            {
                ShowSuccess($"Import completed successfully! {importedCount} movies imported.");
            }
            else
            {
                ShowSuccess($"Import completed with {errorCount} errors. {importedCount} movies imported successfully.");
            }

            LoadMovies();
        }

        protected void clearBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
            ShowSuccess("Form cleared successfully!");
        }

        protected void refreshBtn_Click(object sender, EventArgs e)
        {
            LoadMovies();
            ShowSuccess("Movie list refreshed!");
        }

        private void ShowSuccess(string message)
        {
            messagelbl.Text = message;
            messagelbl.CssClass = "alert alert-success";
            messagelbl.Visible = true;
            errorlbl.Visible = false;
        }

        private void ShowError(string message)
        {
            errorlbl.Text = message;
            errorlbl.CssClass = "alert alert-danger";
            errorlbl.Visible = true;
            messagelbl.Visible = false;
        }

        private void ClearFields()
        {
            nameBox.Text = "";
            directorBox.Text = "";
            descriptionBox.Text = "";
            ratingBox.Text = "";
            movieIdlbl.Text = "";
        }

        private void LoadMovies()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Movies", conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    movieGridView.DataSource = dt;
                    movieGridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorlbl.Text = "Error loading movies: " + ex.Message;
                errorlbl.Visible = true;
            }
        }

        protected void movieGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditView")
            {
                string movieId = e.CommandArgument.ToString();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Movies WHERE MovieId=@movieId", conn))
                    {
                        cmd.Parameters.AddWithValue("@movieId", movieId);

                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    movieIdlbl.Text = reader["MovieId"].ToString();
                                    nameBox.Text = reader["name"].ToString();
                                    directorBox.Text = reader["director"].ToString();
                                    descriptionBox.Text = reader["description"].ToString();
                                    ratingBox.Text = reader["rating"].ToString();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorlbl.Text = "Error loading movie details: " + ex.Message;
                    errorlbl.Visible = true;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }

            if (e.CommandName == "DeleteMovie")
            {
                try
                {
                    string movieId = e.CommandArgument.ToString();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Movies WHERE MovieId=@movieId", conn))
                    {
                        cmd.Parameters.AddWithValue("@movieId", movieId);

                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        int row = cmd.ExecuteNonQuery();
                        if (row > 0)
                        {
                            messagelbl.Visible = true;
                            messagelbl.Text = "Movie deleted successfully!";
                            errorlbl.Visible = false;
                            LoadMovies();
                            ClearFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorlbl.Text = "Error deleting movie: " + ex.Message;
                    errorlbl.Visible = true;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        protected void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(movieIdlbl.Text))
                {
                    errorlbl.Visible = true;
                    errorlbl.Text = "No movie selected for update.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(nameBox.Text) || string.IsNullOrWhiteSpace(ratingBox.Text))
                {
                    errorlbl.Visible = true;
                    errorlbl.Text = "Name and Rating are required fields.";
                    return;
                }

                string sqlQuery = "UPDATE Movies SET name=@name, director=@director, description=@description, rating=@rating WHERE MovieId=@movieId";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", nameBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@director", directorBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", descriptionBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@rating", ratingBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@movieId", movieIdlbl.Text.Trim());

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    int row = cmd.ExecuteNonQuery();
                    if (row > 0)
                    {
                        messagelbl.Visible = true;
                        messagelbl.Text = "Movie updated successfully!";
                        errorlbl.Visible = false;
                        LoadMovies();
                        ClearFields();
                    }
                    else
                    {
                        errorlbl.Visible = true;
                        errorlbl.Text = "Movie not found or no changes made.";
                    }
                }
            }
            catch (Exception ex)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Error: " + ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}
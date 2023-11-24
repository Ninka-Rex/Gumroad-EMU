using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Gumroad_EMU
{
    public partial class Main : Form
    {
        // Static variable to store the application version
        public static string version = "1.0";

        // Constructor for the main form
        public Main()
        {
            InitializeComponent();
        }

        // Event handler for when the main form is loaded
        private void Main_Load(object sender, EventArgs e)
        {
            // Set up server controls and initialize logs
            Server.Set(txtConsole, btnStart, btnStop);
            Logs.SetConsole(txtConsole);
            Logs.Info("Gumroad-EMU " + version + "\n");

            // Start the server on a separate thread
            Thread serverThread = new Thread(Server.Start);
            Logs.Info("Starting server...");
            serverThread.Start();

            // Load parameters from the saved file and update the UI
            loadParameters();
            updateParameters();
        }

        // Event handler for the "Start" button click
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start the server
            Server.Start();
        }

        // Event handler for the "Stop" button click
        private void btnStop_Click(object sender, EventArgs e)
        {
            // Stop the server
            Server.Stop();
        }

        // Event handler for the main form closing
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Display a warning to remove the certificate before exiting
            Logs.Warn("Please remove the certificate before exiting!");

            // Stop the server
            Server.Stop();
        }

        // Save the user-input parameters to a JSON file
        private void saveParameters()
        {
            // Create a JSON object to store the parameters
            JObject parameters = new JObject();
            parameters.Add("email", txtEmail.Text);
            parameters.Add("variant", txtVariant.Text);
            parameters.Add("price", txtPrice.Text);
            parameters.Add("permalink", txtPermalink.Text);
            parameters.Add("productPermalink", txtProductPermalink.Text);
            parameters.Add("productName", txtProductName.Text);

            // Save the JSON object to a file
            using (StreamWriter file = File.CreateText("Gumroad-EMU.json"))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    parameters.WriteTo(writer);
                }
            }
        }

        // Load parameters from a JSON file and update the UI
        private void loadParameters()
        {
            // Check if the JSON file exists
            if (!File.Exists("Gumroad-EMU.json"))
            {
                return;
            }

            // Load the JSON object from the file
            JObject parameters = JObject.Parse(File.ReadAllText("Gumroad-EMU.json"));

            // Set the text boxes to the values from the JSON object
            txtEmail.Text = parameters["email"].ToString();
            txtVariant.Text = parameters["variant"].ToString();
            txtPrice.Text = parameters["price"].ToString();
            txtPermalink.Text = parameters["permalink"].ToString();
            txtProductPermalink.Text = parameters["productPermalink"].ToString();
            txtProductName.Text = parameters["productName"].ToString();
        }

        // Event handlers for text box changes, updating parameters on the fly
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        private void txtVariant_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        private void txtProductPermalink_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        private void txtPermalink_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            updateParameters();
        }

        // Update parameters based on the current text box values
        private void updateParameters()
        {
            responder.setParameters(txtEmail.Text, txtVariant.Text, txtPrice.Text, txtPermalink.Text, txtProductPermalink.Text, txtProductName.Text);
        }

        // Event handler for the "Save" button click
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Attempt to save parameters and log success
                saveParameters();
                Logs.Info("Saved parameters successfully!");
            }
            catch (Exception ex)
            {
                // Log an error if saving parameters fails
                Logs.Error("Couldn't save parameters");
            }
        }

        // Event handler for key press in the price text box, allowing only numeric and decimal input
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}

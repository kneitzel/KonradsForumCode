using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetworkExample
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Creates a new instance of <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click on the Open Client button.
        /// </summary>
        private void OnOpenClientButtonClick(object sender, EventArgs e)
        {
            new ClientWindow().Show();
        }

        /// <summary>
        /// Handles the click on the Open Server button.
        /// </summary>
        private void OnOpenServerButtonClick(object sender, EventArgs e)
        {
            new ServerWindow().Show();
        }

        /// <summary>
        /// Handles the click on the Kill Application button.
        /// </summary>
        private void OnKillAppButtonClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the FormClosing event of the MainWindow.
        /// </summary>
        private void OnMainWindowClosing(object sender, FormClosingEventArgs e)
        {
            // Get all forms that must be closed.
            List<Form> formsToClose = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                if (!ReferenceEquals(form, this))
                    formsToClose.Add(form);
            }

            // Close the forms.
            foreach (var form in formsToClose)
                form.Close();

            // Clear list.
            formsToClose.Clear();
        }
    }
}

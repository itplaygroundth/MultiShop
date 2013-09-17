using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoApplication1
{
    public partial class DetailForm : Form
    {
        public DetailForm()
        {
            InitializeComponent();
        }

        internal void ReadIndividual(Individual individual)
        {
            tbFirstname.Text = individual.FIRSTNAME;
            tbMiddlename.Text = individual.MIDDLENAME;
            tbLastname.Text = individual.LASTNAME;
            tbDateOfBirth.Text = individual.DOB.ToShortDateString();
            tbState.Text = individual.STATE;
        }

        internal void WriteIndividual(Individual individual)
        {
            individual.FIRSTNAME = tbFirstname.Text;
            individual.MIDDLENAME = tbMiddlename.Text;
            individual.LASTNAME = tbLastname.Text;
            DateTime.TryParse(tbDateOfBirth.Text, out individual.DOB);
            individual.STATE = tbState.Text;
        }

    }
}

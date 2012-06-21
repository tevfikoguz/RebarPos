﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using OZOZ.RebarPosWrapper;

namespace RebarPosCommands
{
    public partial class EditBOQForm : Form
    {
        Dictionary<string, ObjectId> m_Styles;
        ObjectId m_CurrentStyle;

        public ObjectId TableStyle { get { return m_CurrentStyle; } }
        public string TableHeader { get { return txtHeader.Text; } }
        public string TableFooter { get { return txtFooter.Text; } }
        public int Multiplier { get { return (int)udMultiplier.Value; } }
        public double TextHeight { get { return double.Parse(txtTextHeight.Text); } }
        public double TableHeight { get { return double.Parse(txtTableHeight.Text); } }
        public double TableMargin { get { return double.Parse(txtTableMargin.Text); } }

        public EditBOQForm()
        {
            InitializeComponent();

            m_Styles = new Dictionary<string, ObjectId>();
        }

        public bool Init(ObjectId tableid)
        {
            m_Styles = DWGUtility.GetTableStyles();

            if (m_Styles.Count == 0)
            {
                return false;
            }

            try
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BOQTable table = tr.GetObject(tableid, OpenMode.ForRead) as BOQTable;
                    if (table == null) return false;
                    m_CurrentStyle = table.StyleId;
                    txtHeader.Text = table.Heading;
                    txtFooter.Text = table.Footing;
                    udMultiplier.Value = table.Multiplier;
                    txtTextHeight.Text = table.Scale.ToString();
                    txtTableHeight.Text  = table.MaxHeight.ToString();
                    txtTableMargin.Text = table.TableSpacing.ToString();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int i = 0;
            foreach (KeyValuePair<string, ObjectId> pair in m_Styles)
            {
                cbStyle.Items.Add(pair.Key);
                if (pair.Value == m_CurrentStyle) cbStyle.SelectedIndex = i;
                i++;
            }

            return true;
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            foreach (ObjectId id in m_Styles.Values)
            {
                if (i == cbStyle.SelectedIndex)
                {
                    m_CurrentStyle = id;
                    break;
                }
                i++;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtTextHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox txt = sender as TextBox;
            double val = 0;
            if (string.IsNullOrEmpty(txt.Text) || double.TryParse(txt.Text, out val))
            {
                if (val < 0.0001)
                {
                    errorProvider.SetError(txt, "Yazı yüksekliği sıfırdan büyük olmalı.");
                    errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                    e.Cancel = true;
                }
                else
                {
                    errorProvider.SetError(txt, "");
                }
            }
            else
            {
                errorProvider.SetError(txt, "Lütfen bir reel sayı girin.");
                errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                e.Cancel = true;
            }
        }

        private void txtTableHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox txt = sender as TextBox;
            double val = 0;
            if (string.IsNullOrEmpty(txt.Text) || double.TryParse(txt.Text, out val))
            {
                errorProvider.SetError(txt, "");
            }
            else
            {
                errorProvider.SetError(txt, "Lütfen bir reel sayı girin.");
                errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                e.Cancel = true;
            }
        }

        private void txtTableMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox txt = sender as TextBox;
            double val = 0;
            if (string.IsNullOrEmpty(txt.Text) || double.TryParse(txt.Text, out val))
            {
                errorProvider.SetError(txt, "");
            }
            else
            {
                errorProvider.SetError(txt, "Lütfen bir reel sayı girin.");
                errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                e.Cancel = true;
            }
        }
    }
}

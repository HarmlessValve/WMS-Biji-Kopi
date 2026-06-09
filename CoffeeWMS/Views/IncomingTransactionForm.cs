using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CoffeeWMS.Models;
using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Views
{
    public partial class IncomingTransactionForm : UserControl, IIncomingTransactionView
    {
        public IncomingTransactionForm()
        {
            InitializeComponent();
            AssociateAndRaiseViewEvents();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedSupplierId
        {
            get => comboBoxSupplier.SelectedValue != null ? (int)comboBoxSupplier.SelectedValue : 0;
            set => comboBoxSupplier.SelectedValue = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedCoffeeId
        {
            get => comboBoxCoffee.SelectedValue != null ? (int)comboBoxCoffee.SelectedValue : 0;
            set => comboBoxCoffee.SelectedValue = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Quantity
        {
            get => (int)numericUpDownQuantity.Value;
            set => numericUpDownQuantity.Value = value;
        }

        public event EventHandler SaveEvent;

        private void AssociateAndRaiseViewEvents()
        {
            buttonSave.Click += delegate { SaveEvent?.Invoke(this, EventArgs.Empty); };
        }

        public void SetSupplierList(List<Supplier> suppliers)
        {
            comboBoxSupplier.DataSource = suppliers;
            comboBoxSupplier.DisplayMember = "CompanyName";
            comboBoxSupplier.ValueMember = "SupplierId";
            if (suppliers.Count > 0)
                comboBoxSupplier.SelectedIndex = 0;
        }

        public void SetCoffeeTypeList(List<CoffeeType> coffeeTypes)
        {
            comboBoxCoffee.DataSource = coffeeTypes;
            comboBoxCoffee.DisplayMember = "CoffeeName";
            comboBoxCoffee.ValueMember = "CoffeeId";
            if (coffeeTypes.Count > 0)
                comboBoxCoffee.SelectedIndex = 0;
        }

        public void ShowMessage(string message, bool isError = false)
        {
            MessageBox.Show(message, isError ? "Error" : "Info", MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public void ClearFields()
        {
            numericUpDownQuantity.Value = 1;
            if (comboBoxSupplier.Items.Count > 0)
                comboBoxSupplier.SelectedIndex = 0;
            if (comboBoxCoffee.Items.Count > 0)
                comboBoxCoffee.SelectedIndex = 0;
        }
    }
}

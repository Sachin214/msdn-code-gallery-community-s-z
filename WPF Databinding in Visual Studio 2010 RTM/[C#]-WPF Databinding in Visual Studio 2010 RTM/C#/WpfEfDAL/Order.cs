﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WpfEfDAL
{
    partial class Order : IDataErrorInfo
    {

        public Order()
        {
            //Handle this event so that UI can be notified if the customer is changed

            this.CustomerReference.AssociationChanged += new CollectionChangeEventHandler(Customer_AssociationChanged);

            //Set defaults
            this.OrderDate = DateTime.Today;
            this.AddError("Customer", "Please select a customer.");
        }

        private void Customer_AssociationChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
                OnPropertyChanging("Customer");
            else
            {
                if (e.Action == CollectionChangeAction.Add)
                    this.RemoveError("Customer");

                OnPropertyChanged("Customer");
            }
        }

        /// <summary>
        // This method is called in the OrderDate property setter of the Order
        // partial class generated by the Entity Data Model designer.
        /// </summary>
        partial void OnOrderDateChanged()
        {
            //Perform validation. 
            this.RemoveError("OrderDate");

            if (! _OrderDate.HasValue )
                this.AddError("OrderDate", "Please enter an order date.");
            else if ( (_ShipDate.HasValue) && (_ShipDate < _OrderDate ) )
                this.AddError("OrderDate", "Order date cannot be after ship date.");
        }


        #region "IDataErrorInfo Members"

        private Dictionary<String, String> m_validationErrors = new Dictionary<String, String>();

        private void AddError(String columnName, String msg)
        {
            if (!m_validationErrors.ContainsKey(columnName))
                m_validationErrors.Add(columnName, msg);
        }

        private void RemoveError(String columnName)
        {
            if (m_validationErrors.ContainsKey(columnName))
                m_validationErrors.Remove(columnName);
        }

        public  Boolean HasErrors
        {
            get
            {
                return (m_validationErrors.Count > 0);
            }
        }

        public string Error
        {
            get
            {
                if (m_validationErrors.Count > 0)
                    return "Order data is invalid";
                else
                    return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (m_validationErrors.ContainsKey(columnName))
                    return m_validationErrors[columnName].ToString();
                else
                    return null;
            }
        }
        #endregion
    }
}
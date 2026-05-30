// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a sales record with product and transaction details.
    /// </summary>
    public class Sale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sale"/> class with specified properties.
        /// </summary>
        /// <param name="UID">The unique identifier for the sale.</param>
        /// <param name="date">The date of the sale.</param>
        /// <param name="menuItem">The name of the item sold.</param>
        /// <param name="soldQuantity">The quantity sold.</param>
        /// <param name="totalSale">The total sale amount.</param>
        public Sale(string UID, System.DateTime date, string menuItem, int soldQuantity, decimal totalSale)
        {
            this.UID = UID;
            this.date = date;
            MenuItem = menuItem;
            this.quantitySold = soldQuantity;
            this.totalSale = totalSale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sale"/> class.
        /// </summary>
        public Sale()
        {
        }

        /// <summary>
        /// Gets or sets the date of the sale.
        /// </summary>
        public System.DateTime date
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name of the menu item sold.
        /// </summary>
        public string MenuItem
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the quantity sold in this transaction.
        /// </summary>
        public int quantitySold
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the total sale amount.
        /// </summary>
        public decimal totalSale
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the unique identifier for this sale.
        /// </summary>
        public string UID
        {
            get; set;
        }
    }
}

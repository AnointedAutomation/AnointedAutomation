// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents an inventory item with stock management details.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with specified properties.
        /// </summary>
        /// <param name="UID">The unique identifier for the inventory item.</param>
        /// <param name="reorder">The reorder status or information.</param>
        /// <param name="itemNo">The item number or SKU.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="manufacturer">The manufacturer of the item.</param>
        /// <param name="supplier">The supplier of the item.</param>
        /// <param name="description">The item description.</param>
        /// <param name="costPerItem">The cost per unit.</param>
        /// <param name="stockQuantity">The current quantity in stock.</param>
        /// <param name="inventoryValue">The total value of the inventory.</param>
        /// <param name="reorderLevel">The quantity at which to reorder.</param>
        /// <param name="daysPerReorder">The number of days between reorders.</param>
        /// <param name="itemReorderQuantity">The quantity to order when reordering.</param>
        /// <param name="itemDiscontinued">Whether the item is discontinued.</param>
        public Inventory(string UID, string reorder, string itemNo, string name, string manufacturer, string supplier, string description, decimal costPerItem,
                    int stockQuantity, string inventoryValue, int reorderLevel, int daysPerReorder, int itemReorderQuantity,
                    bool itemDiscontinued)
        {
            this.UID = UID;
            Reorder = reorder;
            ItemNo = itemNo;
            Name = name;
            Manufacturer = manufacturer;
            Supplier = supplier;
            Description = description;
            this.costPerItem = costPerItem;
            this.stockQuantity = stockQuantity;
            InventoryValue = inventoryValue;
            this.reorderLevel = reorderLevel;
            this.daysPerReorder = daysPerReorder;
            this.itemReorderQuantity = itemReorderQuantity;
            this.itemDiscontinued = itemDiscontinued;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        public Inventory()
        {
        }

        /// <summary>
        /// Gets or sets the cost per unit of the item.
        /// </summary>
        public decimal costPerItem
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the number of days between reorders.
        /// </summary>
        public int daysPerReorder
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the total value of the inventory.
        /// </summary>
        public string InventoryValue
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is discontinued.
        /// </summary>
        public bool itemDiscontinued
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the item number or SKU.
        /// </summary>
        public string ItemNo
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the quantity to order when reordering.
        /// </summary>
        public int itemReorderQuantity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the manufacturer of the item.
        /// </summary>
        public string Manufacturer
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the reorder status or information.
        /// </summary>
        public string Reorder
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the quantity at which to reorder.
        /// </summary>
        public int reorderLevel
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the current quantity in stock.
        /// </summary>
        public int stockQuantity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the supplier of the item.
        /// </summary>
        public string Supplier
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the inventory item.
        /// </summary>
        public string UID
        {
            get; set;
        }
    }
}

// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    [System.Serializable]
    /// <summary>
    /// Represents a product with pricing and sales information.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// Sets default quantity to 1. Currency is left unset (null): there is no implicit currency.
        /// </summary>
        public Product()
        {
            this.quantitySold = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class with specified properties.
        /// </summary>
        /// <param name="name">The name of the product (required).</param>
        /// <param name="cost">The cost to produce/acquire the product.</param>
        /// <param name="currency">The three-letter uppercase ISO 4217 currency code (required, e.g. "USD").</param>
        /// <param name="price">The selling price of the product.</param>
        /// <param name="productId">The unique identifier for the product.</param>
        /// <param name="quantitySold">The quantity sold (defaults to 1 if 0).</param>
        /// <exception cref="System.ArgumentNullException">Thrown when name or currency is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when currency is not three uppercase ASCII letters.</exception>
        public Product(string name, decimal cost, string currency, decimal price, string productId, int quantitySold)
        {
            this.cost = cost;
            this.Currency = RequireCurrencyCode(currency, nameof(currency));
            this.price = price;
            this.ProductId = productId;
            this.quantitySold = (quantitySold == 0) ? 1 : quantitySold;
            this.Name = name ?? throw new System.ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Validates a three-letter uppercase ISO 4217 currency code and returns it unchanged.
        /// </summary>
        /// <param name="currency">The code to validate.</param>
        /// <param name="paramName">The parameter name reported in the exception.</param>
        /// <returns>The validated code.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when currency is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when currency is not three uppercase ASCII letters.</exception>
        public static string RequireCurrencyCode(string currency, string paramName)
        {
            if (currency == null)
            {
                throw new System.ArgumentNullException(paramName, "Currency is required; there is no default currency.");
            }

            if (currency.Length != 3)
            {
                throw new System.ArgumentException("Currency must be a three-letter ISO 4217 code, got '" + currency + "'.", paramName);
            }

            foreach (char c in currency)
            {
                if (c < 'A' || c > 'Z')
                {
                    throw new System.ArgumentException("Currency must be three uppercase ASCII letters (ISO 4217), got '" + currency + "'.", paramName);
                }
            }

            return currency;
        }

        /// <summary>
        /// Cost to produce/acquire
        /// </summary>
        [DataMember]
        public decimal cost
        {
            get; set;
        }

        // </summary>
        [DataMember]
        /// <summary>
        /// Gets or sets the currency code for the product's pricing.
        /// </summary>
        public string Currency
        {
            get; set;
        }

        /// <summary>
        /// Amount of money the purchase costs
        /// </summary>
        [DataMember]
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Amount of money the purchase costs to customers
        /// </summary>
        [DataMember]
        public decimal price
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public string ProductId
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the quantity of this product that has been sold.
        /// </summary>
        public int quantitySold
        {
            get; set;
        }

        /// <summary>
        /// Calculates the total quantity sold for each product based on sales records.
        /// </summary>
        /// <param name="products">The list of products to update.</param>
        /// <param name="sales">The list of sales records to analyze.</param>
        /// <returns>The updated list of products with quantities sold.</returns>
        public static List<Product> GetTotalQuantitySold(List<Product> products, List<Sale> sales)
        {
            Dictionary<string, int> quantities = new Dictionary<string, int>();

            foreach (Sale sale in sales)
            {
                if (quantities.ContainsKey(sale.MenuItem))
                {
                    quantities[sale.MenuItem] += sale.quantitySold;
                }
                else
                {
                    quantities.Add(sale.MenuItem, sale.quantitySold);
                }
            }

            foreach (Product product in products)
            {
                if (quantities.TryGetValue(product.Name, out int soldQuantity))
                {
                    product.quantitySold = soldQuantity;
                }
                else
                {
                    product.quantitySold = 0;
                }
            }

            return products;
        }
    }
}

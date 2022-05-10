namespace Core.Models
{
    /// <summary>
    /// Bank Account.
    /// Contains properties representing an international bank account.
    /// </summary>
    public class BankAccount
    {
        /// <summary>
        /// IBAN property.
        /// </summary>
        /// <value>
        /// A SWIFT code is an international bank code that identifies particular banks worldwide. It's also known as a Bank Identifier Code (BIC). CommBank uses SWIFT codes to send money to overseas banks. A SWIFT code consists of 8 or 11 characters..
        /// </value>
        public string BankIdentifierCode { get; set; }

        /// <summary>
        /// IBAN property.
        /// </summary>
        /// <value>
        /// The IBAN consists of up to 34 alphanumeric characters, as follows: country code using ISO 3166-1 alpha-2 – two letters, check digits – two digits, and. Basic Bank Account Number (BBAN) – up to 30 alphanumeric characters that are country-specific
        /// </value>
        public string IBAN { get; set; }

        /// <summary>
        /// SortCode property.
        /// </summary>
        /// <value>
        /// The sort code, which is a six-digit number, is usually formatted as three pairs of numbers, for example 12-34-56. It identifies both the bank and the branch where the account is held. In some cases, the first digit of the sort code identifies the bank itself and in other cases the first 2 digits identify the bank.
        /// </value>
        public string SortCode { get; set; }

        /// <summary>
        /// RoutingNo property.
        /// </summary>
        /// <value>
        /// The ABA(American Bankers Association) routing number is a 9 digit number used to identify banks in America, similar to a UK sort code.
        /// </value>
        public string RoutingNo { get; set; }

        /// <summary>
        /// AccountNo property.
        /// </summary>
        /// <value>
        /// The Bank account number.
        /// </value>
        public string AccountNo { get; set; }

        /// <summary>
        /// IFSC property.
        /// </summary>
        /// <value>
        /// The Indian Financial System Code - 11 digit alpha.
        /// </value>
        public string IFSC { get; set; }

        /// <summary>
        /// BSB property.
        /// </summary>
        /// <value>
        /// The  BSB code is a six-digit number used to identify the individual branch of an Australian financial institution.
        /// </value>
        public string BSB { get; set; }

        /// <summary>
        /// AccountName property.
        /// </summary>
        /// <value>
        /// The Bank account name.
        /// </value>
        public string AccountName { get; set; }

    }

}
namespace FinCore.Api.Domain.Enums;

public enum AccountType
{
    Checking,
    Savings,
    Credit
}

public enum AccountStatus
{
    Active,
    Frozen,
    Closed
}

public enum CurrencyCode
{
    UAH,
    USD,
    EUR
}

public enum CardType
{
    Debit,
    Credit
}

public enum CardStatus
{
    Active,
    Blocked,
    Expired
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Payment,
    CardPurchase,
    Fee
}

public enum TransactionDirection
{
    Income,
    Expense
}

public enum TransactionStatus
{
    Completed,
    Pending,
    Rejected
}

public enum TransferStatus
{
    Completed,
    Pending,
    Rejected
}

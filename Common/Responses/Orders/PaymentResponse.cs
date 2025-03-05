namespace Common.Responses.Orders;

public record PaymentResponse
(
    int Id,
    decimal Amount,
    string Method, // Örneğin "CreditCard", "PayPal", "BankTransfer"
    DateTime PaymentDate,
    string Status  // "Pending", "Completed", "Failed" gibi değerler
);

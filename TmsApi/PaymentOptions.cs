using System.ComponentModel.DataAnnotations;

public class PaymentOptions
{
    [Required(ErrorMessage = "The GatewayUrl field is required.")]
    public required string GatewayUrl { get; init; }

    [Range(100, 100000, ErrorMessage = "MaxDepositBirr must be between 100 and 100,000 Ethiopian Birr.")]
    public decimal MaxDepositBirr { get; init; }
}

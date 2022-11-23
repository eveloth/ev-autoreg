namespace DataAccessLibrary.Models;

public class ExchangeCredentialsModel
{
#pragma warning disable CS8618
    
    public int UserId { get; set; }
    public byte[] EncryptedEmail { get; set; }
    public byte[] EncryptedPassword { get; set; }
    public byte[] IV { get; set; }
    
#pragma warning restore
}
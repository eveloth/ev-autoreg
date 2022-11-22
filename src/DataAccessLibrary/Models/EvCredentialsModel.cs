namespace DataAccessLibrary.Models;

public class EvCredentialsModel
{
#pragma warning disable CS8618
    
    public int UserId { get; set; }
    public string EncryptedEmail { get; set; }
    public string EncryptedPassword { get; set; }
    public byte[] IV { get; set; }
    
#pragma warning restore
}
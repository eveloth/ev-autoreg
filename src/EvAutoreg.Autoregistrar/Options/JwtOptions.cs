namespace EvAutoreg.Autoregistrar.Options;

public class JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
}
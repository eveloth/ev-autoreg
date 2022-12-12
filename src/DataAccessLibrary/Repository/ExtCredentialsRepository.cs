using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class ExtCredentialsRepository : IExtCredentialsRepository
{
    private readonly ISqlDataAccess _db;

    public ExtCredentialsRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<EvCredentialsModel?> GetEvCredentials(int userId, CancellationToken cts)
    {
        const string sql =
            @"SELECT * FROM ev_credentials
                             WHERE user_id = @UserId";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadSingle<EvCredentialsModel?>(sql, parameters, cts);
    }

    public async Task<int> SaveEvCredentials(
        EvCredentialsModel evCredentials,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO ev_credentials
                             (user_id, encrypted_email, encrypted_password, iv)
                             VALUES (@UserId, @EncryptedEmail, @EncryptedPassword, @IV)
                             ON CONFLICT (user_id)
                             DO UPDATE SET
                             user_id = excluded.user_id,
                             encrypted_email = excluded.encrypted_email,
                             encrypted_password = excluded.encrypted_password,
                             iv = excluded.iv
                             RETURNING user_id";

        var parameters = new DynamicParameters(evCredentials);

        return await _db.SaveData<int>(sql, parameters, cts);
    }

    public async Task<ExchangeCredentialsModel?> GetExchangeCredentials(
        int userId,
        CancellationToken cts
    )
    {
        const string sql =
            @"SELECT * FROM exchange_credentials
                             WHERE user_id = @UserId";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadSingle<ExchangeCredentialsModel?>(sql, parameters, cts);
    }

    public async Task<int> SaveExchangeCredentials(
        ExchangeCredentialsModel exchangeCredentials,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO exchange_credentials
                             (user_id, encrypted_email, encrypted_password, iv)
                             VALUES (@UserId, @EncryptedEmail, @EncryptedPassword, @IV)
                             ON CONFLICT (user_id)
                             DO UPDATE SET
                             user_id = excluded.user_id,
                             encrypted_email = excluded.encrypted_email,
                             encrypted_password = excluded.encrypted_password,
                             iv = excluded.iv
                             RETURNING user_id";

        var parameters = new DynamicParameters(exchangeCredentials);

        return await _db.SaveData<int>(sql, parameters, cts);
    }
}

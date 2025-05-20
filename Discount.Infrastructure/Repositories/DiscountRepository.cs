using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscountRepository> _logger;

    public DiscountRepository(IConfiguration configuration, ILogger<DiscountRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<Coupon> GetDiscount(string productName)
    {

        string connectionString = Environment.GetEnvironmentVariable("PMS_DISCOUNT_CONNECTION_STRING");
        await using var connection = new NpgsqlConnection(connectionString);
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
            ("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
        if (coupon == null)
            return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Available" };
        return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        string connectionString = Environment.GetEnvironmentVariable("PMS_DISCOUNT_CONNECTION_STRING");
        await using var connection = new NpgsqlConnection(connectionString);

        var affected =
            await connection.ExecuteAsync
            ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        string connectionString = Environment.GetEnvironmentVariable("PMS_DISCOUNT_CONNECTION_STRING");
        await using var connection = new NpgsqlConnection(connectionString);

        var affected = await connection.ExecuteAsync
        ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        string connectionString = Environment.GetEnvironmentVariable("PMS_DISCOUNT_CONNECTION_STRING");
        await using var connection = new NpgsqlConnection(connectionString);

        var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });

        if (affected == 0)
            return false;

        return true;
    }
}
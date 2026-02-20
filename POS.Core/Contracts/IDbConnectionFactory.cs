namespace POS.Core.Contracts;

public interface IDbConnectionFactory
{
    System.Data.Common.DbConnection CreateConnection();
}

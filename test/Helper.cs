using Dapper;
using Npgsql;

namespace Tests;

public static class Helper
{
    public static readonly NpgsqlDataSource DataSource;
    public static readonly string ClientAppBaseUrl = "http://localhost:5000";
    public static readonly string ApiBaseUrl = "http://localhost:5000/catalog";

    static Helper()
    {
        var envVarKeyName = "pgconn";

        var rawConnectionString = Environment.GetEnvironmentVariable(envVarKeyName)!;
        if (rawConnectionString == null)
        {
            throw new Exception($@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
YOUR CONN STRING PGCONN IS EMPTY.
Solution: Go to Settings, search for Test Runner, and add the environment variable in there.
Currently, your program looks for an environment variable is called {envVarKeyName}.

Best regards, Alex
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
");
        }

        try
        {
            var uri = new Uri(rawConnectionString);
            var properlyFormattedConnectionString = string.Format(
                "Server={0};Database={1};User Id={2};Password={3};Port={4};Pooling=false;",
                uri.Host,
                uri.AbsolutePath.Trim('/'),
                uri.UserInfo.Split(':')[0],
                uri.UserInfo.Split(':')[1],
                uri.Port > 0 ? uri.Port : 5432);
            DataSource =
                new NpgsqlDataSourceBuilder(properlyFormattedConnectionString).Build();
            DataSource.OpenConnection().Close();
        }
        catch (Exception e)
        {
            throw new Exception($@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
Your connection string is found, but could not be used. Are you sure you correctly inserted
the connection-string to Postgres?

Best regards, Alex
(Below is the inner exception)
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨", e);
        }
    }


    public static string BadResponseBody(string content)
    {
        return $@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
Hey buddy, I've tried to take the response body from the API and turn into a class object,
but that failed. Below is what you sent me + the inner exception.

Best regards, Alex
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
RESPONSE BODY: {content}

EXCEPTION:
";
    }

    public static void TriggerRebuild()
    {
        using (var conn = DataSource.OpenConnection())
        {
            try
            {
                conn.Execute(RebuildScript);
            }
            catch (Exception e)
            {
                throw new Exception($@"
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨
THERE WAS AN ERROR REBUILDING THE DATABASE.

Check the following: Are you running the postgres DB at Amazon Web Services in Stockholm?

Best regards, Alex.
(Below is the inner exception)
🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨🧨", e);
            }
        }
    }


    public static string RebuildScript = @"
DROP SCHEMA IF EXISTS factory_catalog CASCADE;
CREATE SCHEMA factory_catalog;
create table factory_catalog.boxes
(
    boxid       serial
        primary key,
    boxname     varchar(255) not null,
    description text,
    size        varchar(50)  not null,
    price       integer      not null,
    imageurl    text
);
 ";
}
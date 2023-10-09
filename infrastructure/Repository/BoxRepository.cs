using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repository;

public class BoxRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public BoxRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<BoxFeedQuery> GetAllBoxes()
    {
        string sql = $@"
           SELECT boxId AS {nameof(BoxFeedQuery.BoxId)},
           boxName AS {nameof(BoxFeedQuery.BoxName)},
           SUBSTRING(description ,1, 100) AS {nameof(BoxFeedQuery.Description)},
           size AS {nameof(BoxFeedQuery.Size)},
           price AS {nameof(BoxFeedQuery.Price)},
           imageUrl AS {nameof(BoxFeedQuery.ImageUrl)}
           FROM factory_catalog.boxes;
       ";
        using var
            conn = _dataSource.OpenConnection();
        return conn.Query<BoxFeedQuery>(sql);
    }


    public Box CreateBox(string boxName, string description, string size, int price, string imageUrl)
    {
        var sql = $@"
            INSERT INTO factory_catalog.boxes (boxName, description, size, price, imageUrl)
                VALUES (@boxName, @description, @size, @price, @imageUrl)
                RETURNING boxId AS {nameof(Box.BoxId)},
                boxName AS {nameof(Box.BoxName)},
                description AS {nameof(Box.Description)},
                size AS {nameof(Box.Size)},
                price AS {nameof(BoxFeedQuery.Price)},
                imageUrl AS {nameof(Box.ImageUrl)};
        ";
        using var conn = _dataSource.OpenConnection();

        return conn.QueryFirst<Box>(sql, new { boxName, description, size, price, imageUrl });
    }

    public Box GetBoxById(int boxId)
    {
        const string sql = $@"
            SELECT boxId AS BoxId, boxName, description, size, price, imageUrl
            AS {nameof(Box.ImageUrl)}
            FROM factory_catalog.boxes WHERE boxId = @boxId";

        using var
            conn = _dataSource.OpenConnection();

        return conn.QueryFirst<Box>(sql, new { boxId });
    }

    public Box UpdateBox(int boxId,string boxName, string description, string size, int price, string imageUrl)
    {
        var sql = $@"
            UPDATE factory_catalog.boxes SET boxName = @boxName, description = @description, size = @size, price = @price, imageUrl= @imageUrl
            WHERE boxId = @boxId
            RETURNING boxId AS {nameof(Box.BoxId)},
            description AS {nameof(Box.Description)},
            size AS {nameof(Box.Size)},
            price AS {nameof(Box.Price)},
            imageUrl AS {nameof(Box.ImageUrl)}
            ";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Box>(sql, new { boxId,boxName, description, size, price, imageUrl });

        }
    }

    public IEnumerable<BoxFeedQuery> SearchBoxes(string searchDtoSearchTerm, int searchDtoPageSize)
    {
        using (var connection = _dataSource.OpenConnection())
        {
            const string sql = @"
            SELECT boxId AS BoxId, boxName, description, price, size
            FROM factory_catalog.boxes
            WHERE (boxName ILIKE '%' || @searchDtoSearchTerm || '%' OR
                   description ILIKE '%' || @searchDtoSearchTerm || '%' OR
                   price::text ILIKE '%' || @searchDtoSearchTerm || '%' OR
                   size ILIKE '%' || @searchDtoSearchTerm || '%')
            LIMIT @searchDtoPageSize
        ";

            return connection.Query<BoxFeedQuery>(sql, new { searchDtoSearchTerm, searchDtoPageSize });
        }
    }


    
    public bool DeleteBox(int boxId)
    {
        var sql = @"
             DELETE FROM factory_catalog.boxes WHERE boxId = @boxId; 
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { boxId }) > 0;
        }
    }
}
    
  
namespace Data.DBCommands;

public class DbCommands
{
    public static string CreateDbCommandWithNotExists(string dbName) =>
        $@"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') 
               BEGIN
                   CREATE DATABASE [{dbName}];
                   PRINT 'Database {dbName} created successfully.';
               END
               ELSE
                   PRINT 'Database {dbName} already exists.';";

    public static string UseDbCommand(string dbName) => $"USE [{dbName}];";

    public static string CreateTablesCommandIfNotExist() => @"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
            BEGIN
                CREATE TABLE Products (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL,
                    Description NVARCHAR(MAX),
                    Price DECIMAL(18,2) NOT NULL,
                    ImageUrl NVARCHAR(255) NULL,
                    CategoryId INT NOT NULL,
                    Quantity INT NOT NULL DEFAULT 0,
                    CreatedAt DATETIME2 DEFAULT GETDATE(),
                    UpdatedAt DATETIME2 DEFAULT GETDATE(),
                    
                    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) 
                    REFERENCES Categories(Id) ON DELETE CASCADE
                );
                PRINT 'Table Products created successfully.';
            END
            ELSE
                PRINT 'Table Products already exists.';
        ";

    public static string CreateCategoriesTableIfNotExists() => @"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
            BEGIN
                CREATE TABLE Categories (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(50) NOT NULL,
                    Description NVARCHAR(255) NULL
                );
                PRINT 'Table Categories created successfully.';
            END
        ";

    public static string DropTablesCommand() => @"
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
            BEGIN
                DROP TABLE Products;
                PRINT 'Table Products dropped successfully.';
            END
            
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
            BEGIN
                DROP TABLE Categories;
                PRINT 'Table Categories dropped successfully.';
            END
        ";

    public static string GetProducts() => @"
            SELECT 
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageUrl,
                p.CategoryId,
                p.Quantity,
                c.Name AS CategoryName
            FROM 
                Products p
            LEFT JOIN 
                Categories c ON p.CategoryId = c.Id
            ORDER BY 
                p.Name;
        ";

    public static string GetCategories() => @"
        SELECT
            c.Id,
            c.Name,
            c.Description
        FROM Categories c
        ORDER BY 
            c.Name;
        ";

    // public static string InitializeDatabaseScript() =>
    //     CreateDbCommandWithNotExists("Shop") +
    //     UseDbCommand("Shop") +
    //     CreateCategoriesTableIfNotExists() +
    //     CreateTablesCommandIfNotExist();
}
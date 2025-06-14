using System.Data;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Repository.Abstractions;

// Working with managment of database transactions

// public class UnitOfWork : IUnitOfWork, IDisposable
// {
//     private readonly IDbProvider _dbProvider;
//     private IDbConnection? _connection;
//     private IDbTransaction? _transaction;
//     private bool _disposed;
//
//     // Lazy loading репозиториев
//     private Lazy<ProductRepository> _products;
//     private Lazy<CategoryRepository> _categories;
//     private Lazy<CommentRepository> _comments;
//     private Lazy<CartRepository> _carts;
//
//     public UnitOfWork(IDbProvider dbProvider, IServiceProvider serviceProvider)
//     {
//         _dbProvider = dbProvider;
//         
//         // Инициализируем lazy репозитории
//         _products = new Lazy<ProductRepository>(() => 
//             serviceProvider.GetRequiredService<ProductRepository>());
//         _categories = new Lazy<CategoryRepository>(() => 
//             serviceProvider.GetRequiredService<ICategoryRepository>());
//         _comments = new Lazy<CommentRepository>(() => 
//             serviceProvider.GetRequiredService<ICommentRepository>());
//         _carts = new Lazy<CartRepository>(() => 
//             serviceProvider.GetRequiredService<ICartRepository>());
//     }
//
//     public IProductRepository Products => _products.Value;
//     public ICategoryRepository Categories => _categories.Value;
//     public ICommentRepository Comments => _comments.Value;
//     public ICartRepository Carts => _carts.Value;
//
//     public async Task BeginTransactionAsync()
//     {
//         _connection ??= _dbProvider.CreateConnection();
//         if (_connection.State != ConnectionState.Open)
//             await _connection.OpenAsync();
//             
//         _transaction = await _connection.BeginTransactionAsync();
//     }
//
//     public async Task CommitAsync()
//     {
//         if (_transaction != null)
//         {
//             await _transaction.CommitAsync();
//             await _transaction.DisposeAsync();
//             _transaction = null;
//         }
//     }
//
//     public async Task RollbackAsync()
//     {
//         if (_transaction != null)
//         {
//             await _transaction.RollbackAsync();
//             await _transaction.DisposeAsync();
//             _transaction = null;
//         }
//     }
//
//     public void Dispose()
//     {
//         if (!_disposed)
//         {
//             _transaction?.Dispose();
//             _connection?.Dispose();
//             _disposed = true;
//         }
//     }
// }
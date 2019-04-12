using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace WebService.Models
{
    /// <summary>
    /// Definition of CRUD methods on repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrderRepository<T>
    {
        /// <summary>
        /// Path to the In-memory Database ex. C:/database  
        /// </summary>
        string databasePath { get; set; }
        /// <summary>
        /// Name of Ts Table ex. orders
        /// </summary>
        string tableName { get; set; }

        /// <summary>
        /// Finds all Ts form Database
        /// </summary>
        /// <returns> All Ts from database in IEnumerable array</returns>
        IEnumerable<T> FindAll();
        /// <summary>
        /// Find T by Id
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>IEnumerable array of T with one T found, or empty array if not</returns>
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Stores Entity from parameter in database
        /// </summary>
        /// <param name="entity">Entity will be stored in Database</param>
        void Create(T entity);
        /// <summary>
        /// Updates entiry in databse
        /// </summary>
        /// <param name="id">Id of to be modified entity</param>
        /// <param name="entity">new modifed copy</param>
        /// <returns>true if successful, false if not</returns>
        bool Update(Guid id, T entity);
        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="id">Id of entity, which will be deleted</param>
        /// <returns>True if successful, false if entity was not found</returns>
        bool Delete(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository<T> : IQueryable<T>
        where T : GuidEntity
    {
        /// <summary>
        /// Entity adding
        /// </summary>
        /// <param name="entity">Entity</param>
        void Add(T entity);

        /// <summary>
        /// entity async adding
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task AddAsync(T entity, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Many entities adding
        /// </summary>
        /// <param name="entities"></param>
        void AddMany(IEnumerable<T> entities);

        /// <summary>
        /// many entities async adding
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task AddManyAsync(IEnumerable<T> entities, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Full enity(document) saving
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isUpsert">create if absent</param>
        void Save(T entity, bool isUpsert = false);

        /// <summary>
        /// Full enity(document) async saving
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isUpsert">добавлять сущность, если она отсутствует в ИД</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SaveAsync(T entity, bool isUpsert = false, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Entity partial properties saving
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate">properties to update</param>
        void SavePartial(T entity, params Expression<Func<T, object>>[] propertiesToUpdate);

        /// <summary>
        /// Entity partial properties sync saving
        /// properties to update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <param name="propertiesToUpdate"></param>
        /// <returns></returns>
        Task SavePartialAsync(T entity, CancellationToken token = default(CancellationToken), params Expression<Func<T, object>>[] propertiesToUpdate);

        /// <summary>
        /// entity deleting
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// entity async deleting
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// many entites deleting
        /// </summary>
        /// <param name="entities"></param>
        void DeleteMany(IEnumerable<T> entities);

        /// <summary>
        /// many entites async deleting
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken token = default(CancellationToken));
    }
}

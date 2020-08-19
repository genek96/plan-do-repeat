using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Commons.MongoDB
{
	public class MongoDbRepository<TEntity>
	{
		private readonly IMongoDatabase database;
		private readonly string collection;

		private IMongoCollection<TEntity> Entities => database.GetCollection<TEntity>(collection);

		public MongoDbRepository(MongoDatabaseSettings settings)
		{
			var client = new MongoClient(settings.ConnectionString);
			database = client.GetDatabase(settings.DatabaseName);
			collection = settings.CollectionName;
		}


		protected Task AddAsync(TEntity p)
		{
			return Entities.InsertOneAsync(p);
		}

		protected async IAsyncEnumerable<TEntity> FindAsync(
			Expression<Func<TEntity, bool>> findConstraint,
			int? skip = null,
			int? limit = null,
			SortDirection? sortDirection = null,
			Expression<Func<TEntity, object>> sortBy = null)
		{
			var foundEntities = Entities.Find(findConstraint);
			if (sortDirection.HasValue)
			{
				sortBy = sortBy ?? throw new ArgumentNullException(nameof(sortBy));
				foundEntities = foundEntities.Sort(sortDirection.Value == SortDirection.Ascending
					? new SortDefinitionBuilder<TEntity>().Ascending(sortBy)
					: new SortDefinitionBuilder<TEntity>().Descending(sortBy));
			}

			var cursor = await foundEntities.Skip(skip).Limit(limit).ToCursorAsync();
			using (cursor)
			{
				while (true)
				{
					if (cursor.MoveNext())
					{
						foreach (var entity in cursor.Current) yield return entity;
					}
					else
					{
						break;
					}
				}
			}
		}

		protected Task<UpdateResult> UpdateAsync(
			Expression<Func<TEntity, bool>> filterConstraint,
			Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> updater)
		{
			return Entities.UpdateOneAsync<TEntity>(
				filterConstraint,
				updater(new UpdateDefinitionBuilder<TEntity>()));
		}

		protected Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterConstraint)
		{
			return Entities.DeleteOneAsync(filterConstraint);
		}
	}
}
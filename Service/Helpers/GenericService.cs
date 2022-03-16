using Mapster;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using ModelContext;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Helpers;
using Utils.Interfaces;

namespace Service.Helpers
{
    public class GenericService<T> : IGrGenericService<T> where T : class, IEntityModel
    {
        public WebContext Context {get;}
        private DbSet<T> DbSet { get; }

        private readonly List<string> IgnoreDefaultPropeties = new List<string>() { "CreatedBy", "UpdatedBy", "CreationDate", "UpdatedDate" };

        private readonly Logger Log;

        public GenericService(WebContext context)
        {
            Context = context;
            Log = LogManager.GetCurrentClassLogger();
            DbSet = Context.Set<T>();
        }

        public async Task<X> GetById<X>(Guid id) where X : IBaseIdDTO
        {
           
            if (Log.IsEnabled(LogLevel.Debug))
                Log.Info("Start GetById  {0}", typeof(T).Name);

            T ret = await DbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

            if(ret == null)
            {
                throw new KeyNotFoundException();
            }

            return (ret).Adapt<X>(TypeAdapterConfig.GlobalSettings);
        }
        public async Task<X> Create<X>(X entityDTO, bool ignorePermission = false) where X : IBaseIdDTO
        {
            if (Log.IsEnabled(LogLevel.Debug))
                Log.Info("Start Create {0}", typeof(T).Name);

            T newEntity = MapDtoToModel<X, T>(entityDTO);

            DbSet.Add(newEntity);

            await Context.SaveChangesAsync();

            return MapModelToDto<X, T>(newEntity);
        }

        public async Task<X> Update<X>(X entityDTO, Guid id, List<string> ignoreProperties = null) where X : IBaseIdDTO
        {
            T dbEntity = await DbSet.FindAsync(id);

            T updateEntity = MapDtoToModelMerged<X, T>(entityDTO, dbEntity, ignoreProperties);

            DbSet.Update(updateEntity);

            await Context.SaveChangesAsync();

            return MapModelToDto<X, T>(updateEntity);
        }

        public async Task<X> Delete<X>(Guid id, bool ignorePermission = false, bool ignoreOrder = false) where X : IBaseIdDTO
        {
            T dbEntity = DbSet.Find(id);
            DbSet.Remove(dbEntity);

            await Context.SaveChangesAsync();

            return MapModelToDto<X, T>(dbEntity);
        }

        public async Task<Pagination<X>> GetAll<X>(ODataQueryOptions<T> querySettings) where X : IBaseIdDTO, new()
        {
            return await ODataPagination<X,T>(querySettings, DbSet.AsQueryable());

        }

        protected async Task<Pagination<X>> ODataPagination<X, Y>(ODataQueryOptions<Y> querySettings, IQueryable<Y> query, IDictionary<string, object> parameters = null) where Y : class, IEntity where X : new()
        {

            string[] requestedExpands = ServiceHelper.GetMembersToExpandNames(querySettings);

            Pagination<X> ret = new Pagination<X>();
            ODataQuerySettings settings = new ODataQuerySettings
            {
                EnableCorrelatedSubqueryBuffering = true,
                HandleReferenceNavigationPropertyExpandFilter = true,
                PageSize = 200
            };

            if (querySettings.Filter != null)
            {
                ret.Count = querySettings.Filter.ApplyTo(query, settings).Cast<dynamic>().Count();
            }
            else
            {
                ret.Count = query.Count();
            }

            if (querySettings.Count != null && querySettings.Count.Value == true)
            {
                return ret;
            }

            X e = new X();


            if (e is ICustomQueryDTO<Y>)
            {
                query = ServiceHelper.ImplementCustomQuery<X, Y>(query, e, parameters);
                using (var scope = new MapContextScope())
                {
                    foreach (var k in parameters)
                    {
                        scope.Context.Parameters.Add(k.Key, k.Value);
                    }

                    ret.Items = querySettings.ApplyTo(query, settings).Adapt<List<X>>(TypeAdapterConfig.GlobalSettings).Cast<dynamic>();
                }
            }
            else
            {
                ret.Items = await querySettings.ApplyTo(query, settings).Cast<Y>().ProjectToType<X>().Cast<dynamic>().ToListAsync();
            }


            ret.PerPage = querySettings.Top == null ? 10 : querySettings.Top.Value > 200 ? 200 : querySettings.Top.Value;

            ret.Page = (querySettings.Skip == null ? 0 : querySettings.Skip.Value / ret.PerPage) + 1;

            return ret;
        }

        #region mapper
        protected Y MapDtoToModelMerged<X, Y>(X dto, Y destiny, List<string> ignoreProperties = null) where Y : class, IEntityModel
        {
            if (ignoreProperties == null)
            {
                ignoreProperties = new List<string>();
            }

            ignoreProperties.AddRange(((IBaseDTO)dto).IgnogreProperties);

            ignoreProperties.AddRange(IgnoreDefaultPropeties);

            TypeAdapterConfig type = TypeAdapterConfig<X, Y>.NewConfig().Ignore(ignoreProperties.ToArray()).Config;

            return dto.Adapt<X, Y>(destiny, type);
        }


        protected Y MapDtoToModel<X, Y>(X dto, List<string> ignoreProperties = null) where Y : class, IEntityModel
        {
            if (ignoreProperties == null)
            {
                ignoreProperties = new List<string>();
            }

            ignoreProperties.AddRange(((IBaseDTO)dto).IgnogreProperties);

            ignoreProperties.AddRange(IgnoreDefaultPropeties);

            TypeAdapterConfig type = TypeAdapterConfig<X, Y>.NewConfig().Ignore(ignoreProperties.ToArray()).Config;
            return dto.Adapt<Y>(type);
        }

        protected X MapModelToDto<X, Y>(Y model) where Y : class, IEntityModel
        {
            return model.Adapt<X>();
        }
        protected Pagination<X> MapModelToDto<X, Y>(Pagination<Y> model) where Y : class, IEntityModel
        {
            return model.Adapt<Pagination<X>>();
        }

        #endregion
    }
}

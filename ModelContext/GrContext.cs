
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Utils.Attributes;
using Utils.Interfaces;

namespace ModelContext
{
    public class GrContext : DbContext, IGrContext, IInfrastructure<IServiceProvider>
    {

        private readonly IHttpContextAccessor HttpContextAcessor;

        public IServiceProvider Provider;

        public GrContext(DbContextOptions options, IHttpContextAccessor httpContextAcessor)
           : base(options)
        {
            HttpContextAcessor = httpContextAcessor;
        }

        public GrContext(DbContextOptions options)
           : base(options)
        {

            try
            {
                Provider = this.GetInfrastructure<IServiceProvider>();
                HttpContextAcessor = Provider.GetService<IHttpContextAccessor>();

            }
            catch
            {

            }

        }


        public GrContext()
         : base()
        {
        }

  
        protected void ModelBuilderGenerate(ModelBuilder modelBuilder, Type model)
        {

            modelBuilder
                .Entity(model)
                .HasKey("Id");


            PropertyInfo[] props = model.GetProperties();
            //if (model.GetInterface(typeof(IMrOrderable<>).Name) != null)
            //{
            //    Attribute MrClass = model.GetCustomAttribute(typeof(MrClassAttribute));

            //    if (MrClass == null || ((MrClassAttribute)MrClass).UniqueKeyOrder == null)
            //    {
            //        //TODO:  Change to MedRoomException
            //        throw new Exception("Deve implementar qual campo será unique para manter a ordenação correta");
            //    }
            //    string UniqueKeyOrder = ((MrClassAttribute)MrClass).UniqueKeyOrder;

            //    modelBuilder
            //    .Entity(model)
            //    .HasIndex(UniqueKeyOrder, "Order")
            //    .IsUnique();


            //}
            Attribute attrIndex = model.GetCustomAttribute(typeof(GrIndexAttribute));
            if (attrIndex != null)
            {
                modelBuilder
                        .Entity(model).HasIndex(((GrIndexAttribute)attrIndex).Index)
                        .IsUnique();
            }

            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.Equals("Id"))
                {
                    continue;
                }

                object[] attrs = prop.GetCustomAttributes(true);

                foreach (object attr in attrs)
                {
                    if (attr == null)
                    {
                        continue;
                    }

                    //if (attr is MrReferenceAttribute rt)
                    //{
                    //    modelBuilder.Entity(model).Property(prop.Name)
                    //     .HasMany(rt.EntityReference, prop.Name)
                    //     .WithOne().Map()
                    //}

                    if (attr is GrForeignKeyAttribute at)
                    {
                        if (at.NameReference == null)
                        {
                            modelBuilder.Entity(at.Type)
                           .HasMany(model)
                           .WithOne(at.NavigationProperty ?? prop.Name.Substring(0, prop.Name.Length - 2))
                           .HasForeignKey(prop.Name)
                           .OnDelete(DeleteBehavior.Restrict); ;
                        }
                        else
                        {

                            modelBuilder.Entity(at.Type)
                              .HasMany(model, at.NameReference)
                              .WithOne(at.NavigationProperty ?? prop.Name.Substring(0, prop.Name.Length - 2))
                              .HasForeignKey(prop.Name)
                              .OnDelete(DeleteBehavior.Restrict); ;
                        }

                        continue;
                    }


                    if (attr is GrPropertyAttribute)
                    {
                        GrPropertyAttribute convertAttr = (GrPropertyAttribute)attr;
                        if (prop.PropertyType == typeof(string) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(string))
                        {
                            if (convertAttr.Length > 0)
                            {
                                modelBuilder.Entity(model)
                                  .Property(prop.Name).HasMaxLength(convertAttr.Length);
                            }
                            else if (!prop.PropertyType.IsEnum)
                            {
                                //TODO:  Change to MedRoomException
                                throw new Exception("Todas as string devem ter Lengths");
                            }
                        }
                        else if (prop.PropertyType == typeof(decimal) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(decimal))
                        {
                            if (convertAttr.Precision > 0)
                            {
                                modelBuilder.Entity(model)
                                 .Property(prop.Name).HasColumnType("decimal(" + convertAttr.Precision + "," + convertAttr.Scale + ")");
                            }
                            else
                            {
                                //TODO:  Change to MedRoomException
                                throw new Exception("Todos os decimal devem ter Precision");
                            }
                        }

                        if (convertAttr.UniqueKey)
                        {
                            List<string> unique = convertAttr.UniqueKeyComposite.ToList();

                            unique.Add(prop.Name);
                            modelBuilder
                            .Entity(model).HasIndex(unique.ToArray())
                            .IsUnique();
                        }



                        if (convertAttr.Default != null)
                        {
                            modelBuilder.Entity(model)
                                    .Property(prop.Name).HasDefaultValue(convertAttr.Default);
                        }

                        if (convertAttr.Required)
                        {

                            modelBuilder.Entity(model)
                                        .Property(prop.Name)
                                        .IsRequired();
                        }
                        else
                        {
                            modelBuilder.Entity(model)
                                        .Property(prop.Name)
                                        .IsRequired(false);
                        }
                    }
                }
            }
        }

        public override int SaveChanges()
        {

            SaveChangesHelper();

            return base.SaveChanges();
        }

        public virtual int SaveChangesIgnoreHelper()
        {

            return base.SaveChanges();
        }

        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default, bool ignoreHelper = false)
        {
            SaveChangesHelper(ignoreHelper);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, bool ignoreHelper = false)
        {
            SaveChangesHelper(ignoreHelper);
            
            return base.SaveChangesAsync(cancellationToken);
        }
      

        public override EntityEntry Remove(object entity)
        {
            //base.Attach(entity);
            return base.Remove(entity);
        }

        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class
        {
            //base.Attach<TEntity>(entity);
            return base.Remove<TEntity>(entity);
        }


        public override void RemoveRange(IEnumerable<object> entities)
        {
            //base.AttachRange(entities);
            base.RemoveRange(entities);
        }

        public override void RemoveRange(params object[] entities)
        {
            //base.AttachRange(entities);
            base.RemoveRange(entities);
        }

        public override EntityEntry Update(object entity)
        {
            return base.Update(entity);
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            //base.Attach<TEntity>(entity);
            if (entity is IEntityModel cv)
            {
                DetachLocal<TEntity>(cv.Id, entity);
            }
        
            return base.Update<TEntity>(entity);
        }

        private void DetachLocal<TEntity>(Guid Id, TEntity entity) where TEntity : class
        {
            var local = ChangeTracker.Entries<TEntity>().Where(x => ((IEntityModel)x.Entity).Id == Id && x.Entity.GetHashCode() != entity.GetHashCode()).FirstOrDefault(); 

            if(local != null)
            {
                local.State = EntityState.Detached;

                this.Attach(entity);
            }
        }

        public override void UpdateRange(IEnumerable<object> entities)
        {
            //base.AttachRange(entities);
            base.UpdateRange(entities);
        }

        public override void UpdateRange(params object[] entities)
        {
            //base.AttachRange(entities);
            base.UpdateRange(entities);
        }
        private void SaveChangesHelper(bool ignoreHelper = false)
        {
            ChangeTracker.DetectChanges();
            object[] added = ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Added)
                        .Select(t => t.Entity)
                        .ToArray();

            foreach (object entity in added)
            {
                if (entity is IEntityModel && !ignoreHelper)
                {
                    IEntityModel track = (IEntityModel)entity;
                    if (track.Id == null)
                    {
                        track.Id = Guid.NewGuid();
                    }

                    track.CreationDate = DateTime.Now;
               
                    track.UpdatedDate = DateTime.Now;
                }
            }

            object[] modified = ChangeTracker.Entries()
             .Where(t => t.State == EntityState.Modified)
             .Select(t => t.Entity)
             .ToArray();

            foreach (object entity in modified)
            {

                if (entity is IEntityModel && !ignoreHelper)
                {
                    Entry<IEntityModel>((IEntityModel)entity).Property(x => x.CreationDate).IsModified = false;

                    IEntityModel track = (IEntityModel)entity;
                    track.UpdatedDate = DateTime.Now;
                }
            }

            object[] deleted = ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Deleted)
            .Select(t => t.Entity)
            .ToArray();
        }
    }
}

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Domain;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class;
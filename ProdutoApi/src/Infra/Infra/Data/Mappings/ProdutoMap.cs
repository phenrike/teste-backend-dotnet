using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Mappings;

public class ProdutoMap : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome)
            .IsRequired();
        builder.Property(p => p.Preco)
            .IsRequired();
        builder.Property(p => p.MoedaOrigem)
            .IsRequired()
            .HasMaxLength(3);
    }
}
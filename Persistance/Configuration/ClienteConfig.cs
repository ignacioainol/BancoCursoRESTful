﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistance.Configuration
{
    public class ClienteConfig : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nombre)
                .HasMaxLength(80)
                .IsRequired();
            builder.Property(p => p.Nombre)
            .HasMaxLength(80)
            .IsRequired();
            builder.Property(p => p.FechaNacimiento)
                .IsRequired();
            builder.Property(p => p.Telefono)
                .HasMaxLength(9)
                .IsRequired();
            builder.Property(p => p.Email)
                .HasMaxLength(100);
            builder.Property(p => p.Direccion)
                .IsRequired()
                .HasMaxLength(120);
            builder.Property(p => p.Edad);
            builder.Property(p => p.CreatedBy)
                .HasMaxLength(30);
            builder.Property(p => p.LastModifiedBy)
                .HasMaxLength(30);
        }
    }
}

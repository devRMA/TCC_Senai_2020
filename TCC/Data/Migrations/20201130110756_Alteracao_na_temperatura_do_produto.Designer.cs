﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TCC.Data;

namespace TCC.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201130110756_Alteracao_na_temperatura_do_produto")]
    partial class Alteracao_na_temperatura_do_produto
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("TCC.Models.Produto", b =>
                {
                    b.Property<int?>("ProdutoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<float?>("Limite_max")
                        .HasColumnType("real");

                    b.Property<float?>("Limite_min")
                        .HasColumnType("real");

                    b.Property<float?>("Temperatura")
                        .HasColumnType("real");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("integer");

                    b.HasKey("ProdutoId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("TCC.Models.Usuario", b =>
                {
                    b.Property<int?>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UsuarioId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("TCC.Models.Produto", b =>
                {
                    b.HasOne("TCC.Models.Usuario", "Usuario")
                        .WithMany("Produtos")
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("TCC.Models.Usuario", b =>
                {
                    b.Navigation("Produtos");
                });
#pragma warning restore 612, 618
        }
    }
}

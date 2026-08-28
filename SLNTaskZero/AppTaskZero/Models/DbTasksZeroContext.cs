using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppTaskZero.Models;

public partial class DbTasksZeroContext : DbContext
{
    public DbTasksZeroContext()
    {
    }

    public DbTasksZeroContext(DbContextOptions<DbTasksZeroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Funcionario> Funcionarios { get; set; }

    public virtual DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConexaoSqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__Funciona__06370DAD8CA48CDA");

            entity.ToTable("Funcionario");

            entity.Property(e => e.Cargo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__Tarefa__06370DADD3A480D2");

            entity.ToTable("Tarefa");

            entity.Property(e => e.DataCancelada).HasColumnType("datetime");
            entity.Property(e => e.DataFinalizada).HasColumnType("datetime");
            entity.Property(e => e.DataIniciada).HasColumnType("datetime");
            entity.Property(e => e.DataPlanejada).HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Prazo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusTarefa)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Funcionario).WithMany(p => p.Tarefas)
                .HasForeignKey(d => d.FuncionarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarefa_Funcionario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

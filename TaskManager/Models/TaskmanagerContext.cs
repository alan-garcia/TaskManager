using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models;

public partial class TaskmanagerContext : DbContext
{
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Task> Tasks { get; set; }

    public TaskmanagerContext(DbContextOptions<TaskmanagerContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.;Database=taskmanager;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

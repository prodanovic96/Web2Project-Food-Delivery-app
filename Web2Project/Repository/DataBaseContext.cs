using Microsoft.EntityFrameworkCore;
using Web2Project.Models;

namespace Web2Project.Repository
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DbSet<Korisnik> Korisnik { get; set; }
        public DbSet<Proizvod> Proizvod { get; set; }
        public DbSet<Kategorija> Kategorija { get; set; }
        //public DbSet<Porudzbina> Porudzbina { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Korisnik>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.KorisnickoIme).IsRequired();
        //        entity.HasIndex(e => e.KorisnickoIme).IsUnique();
        //    });

        //    //modelBuilder.Entity<Porudzbina>(entity =>
        //    //{
        //    //    entity.ToTable("Porudzbine");
        //    //    entity.HasKey(e => e.Id);

        //    //    entity.HasOne(e => e.Dostavljac)
        //    //          .WithMany(p => p.Porudzbine)
        //    //          .HasForeignKey(d => d.DostavljacId);

        //    //    entity.HasOne(e => e.Potrosac)
        //    //          .WithMany(p => p.Porudzbine)
        //    //          .HasForeignKey(d => d.PotrosacId);


        //    //});


        //    //modelBuilder.Entity<Proizvod>(entity =>
        //    //{
        //    //    entity.ToTable("Proizvodi");
        //    //    entity.HasKey(e => e.Id);
        //    //    entity.Property(e => e.Naziv).IsRequired();
        //    //    entity.HasIndex(e => e.Naziv).IsUnique();
        //    //});

        //    //modelBuilder.Entity<Proizvod_Korpa>(entity =>
        //    //{
        //    //    entity.ToTable("Proizvod_Korpa");
        //    //    entity.HasKey(e => e.Id);

        //    //});
        //}
    }
}
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dodaj za svaku tabelu posebno da ne mogu imati null property-je i da im definises kljuc

            // Definisemo tabelu Kategorija
            modelBuilder.Entity<Kategorija>().HasKey(k => k.Id);
            modelBuilder.Entity<Kategorija>().Property(k => k.Naziv).IsRequired();
            modelBuilder.Entity<Kategorija>().HasIndex(k => k.Naziv).IsUnique();

            // Definisemo tabelu Proizvod
            modelBuilder.Entity<Proizvod>().HasKey(p => p.Id);
            modelBuilder.Entity<Proizvod>().Property(p => p.Naziv).IsRequired();
            modelBuilder.Entity<Proizvod>().HasIndex(p => p.Naziv).IsUnique();
            modelBuilder.Entity<Proizvod>().Property(p => p.Sastojci).IsRequired();
            modelBuilder.Entity<Proizvod>().Property(p => p.Cena).IsRequired();
            modelBuilder.Entity<Proizvod>().Property(p => p.ImagePath).IsRequired();

            modelBuilder.Entity<Proizvod>()
                .HasOne<Kategorija>(p => p.Kategorija)
                .WithMany(k => k.Proizvodi)
                .HasForeignKey(p => p.KategorijaId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }


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
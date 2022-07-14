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
        public DbSet<Korpa> Korpa { get; set; }
        public DbSet<KorpaProizvod> KorpaProizvod { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definisemo tabelu Korisnik
            modelBuilder.Entity<Korisnik>().HasKey(k => k.Id);
            modelBuilder.Entity<Korisnik>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Korisnik>().Property(k => k.KorisnickoIme).IsRequired();
            modelBuilder.Entity<Korisnik>().HasIndex(k => k.KorisnickoIme).IsUnique();
            modelBuilder.Entity<Korisnik>().Property(k => k.Email).IsRequired();
            modelBuilder.Entity<Korisnik>().HasIndex(k => k.Email).IsUnique();
            modelBuilder.Entity<Korisnik>().Property(k => k.Ime).IsRequired();
            modelBuilder.Entity<Korisnik>().Property(k => k.Prezime).IsRequired();
            modelBuilder.Entity<Korisnik>().Property(k => k.DatumRodjenja).IsRequired();
            modelBuilder.Entity<Korisnik>().Property(k => k.Adresa).IsRequired();
            modelBuilder.Entity<Korisnik>().Property(k => k.TipKorisnika).IsRequired();
            modelBuilder.Entity<Korisnik>().Property(k => k.Verifikovan).IsRequired();

            // Definisemo tabelu Kategorija
            modelBuilder.Entity<Kategorija>().HasKey(k => k.Id);
            modelBuilder.Entity<Kategorija>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Kategorija>().Property(k => k.Naziv).IsRequired();
            modelBuilder.Entity<Kategorija>().HasIndex(k => k.Naziv).IsUnique();

            // Definisemo tabelu Proizvod
            modelBuilder.Entity<Proizvod>().HasKey(p => p.Id);
            modelBuilder.Entity<Proizvod>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Proizvod>().Property(p => p.Naziv).IsRequired();
            modelBuilder.Entity<Proizvod>().HasIndex(p => p.Naziv).IsUnique();
            modelBuilder.Entity<Proizvod>().Property(p => p.Sastojci).IsRequired();
            modelBuilder.Entity<Proizvod>().Property(p => p.Cena).IsRequired();

            //// Definisemo tabelu Korpa
            modelBuilder.Entity<Korpa>().HasKey(p => p.Id);
            modelBuilder.Entity<Korpa>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Korpa>().Property(p => p.Cena).IsRequired();
            modelBuilder.Entity<Korpa>().Property(p => p.Status).IsRequired();

            // Definisemo tabelu KorpaProizvod
            modelBuilder.Entity<KorpaProizvod>().HasKey(p => p.Id);
            modelBuilder.Entity<KorpaProizvod>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<KorpaProizvod>().Property(p => p.Kolicina).IsRequired();

            // Korpa - KorpaProizvod
            modelBuilder.Entity<KorpaProizvod>()
                .HasOne<Korpa>(pp => pp.Korpa)
                .WithMany(p => p.KorpeProizvodi)
                .HasForeignKey(pp => pp.KorpaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Korisnik - Korpa
            modelBuilder.Entity<Korpa>()
                .HasOne<Korisnik>(p => p.Korisnik)
                .WithMany(k => k.Korpe)
                .HasForeignKey(p => p.KorisnikId);

            // Proizvod - KorpaProizvod
            modelBuilder.Entity<KorpaProizvod>()
                .HasOne<Proizvod>(pp => pp.Proizvod)
                .WithMany(p => p.KorpeProizvodi)
                .HasForeignKey(pp => pp.ProizvodId);

            // Proizvod - Kategorija
            modelBuilder.Entity<Proizvod>()
                .HasOne<Kategorija>(p => p.Kategorija)
                .WithMany(k => k.Proizvodi)
                .HasForeignKey(p => p.KategorijaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
namespace KinoBilietuRezervacija.Models;

public class Movie
{
    public int ID { get; set; }
    public string Pavadinimas { get; set; }
    public string Zanras { get; set; }
    public int Trukme { get; set; }
    public string Aprasymas { get; set; }

    public virtual ICollection<Screening> Seansai { get; set; } = new List<Screening>();
}

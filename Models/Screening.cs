using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace KinoBilietuRezervacija.Models;

public class Screening
{
    public int ID { get; set; }

    [Column("FilmoID")]
    public int FilmoID { get; set; }
    public DateTime DataLaikas { get; set; }
    public decimal Kaina { get; set; }
    public int SalesNumeris { get; set; }

    [ValidateNever]
    public virtual Movie? Filmas { get; set; }
}

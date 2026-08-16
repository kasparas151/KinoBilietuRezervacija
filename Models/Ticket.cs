using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace KinoBilietuRezervacija.Models;

public class Ticket
{
    public int ID { get; set; }

    public int SeansoID { get; set; }

    public string PirkejoVardas { get; set; }

    public int VietosNumeris { get; set; }

    public string MokejimoBusena { get; set; } = "Neapmokėta";

    public string? UserId { get; set; }
    [DefaultValue("Neapmokėta")]

    [ValidateNever]
    public ApplicationUser? User { get; set; }

    [ValidateNever]
    public Screening? Seansas { get; set; }
}

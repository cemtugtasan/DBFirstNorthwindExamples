using System;
using System.Collections.Generic;

namespace Odev.Model;

public partial class Rehber
{
    public int RehberId { get; set; }

    public string Ad { get; set; } = null!;

    public string? Soyad { get; set; }

    public string? Telefon { get; set; }
}

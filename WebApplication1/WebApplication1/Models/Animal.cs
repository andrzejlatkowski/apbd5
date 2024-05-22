using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Animal
{
    public int IdAnimal { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Area { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace server;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }
}

using System;
using System.Collections.Generic;

namespace For_Interview.Models.DbModels;

public partial class ApplyFile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? FilePath { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual User User { get; set; } = null!;
}

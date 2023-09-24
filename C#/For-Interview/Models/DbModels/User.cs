using System;
using System.Collections.Generic;

namespace For_Interview.Models.DbModels;

public partial class User
{
    public int Id { get; set; }

    public int OrgId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string Email { get; set; } = null!;

    public string Account { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<ApplyFile> ApplyFiles { get; set; } = new List<ApplyFile>();

    public virtual Org Org { get; set; } = null!;
}

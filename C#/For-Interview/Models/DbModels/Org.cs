using System;
using System.Collections.Generic;

namespace For_Interview.Models.DbModels;

public partial class Org
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

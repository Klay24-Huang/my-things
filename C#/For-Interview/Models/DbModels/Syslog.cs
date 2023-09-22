using System;
using System.Collections.Generic;

namespace For_Interview.Models.DbModels;

public partial class Syslog
{
    public int SeqNo { get; set; }

    public string Account { get; set; } = null!;

    public string Ipaddress { get; set; } = null!;

    public DateTime? LoginAt { get; set; }

    public DateTime CreateAt { get; set; }
}

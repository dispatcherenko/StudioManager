using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class User
{
    public int IdUser { get; set; }

    public string Userlogin { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public string Useremail { get; set; } = null!;

    public byte[]? Userprofilepicture { get; set; }

    public virtual ICollection<Usergame> Usergames { get; set; } = new List<Usergame>();
}

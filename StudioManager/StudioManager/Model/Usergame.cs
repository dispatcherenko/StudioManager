using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class Usergame
{
    public int IdUsergames { get; set; }

    public int? IdGame { get; set; }

    public int? IdUser { get; set; }

    public virtual Game? IdGameNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}

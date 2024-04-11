using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class Game
{
    public int IdGame { get; set; }

    public string Gamename { get; set; } = null!;

    public string Gamegenre { get; set; } = null!;

    public DateOnly Gamereleasedate { get; set; }

    public string Gamedescription { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Usergame> Usergames { get; set; } = new List<Usergame>();
}

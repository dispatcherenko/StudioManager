using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class Task
{
    public int IdTask { get; set; }

    public int? IdDepartment { get; set; }

    public int? IdGame { get; set; }

    public string Taskgroup { get; set; } = null!;

    public string Taskname { get; set; } = null!;

    public bool? Taskisactive { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; }

    public virtual Game? IdGameNavigation { get; set; }
}

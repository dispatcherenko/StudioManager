using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class Department
{
    public int IdDepartment { get; set; }

    public string Departmentname { get; set; } = null!;

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}

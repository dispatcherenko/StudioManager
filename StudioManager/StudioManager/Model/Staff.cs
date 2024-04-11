using System;
using System.Collections.Generic;

namespace StudioManager.Model;

public partial class Staff
{
    public int IdEmployee { get; set; }

    public int? IdDepartment { get; set; }

    public string Employeefullname { get; set; } = null!;

    public string Employeephonenumber { get; set; } = null!;

    public string Employeeemail { get; set; } = null!;

    public string Employeeposition { get; set; } = null!;

    public byte[]? Employeephoto { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StudioManager.Model;

public partial class Department : DataBaseTable
{
    [ObservableProperty]
    private int _departmentcount;
    private string _departmentname = null!;

    [Required]
    [MinLength(2)]
    [MaxLength(64)]
    public string Departmentname
    {
        get => _departmentname;
        set => SetProperty(ref _departmentname, value, true);
    }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StudioManager.Model;

public partial class Task : ObservableValidator
{
    [ObservableProperty]
    private int _idTask;
    [ObservableProperty]
    private int? _idDepartment;
    [ObservableProperty]
    private int? _idGame;
    private string _taskgroup;
    private string _taskname = null!;
    private string _taskstate = null!;
    [ObservableProperty]
    private DateOnly _taskdeadline;

    [MaxLength(64)]
    [MinLength(1)]
    public string Taskgroup
    {
        get => _taskgroup;
        set => SetProperty(ref _taskgroup, value, true);
    }

    [Required]
    [MaxLength(64)]
    [MinLength(1)]
    public string Taskname
    {
        get => _taskname;
        set => SetProperty(ref _taskname, value, true);
    }

    [Required]
    public string Taskstate
    {
        get => _taskstate;
        set => SetProperty(ref _taskstate, value, true);
    }

    public virtual Department? IdDepartmentNavigation { get; set; }

    public virtual Game? IdGameNavigation { get; set; }

  

}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StudioManager.Model;

public partial class Staff : ObservableValidator
{
    [ObservableProperty]
    private int _idEmployee;
    [ObservableProperty]
    private int? _idDepartment;
    private string _employeefullname = null!;
    private string _employeephonenumber = null!;
    private string _employeeemail = null!;
    private string _employeeposition = null!;
    [ObservableProperty]
    private byte[]? _employeephoto;
    private Sex _employeesex;

    [Required]
    [NotNull]
    [MaxLength(64)]
    [MinLength(2)]
    public string Employeefullname
    {
        get => _employeefullname;
        set => SetProperty(ref _employeefullname, value, true);
    }

    [Required]
    [NotNull]
    [Phone]
    public string Employeephonenumber
    {
        get => _employeephonenumber;
        set => SetProperty(ref _employeephonenumber, value, true);
    }

    [Required]
    [NotNull]
    [EmailAddress]
    public string Employeeemail
    {
        get => _employeeemail;
        set => SetProperty(ref _employeeemail, value, true);
    }

    [Required]  
    [MaxLength(64)]
    [MinLength(2)]
    public string Employeeposition
    {
        get => _employeeposition;
        set => SetProperty(ref _employeeposition, value, true);
    }

    [Required]
    public Sex Employeesex
    {
        get => _employeesex;
        set => SetProperty(ref _employeesex, value, true);
    }

    public virtual Department? IdDepartmentNavigation { get; set; }
}

public enum Sex
{
    М,
    Ж
}

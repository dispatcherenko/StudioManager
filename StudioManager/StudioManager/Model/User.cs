using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StudioManager.Model;

public partial class User : DataBaseTable
{
    private string _userlogin = null!;
    private string _userpassword = null!;
    private string _useremail = null!;
    [ObservableProperty]
    private byte[]? _userprofilepicture;

    [Required]
    [MinLength(4)]
    [MaxLength(64)]
    public string Userlogin
    {
        get => _userlogin;
        set => SetProperty(ref _userlogin, value);
    }

    [Required]
    [MinLength(4)]
    [MaxLength(64)]
    public string Userpassword
    {
        get => _userpassword;
        set => SetProperty(ref _userpassword, value);   
    }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{1,}$", ErrorMessage = "Invalid email format")]
    public string Useremail
    {
        get => _useremail;
        set => SetProperty(ref _useremail, value);
    }

    public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();

    public virtual ICollection<Usergame> Usergames { get; set; } = new List<Usergame>();
}

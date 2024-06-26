﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StudioManager.Model;

public partial class Game : DataBaseTable
{
    [ObservableProperty]
    private byte[]? _gamepicture;
    [ObservableProperty]
    private DateOnly _gamereleasedate;
    private string _gamename = null!;
    private string _gamegenre = null!;
    private string _gamedescription;
    [ObservableProperty]
    private string _gameinfo;

    [Required]
    [NotNull]
    [MaxLength(64)]
    [MinLength(2)]
    public string Gamename
    {
        get => _gamename;
        set => SetProperty(ref _gamename, value, true);
    }

    [Required]
    [NotNull]
    [MaxLength(64)]
    [MinLength(2)]
    public string Gamegenre
    {
        get => _gamegenre;
        set => SetProperty(ref _gamegenre, value, true);
    }

    [MaxLength(256)]
    public string Gamedescription
    {
        get => _gamedescription;
        set => SetProperty(ref _gamedescription, value, true);
    }

    public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Usergame> Usergames { get; set; } = new List<Usergame>();
}

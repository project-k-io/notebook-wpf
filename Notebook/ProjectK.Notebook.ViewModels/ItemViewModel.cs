using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.ViewModels.Enums;

namespace ProjectK.Notebook.ViewModels;

public class ItemViewModel : ObservableObject
{
    private bool _isSelected;
    private string _kind;
    private ModifiedStatus _modified;

    public string Kind
    {
        get => _kind;
        set => SetProperty(ref _kind, value);
    }

    public INode Model { get; set; }

    public string Description
    {
        get => Model.Description;
        set
        {
            if (Model.Description == value)
                return;

            Model.Description = value;
            OnPropertyChanged();
        }
    }

    public Guid Id
    {
        get => Model.Id;
        set
        {
            if (Model.Id == value)
                return;

            Model.Id = value;
            OnPropertyChanged();
        }
    }

    public Guid ParentId
    {
        get => Model.ParentId;
        set
        {
            if (Model.ParentId == value)
                return;

            Model.ParentId = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name == value)
                return;

            Model.Name = value;
            OnPropertyChanged();
        }
    }

    public DateTime Created
    {
        get => Model.Created;
        set
        {
            if (Model.Created == value)
                return;

            Model.Created = value;
            OnPropertyChanged();
        }
    }

    public string Context
    {
        get => Model.Context;
        set
        {
            if (Model.Context == value)
                return;

            Model.Context = value;
            OnPropertyChanged();
        }
    }

    public ModifiedStatus Modified
    {
        get => _modified;
        set => SetProperty(ref _modified, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
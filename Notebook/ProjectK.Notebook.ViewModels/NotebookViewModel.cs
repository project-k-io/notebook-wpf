using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

//using ;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ItemViewModel
    {
        public NotebookViewModel()
        {
            Kind = "Notebook";
        }

        public NotebookViewModel(INode model) : this()
        {
            Model = model;
        }

        #region Storage Functions

        #endregion
    }
}
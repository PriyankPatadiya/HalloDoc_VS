﻿using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IViewNotes
    {
        public ViewNotesVM viewnotes(int id);
    }
}

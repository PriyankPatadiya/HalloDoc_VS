using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class ViewNotesRepo : IViewNotes
    {
        private readonly  ApplicationDbContext _context;
        public ViewNotesRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public ViewNotesVM viewnotes(int id)
        {
            ViewNotesVM model = new ViewNotesVM();
            var transferedNotes = (from reqlog in _context.RequestStatusLogs 
                                   where reqlog.RequestId == id 
                                   select reqlog.Notes).ToList();
            model.TransferNotes = transferedNotes;
            model.AdminNotes = _context.RequestNotes.FirstOrDefault(s => s.RequestId == id).AdminNotes;
            model.PhysicianNotes = _context.RequestNotes.FirstOrDefault(s => s.RequestId == id).PhysicianNotes;
            return model;
        }
    }
}

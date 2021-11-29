using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userID;
        public NoteService(Guid userID)
        {
            _userID = userID;
        }
        public bool CreateNote(NoteCreate model)
        {
            var entity = new Note()
            {
                OwnerID = _userID,
                Title = model.Title,
                Content = model.Content,
                CreatedUTC = DateTimeOffset.Now
            };
            using(var ctx = new ApplicationDbContext())
            {
                ctx.Notes.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }
        public IEnumerable<NoteListItem> GetNotes()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                                .Notes
                                .Where(e => e.OwnerID == _userID)
                                .Select(e => new NoteListItem
                                {
                                    NoteID = e.NoteID,
                                    Title = e.Title,
                                    CreatedUTC = e.CreatedUTC
                                });
                return query.ToArray();
            }
        }
    }
}

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
        public bool CreateNote(NoteCreate model) //creates an instance of note
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
        public IEnumerable<NoteListItem> GetNotes() // see all notes that belong to a specfic user 
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
        public NoteDetail GetNoteByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteID == id && e.OwnerID == _userID);
                return new NoteDetail
                {
                    NoteID = entity.NoteID,
                    Title = entity.Title,
                    Content = entity.Content,
                    CreatedUTC = entity.CreatedUTC,
                    ModifiedUTC = entity.ModifiedUTC,
                };
            }
        }
        public bool UpdateNote(NoteEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Notes.SingleOrDefault(e => e.NoteID == model.NoteID && e.OwnerID == _userID); //the .Single will kick an error on null, .SingleOrDefault will default to null removing the error (will return internal server error in the NoteController but wont c

                if (entity != null)
                {
                    entity.Title = model.Title;
                    entity.Content = model.Content;
                    entity.ModifiedUTC = DateTimeOffset.UtcNow;

                    return ctx.SaveChanges() == 1;
                }
                return false;
         
            }
        }
        public bool DeleteNote(int noteID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Notes.Single(e => e.NoteID == noteID && e.OwnerID == _userID);

                ctx.Notes.Remove(entity);
                return ctx.SaveChanges() == 1;
            }
        }
    }
}

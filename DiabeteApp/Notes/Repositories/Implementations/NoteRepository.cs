using MongoDB.Driver;
using Notes.Data;
using Notes.Entities;
using Notes.Repositories.Interfaces;

namespace Notes.Repositories.Implementations
{
    public class NoteRepository : INoteRepository
    {
        private readonly IMongoCollection<Note> _notesCollection;

        public NoteRepository(MongoDbSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);

            _notesCollection = database.GetCollection<Note>(mongoDbSettings.NotesCollectionName);
        }

        public async Task<Note> CreateAsync(Note note)
        {
            await _notesCollection.InsertOneAsync(note);
            return note;
        }

        public async Task<Note?> GetByIdAsync(string id)
        {
            var filter = Builders<Note>.Filter.Eq(n => n.Id, id);
            return await _notesCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Note>> GetByPatientAsync(int idPatient)
        {
            var filter = Builders<Note>.Filter.Eq(n => n.IdPatient, idPatient);
            return await _notesCollection
                .Find(filter)
                .SortByDescending(n => n.DateCreation)
                .ToListAsync();
        }
    }
}

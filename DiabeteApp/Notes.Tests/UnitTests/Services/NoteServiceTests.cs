using Moq;
using Notes.Entities;
using Notes.DTOs;
using Notes.Repositories.Interfaces;
using Notes.Services.Implementations;

namespace Notes.Tests.UnitTests.Services
{
    public class NoteServiceTests
    {
        private readonly Mock<INoteRepository> _repositoryMock;
        private readonly NoteService _service;

        public NoteServiceTests()
        {
            _repositoryMock = new Mock<INoteRepository>();
            _service = new NoteService(_repositoryMock.Object);
        }


        // -- CreateAsync --

        [Fact]
        public async Task CreateAsync_DonneesValides_RetourneNouvelleNote()
        {
            // Arrange
            var dto = new NoteCreateDto
            {
                Contenu = "Contenu Test 1",
                IdPatient = 1
            };

            var entity = new Note
            {
                Id = "1",
                Contenu = dto.Contenu,
                DateCreation = new DateTime(2026, 1, 1),
                IdPatient = dto.IdPatient
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Note>()))
                                  .ReturnsAsync(entity);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Contenu, result.Contenu);
            Assert.Equal(entity.DateCreation, result.DateCreation);
            Assert.Equal(entity.IdPatient, result.IdPatient);
        }

        [Fact]
        public async Task CreateAsync_DonneesInvalides_DeclencheException()
        {
            // Arrange
            var dto = new NoteCreateDto
            {
                Contenu = "",
                IdPatient = 1
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Note>()))
                .ThrowsAsync(new ArgumentException("Le contenu est requis."));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }


        // -- GetByPatientAsync --

        [Fact]
        public async Task GetByPatientAsync_IdValide_RetourneListeNotesPourPatient1()
        {
            // Arrange
            var entities = new List<Note>
            {
                new Note
                {
                    Id = "1",
                    Contenu = "Contenu Test 1",
                    DateCreation = new DateTime(2026, 1, 1),
                    IdPatient = 1
                },

                new Note
                {
                    Id = "2",
                    Contenu = "Contenu Test 2",
                    DateCreation = new DateTime(2026, 2, 2),
                    IdPatient = 1
                }
            };

            _repositoryMock.Setup(r => r.GetByPatientAsync(1))
                .ReturnsAsync(entities);

            // Act
            var result = await _service.GetByPatientAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.GetByPatientAsync(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entities.Count, result.Count());
        }


        // -- GetByIdAsync --

        [Fact]
        public async Task GetByIdAsync_IdValide_RetourneNote()
        {
            // Arrange
            var entity = new Note
            {
                Id = "1",
                Contenu = "Contenu Test 1",
                DateCreation = new DateTime(2026, 1, 1),
                IdPatient = 1
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(entity.Id);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(entity.Id), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_IdInexistant_RetourneNull()
        {
            // Arrange
            string id = "999";
            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Note?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            Assert.Null(result);
        }
    }
};

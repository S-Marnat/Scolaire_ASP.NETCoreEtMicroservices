using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Entities;
using Patients.DTOs;
using Patients.Repositories.Interfaces;
using Patients.Services.Implementations;

namespace Patients.Tests.UnitTests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();
            _service = new PatientService(_repositoryMock.Object);
        }


        // -- CreateAsync --

        [Fact]
        public async Task CreateAsync_DonneesValides_RetourneNouveauPatient()
        {
            // Arrange
            var dto = new PatientCreateDto
            {
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var entity = new Patient
            {
                Id = 1,
                Prenom = dto.Prenom,
                Nom = dto.Nom,
                DateDeNaissance = dto.DateDeNaissance,
                Genre = dto.Genre,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Patient>()))
                                  .ReturnsAsync(entity);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Patient>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Prenom, result.Prenom);
            Assert.Equal(entity.Nom, result.Nom);
            Assert.Equal(entity.DateDeNaissance, result.DateDeNaissance);
            Assert.Equal(entity.Genre, result.Genre);
            Assert.Equal(entity.Adresse, result.Adresse);
            Assert.Equal(entity.Telephone, result.Telephone);
        }

        [Fact]
        public async Task CreateAsync_DonneesInvalides_DeclencheException()
        {
            // Arrange
            var dto = new PatientCreateDto
            {
                Prenom = "",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Patient>()))
                .ThrowsAsync(new ArgumentException("Le prénom est requis."));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }


        // -- GetAllAsync --

        [Fact]
        public async Task GetAllAsync_IdValide_RetourneListePatients()
        {
            // Arrange
            var entities = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    Prenom = "Prénom Test 1",
                    Nom = "Nom Test 1",
                    DateDeNaissance = new DateTime(2001, 1, 1),
                    Genre = "F",
                    Adresse = "Adresse Test 1",
                    Telephone = "111-111-1111"
                },

                new Patient
                {
                    Id = 2,
                    Prenom = "Prénom Test 2",
                    Nom = "Nom Test 2",
                    DateDeNaissance = new DateTime(2002, 2, 2),
                    Genre = "M",
                    Adresse = "Adresse Test 2",
                    Telephone = "222-222-2222"
                }
            };

            _repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entities.Count, result.Count());
        }


        // -- GetByIdAsync --

        [Fact]
        public async Task GetByIdAsync_IdValide_RetournePatient()
        {
            // Arrange
            var entity = new Patient
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
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
            int id = 999;
            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            Assert.Null(result);
        }


        // -- UpdateAsync --

        [Fact]
        public async Task UpdateAsync_IdCorrespond_MetAJourPatient()
        {
            // Arrange
            var dto = new PatientUpdateDto
            {
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var entity = new Patient
            {
                Id = 1,
                Prenom = "Prénom Update 1",
                Nom = "Nom Update 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Prenom, result.Prenom);
            Assert.Equal(entity.Nom, result.Nom);
            Assert.Equal(entity.DateDeNaissance, result.DateDeNaissance);
            Assert.Equal(entity.Genre, result.Genre);
            Assert.Equal(entity.Adresse, result.Adresse);
            Assert.Equal(entity.Telephone, result.Telephone);
        }

        [Fact]
        public async Task UpdateAsync_IdInexistant_RetourneNull()
        {
            // Arrange
            var dto = new PatientUpdateDto
            {
                Prenom = "Prénom Test 999",
                Nom = "Nom Test 999",
                DateDeNaissance = new DateTime(1999, 9, 9),
                Genre = "F",
                Adresse = "Adresse Test 999",
                Telephone = "999-999-999"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Patient?)null);
            
            // Act
            var result = await _service.UpdateAsync(999, dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Never);
            Assert.Null(result);
        }
    }
};

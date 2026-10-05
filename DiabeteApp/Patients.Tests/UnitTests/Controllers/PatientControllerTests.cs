using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.DTOs;
using Patients.Services.Interfaces;

namespace Patients.Tests.UnitTests.Controllers
{
    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _serviceMock;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _serviceMock = new Mock<IPatientService>();
            _controller = new PatientController(_serviceMock.Object);
        }


        // -- GetPatients --

        [Fact]
        public async Task GetPatients_AucuneCondition_RetourneOkAvecListePatients()
        {
            // Arrange
            var patients = new List<PatientReadDto>
            {
                new PatientReadDto
                {
                    Id = 1,
                    Prenom = "Prénom Test 1",
                    Nom = "Nom Test 1",
                    DateDeNaissance = new DateTime(2001, 1, 1),
                    Genre = "F",
                    Adresse = "Adresse Test 1",
                    Telephone = "111-111-1111"
                },

                new PatientReadDto
                {
                    Id = 2,
                    Prenom = "Prénom  Test 2",
                    Nom = "Nom Test 2",
                    DateDeNaissance = new DateTime(2002, 2, 2),
                    Genre = "M",
                    Adresse = "Adresse Test 2",
                    Telephone = "222-222-2222"
                }
            };

            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(patients);

            // Act
            var result = await _controller.GetPatients();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PatientReadDto>>(ok.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetPatients_AucuneDonnee_RetourneOkAvecListeVide()
        {
            // Arrange
            var patients = new List<PatientReadDto>();
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(patients);
            
            // Act
            var result = await _controller.GetPatients();
            
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<PatientReadDto>>(ok.Value);
            Assert.Empty(returnedList);
        }

        [Fact]
        public async Task GetPatients_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                .ThrowsAsync(new Exception("Erreur de test"));
            
            // Act
            var result = await _controller.GetPatients();
            
            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, internalServerError.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerError.Value);
        }


        // -- GetPatient --

        [Fact]
        public async Task GetPatient_IdValide_RetourneOkAvecPatient()
        {
            // Arrange
            var patients = new List<PatientReadDto>
            {
                new PatientReadDto
                {
                    Id = 1,
                    Prenom = "Prénom Test 1",
                    Nom = "Nom Test 1",
                    DateDeNaissance = new DateTime(2001, 1, 1),
                    Genre = "F",
                    Adresse = "Adresse Test 1",
                    Telephone = "111-111-1111"
                }
            };

            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(patients.First());

            // Act
            var result = await _controller.GetPatient(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientReadDto>(ok.Value);
            Assert.Equal("Prénom Test 1", returnedPatient.Prenom);
        }

        [Fact]
        public async Task GetPatient_IdInexistant_RetourneNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((PatientReadDto)null);

            // Act
            var result = await _controller.GetPatient(999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("L'Id renseigné ne correspond à aucun patient.", notFound.Value);
        }

        [Fact]
        public async Task GetPatient_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erreur de test"));

            // Act
            var result = await _controller.GetPatient(1);

            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, internalServerError.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerError.Value);
        }


        // -- PostPatient --

        [Fact]
        public async Task PostPatient_DonneesValides_RetourneCreatedAvecPatient()
        {
            // Arrange
            var newPatient = new PatientCreateDto
            {
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };
            var createdPatient = new PatientReadDto
            {
                Id = 1,
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            _serviceMock.Setup(s => s.CreateAsync(newPatient))
                .ReturnsAsync(createdPatient);

            // Act
            var result = await _controller.PostPatient(newPatient);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientReadDto>(created.Value);
            Assert.Equal(1, returnedPatient.Id);
        }

        [Fact]
        public async Task PostPatient_ModelStateInvalide_RetournerBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nom", "Required");

            // Act
            var result = await _controller.PostPatient(new PatientCreateDto());

            // Assert
            _serviceMock.Verify(s => s.CreateAsync(It.IsAny<PatientCreateDto>()), Times.Never);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Les informations fournies pour le patient sont invalides.", badRequest.Value);
        }

        [Fact]
        public async Task PostPatient_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<PatientCreateDto>()))
                .ThrowsAsync(new Exception("Erreur de test"));

            // Act
            var result = await _controller.PostPatient(new PatientCreateDto());

            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, internalServerError.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerError.Value);
        }


        // -- PutPatient --

        [Fact]
        public async Task PutPatient_IdCorrespond_RetourneOkAvecPatient()
        {
            // Arrange
            var patient = new PatientUpdateDto
            {
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var updatedPatient = new PatientReadDto
            {
                Id = 1,
                Prenom = "Prénom Updated 1",
                Nom = "Nom Updated 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            _serviceMock.Setup(s => s.UpdateAsync(1, patient))
                .ReturnsAsync(updatedPatient);

            // Act
            var result = await _controller.PutPatient(1, patient);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returnedPatient = Assert.IsType<PatientReadDto>(ok.Value);
            Assert.Equal(1, returnedPatient.Id);
        }

        [Fact]
        public async Task PutPatient_IdInexistant_RetourneNotFound()
        {
            // Arrange
            var patient = new PatientUpdateDto
            {
                Prenom = "Prénom Test 1",
                Nom = "Nom Test 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            var updatedPatient = new PatientReadDto
            {
                Id = 1,
                Prenom = "Prénom Updated 1",
                Nom = "Nom Updated 1",
                DateDeNaissance = new DateTime(2001, 1, 1),
                Genre = "F",
                Adresse = "Adresse Test 1",
                Telephone = "111-111-1111"
            };

            // Act
            var result = await _controller.PutPatient(99, patient);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("L'Id renseigné ne correspond à aucun patient.", notFound.Value);
        }

        [Fact]
        public async Task PutPatient_ModelStateInvalide_RetournerBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nom", "Required");

            // Act
            var result = await _controller.PutPatient(1, new PatientUpdateDto());

            // Assert
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<PatientUpdateDto>()), Times.Never);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les informations fournies pour le patient Id=1 sont invalides.", badRequest.Value);
        }

        [Fact]
        public async Task PutPatient_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<PatientUpdateDto>()))
                .ThrowsAsync(new Exception("Erreur de test"));

            // Act
            var result = await _controller.PutPatient(1, new PatientUpdateDto());

            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerError.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerError.Value);
        }
    }
}
